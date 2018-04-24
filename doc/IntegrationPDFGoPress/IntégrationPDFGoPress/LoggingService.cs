// ==========================================================================
//      AUXIPRESS SA/NV
//      Copyright(C)  2011.  Tous droits réservés.
// ==========================================================================
//   Document    :  General.cs
//   Département :  IT, Auderghem, Belgium
//   Auteur      :  Fabrice De Nève
//   email       :  fabrice.deneve@auxipress.be
// =========================================================================

using System;
using System.DirectoryServices;
using System.Linq;
using APMDB.Data;
using NLog;

namespace IntégrationPDFGoPress
{
  public class LoggingService
  {
    private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
    public void LogException(int intSeverityID, string strFileName, string strFID, string strMsg)
    {
      LogEventInfo logEventInfo = new LogEventInfo();
      switch (intSeverityID)
      {
        case 1:
          logEventInfo.Level = LogLevel.Fatal;
          break;
        case 2:
          logEventInfo.Level = LogLevel.Error;
          break;
        case 3:
          logEventInfo.Level = LogLevel.Warn;
          break;
        case 4:
          logEventInfo.Level = LogLevel.Info;
          break;
        case 5:
          logEventInfo.Level = LogLevel.Debug;
          break;
        default:
          logEventInfo.Level = LogLevel.Debug;
          break;
      }
      logEventInfo.Properties["SEVERITY"] = intSeverityID;
      logEventInfo.Properties["LOGFILE"] = strFileName;
      logEventInfo.Properties["LOGPROCEDURE"] = strFID;
      logEventInfo.Message = strMsg;
      // Récpère les informations de l'utilisateur authentifié sur le domaine
      logEventInfo.Properties["USERNAME"] = Environment.UserName;
      logEventInfo.Properties["DISPLAYNAME"] = Environment.UserName; //GetDisplayName(Environment.UserName);
      // Log l'information via NLog (cf fichier NLog.config
      Logger.Log(logEventInfo);
    }


    //public string GetUser()
    //{
    //  string strUserName = GetDisplayName(Environment.UserName);
    //  if (strUserName == "") strUserName = Environment.UserName;
    //  return strUserName;
    //}

    public string GetLDAPServer()
    {
      var entities = new APMDBEntities();
      var result = from e in entities.Parameters
                   where (e.ParamName.ToLowerInvariant() == "ldapserver")
                   select e.ParamName;
      return result.FirstOrDefault();
    }


    // --------------------------------------------------------------------------------
    //  Routine     :  GetDisplayName [function]
    //  Auteur      :  Fabrice, le 08/12/2011 à 14:25:02
    // --------------------------------------------------------------------------------
    //public string GetDisplayName(string strUserName)
    //{
    //  try
    //  {
    //    // domainedhcp.euroargus.be
    //    return GetAttribute(GetLDAPServer(), strUserName, "displayname");
    //  }
    //  catch (Exception)
    //  {
    //    //LogEvt.LogError(FILENAME, FID, ex.Message);
    //    return strUserName;
    //  }
    //}

    // --------------------------------------------------------------------------------
    //  Routine     :  GetAttribute [function]
    //  Auteur      :  Fabrice, le 08/12/2011 à 14:24:54
    // --------------------------------------------------------------------------------
    public static string GetAttribute(string LDAPServerName, string strUserName, string strAttribute)
    {
      string strReturnValue = string.Empty;

      try
      {
        DirectoryEntry de = new DirectoryEntry("LDAP://" + LDAPServerName);
        DirectorySearcher ds = new DirectorySearcher(de)
        {
          Filter =
                "(&(objectClass=user)(objectCategory=person)(sAMAccountName=" + strUserName + "))"
        };
        SearchResultCollection results = ds.FindAll();
        foreach (SearchResult result in results)
        {
          strReturnValue = GetProperty(result, strAttribute);
        }
      }
      catch (Exception)
      {
        //LogEvt.LogError(FILENAME, FID, ex.ToString());
      }
      return strReturnValue ?? string.Empty;
    }

    // --------------------------------------------------------------------------------
    //  Routine     :  GetProperty [function]
    //  Auteur      :  Fabrice, le 08/12/2011 à 14:25:11
    // --------------------------------------------------------------------------------
    public static string GetProperty(SearchResult searchResult, string PropertyName)
    {
      if (searchResult.Properties.Contains(PropertyName))
      {
        return searchResult.Properties[PropertyName][0].ToString();
      }
      return string.Empty;
    }

  }
}
