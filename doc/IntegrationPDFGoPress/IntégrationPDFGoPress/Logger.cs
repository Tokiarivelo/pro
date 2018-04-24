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
using System.Diagnostics;
using System.Reflection;

namespace IntégrationPDFGoPress
{
    public static class Logger
  {
    public enum LogType
    {
      Fatal = 1,
      Error = 2,
      Warn = 3,
      Info = 4,
      Debug = 5,
      Trace = 6
    }

    public static int EnumValue(LogType enumValue)
    {
      return (int)enumValue;
    }

    public static bool LogDebug(string strMsg)
    {
      return WriteLog(LogType.Debug, strMsg);
    }

    public static bool LogInfo(string strMsg)
    {
      return WriteLog(LogType.Info, strMsg);
    }

    public static bool LogWarn(string strMsg)
    {
      return WriteLog(LogType.Warn, strMsg);
    }

    public static bool LogError(string strMsg)
    {
      return WriteLog(LogType.Error, strMsg);
    }

    public static bool LogCritical(string strMsg)
    {
      return WriteLog(LogType.Fatal, strMsg);
    }

    private static bool WriteLog(LogType type, string strMsg)
    {
      try
      {
        StackTrace stackTrace = new StackTrace();
        MethodBase methodBase = stackTrace.GetFrame(2).GetMethod();
        var logErrorServiceClient = new IntégrationPDFGoPress.LoggingService();
        // ReSharper disable once PossibleNullReferenceException
        logErrorServiceClient.LogException(EnumValue(type), methodBase.ReflectedType.FullName, methodBase.Name, strMsg);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }
  }
}
