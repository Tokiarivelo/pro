using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using APMDB.Data;

using Diacritics.Extensions;
using Microsoft.Win32;
using IntégrationPDFGoPress;

namespace IntégrationPDFGoPress.Utils
{
  internal class General
  {
    private static bool invalid;
    private static string DomainMapper(Match match)
    {
      // IdnMapping class with default property values.
      IdnMapping idn = new IdnMapping();

      string domainName = match.Groups[2].Value;
      try
      {
        domainName = idn.GetAscii(domainName);
      }
      catch (ArgumentException)
      {
        invalid = true;
      }
      return match.Groups[1].Value + domainName;

    }


    public static bool IsValidEmail(string strIn)
    {
      invalid = false;

      if (strIn.HasDiacritics()) return false;
      if (string.IsNullOrEmpty(strIn)) return false;

      // Use IdnMapping class to convert Unicode domain names.
      try
      {
        strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper,
                              RegexOptions.None, TimeSpan.FromMilliseconds(200));
      }
      catch (RegexMatchTimeoutException)
      {
        return false;
      }

      if (invalid) return false;

      // Return true if strIn is in valid e-mail format.
      try
      {
        return Regex.IsMatch(strIn,
          @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
          RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
      }
      catch (RegexMatchTimeoutException)
      {
        return false;
      }
    }
    public static bool IsContextChanges(DbContext context)
    {
      try
      {
        var entries = context.ChangeTracker.Entries()
                 .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
                 .ToList();
        return entries.Count > 0;
      }
      catch (Exception ex)
      {
        Logger.LogError(ex.ToString());
        return false;
      }
    }
    public static bool SaveContextChanges(string strMsg, DbContext context)
    {
      try
      {
        context.Database.CommandTimeout = 300;
        string entityName = context.ToString().Split('.').Last();

        var entries = context.ChangeTracker.Entries()
              .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
              .ToList();
        foreach (var entry in entries)
        {
          Logger.LogDebug("Début de la sauvegarde du contexte <" + entityName + ">");

          dynamic entryValues;
          if (entry.State == EntityState.Deleted) entryValues = entry.OriginalValues.PropertyNames;
          else entryValues = entry.CurrentValues.PropertyNames;

          foreach (string o in entryValues)
          {
            var property = entry.Property(o);

            //var  baseType = entry.Entity.GetType().Name;
            //if (baseType != null) 
            string tableName = entry.Entity.GetType().Name.Split('_')[0];

            string currentVal, originalVal;

            switch (entry.State)
            {
              case EntityState.Added:
                currentVal = (property.CurrentValue ?? string.Empty).ToString();
                Logger.LogDebug(entityName + " <" + tableName + "> (" + strMsg + ") - Ajout de <" + property.Name + "> = '" + currentVal + "'.");
                break;
              case EntityState.Modified:
                currentVal = (property.CurrentValue ?? string.Empty).ToString();
                originalVal = (property.OriginalValue ?? string.Empty).ToString();
                if (currentVal != originalVal)
                {
                  Logger.LogDebug(entityName + " Modification de  (" + strMsg + ") -  <" + tableName + ">." + "<" + property.Name + "> de '" + property.OriginalValue + "' à '" + property.CurrentValue + "'.");
                }
                break;
              case EntityState.Deleted:
                originalVal = (property.OriginalValue ?? string.Empty).ToString();
                Logger.LogDebug(entityName + " <" + tableName + "> " + "Effacement de <" + property.Name + ">  (" + strMsg + ") - '" + originalVal + "'.");
                break;
            }

          }
          Logger.LogDebug("Fin de la sauvegarde du contexte <" + entityName + ">");
        }
        try
        {
          context.SaveChanges();
        }
        catch (System.Data.Entity.Validation.DbEntityValidationException ex)
        {
          var sb = new StringBuilder();
          foreach (var failure in ex.EntityValidationErrors)
          {
            sb.AppendFormat("{0} failed validation", failure.Entry.Entity.GetType());
            foreach (var error in failure.ValidationErrors)
            {
              sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
              sb.AppendLine();
            }
          }

          throw new Exception(sb.ToString());
        }
        return true;
      }
      catch (Exception ex)
      {
        Logger.LogError(ex.ToString());
        return false;
      }
    }

    public static string GetParamValue(string strParamName)
    {
      try
      {
        var entities = new APMDBEntities();

        var parameters = from e in entities.Parameters
                         where (e.ParamName == strParamName)
                         select e;
        var parameter = parameters.FirstOrDefault();
        return parameter != null ? parameter.ParamValue : String.Empty;
      }
      catch (Exception ex)
      {
        Logger.LogError(ex.ToString());
        return String.Empty;
      }
    }

    public static string GetDefaultBrowserPath()
    {
      const string urlAssociation = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http";
      const string browserPathKey = @"$BROWSER$\shell\open\command";

      try
      {
        //Read default browser path from userChoiceLKey
        var userChoiceKey = Registry.CurrentUser.OpenSubKey(urlAssociation + @"\UserChoice", false);

        //If user choice was not found, try machine default
        if (userChoiceKey == null)
        {
          //Read default browser path from Win XP registry key
          var browserKey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false) ??
                           Registry.CurrentUser.OpenSubKey(urlAssociation, false);

          //If browser path wasn’t found, try Win Vista (and newer) registry key
          if (browserKey != null)
          {
            var path = CleanifyBrowserPath(browserKey.GetValue(null) as string);
            browserKey.Close();
            return path;
          }
        }
        // user defined browser choice was found
        if (userChoiceKey != null)
        {
          string progId = (userChoiceKey.GetValue("ProgId").ToString());
          userChoiceKey.Close();

          // now look up the path of the executable
          string concreteBrowserKey = browserPathKey.Replace("$BROWSER$", progId);
          var kp = Registry.ClassesRoot.OpenSubKey(concreteBrowserKey, false);
          if (kp != null)
          {
            var browserPath = CleanifyBrowserPath(kp.GetValue(null) as string);
            kp.Close();
            return browserPath;
          }
        }
        return "";
      }
      catch (Exception ex)
      {
        Logger.LogError(ex.ToString());
        return "";
      }
    }

    private static string CleanifyBrowserPath(string p)
    {
      string[] url = p.Split('"');
      string clean = url[1];
      return clean;
    }

  }
}
