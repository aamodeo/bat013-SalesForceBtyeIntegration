using log4net;
using System;
using System.IO;

namespace SFClientServiceLogger
{
    public class LoggerService
    {
        #region Global variable declaration
        public static ILog log;
        #endregion

        #region Methods

        private static ILog GetLogger(string logger)
        {
            return LogManager.GetLogger(logger);
        }

        public void Init()
        {
            string errorLogFolder = BuildLogFolderStructure() + @"ByteProSFClient.log";
            log4net.GlobalContext.Properties["LogPath"] = String.Concat("", errorLogFolder);
            log4net.GlobalContext.Properties["NDC"] =
                    "========================================================================================================";
            log4net.Config.XmlConfigurator.Configure();
            log = LogManager.GetLogger("UserId");
        }

        #region Method : BuildLogFolderStructure        

        public string BuildLogFolderStructure()
        {
            string strFolderName = DateTime.Today.Year.ToString() + @"\";
            try
            {
                strFolderName += DateTime.Now.ToString("MMMM") + @"\";

                string CurrDirectoryPath = strFolderName;
                if (!File.Exists(CurrDirectoryPath))
                {
                    Directory.CreateDirectory(CurrDirectoryPath);
                }
                strFolderName += DateTime.Today.ToShortDateString();
                strFolderName = strFolderName.Replace(@"/", @"-");

            }
            catch (Exception ex)
            {
                LoggerService.Error("", "BuildLogFolderStructure: " + "\r\nError Message: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
            }
            return strFolderName;

        }
        #endregion

        public static string DrawLine()
        {
            try
            {
                string symbol = "====================================================================";
                return symbol;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void abc()
        {
            try
            {
                log = LogManager.GetLogger("UserId");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void Debug(string logger, object obj)
        {
            try
            {
                string CustError = obj.ToString();
                GetLogger(logger).Debug(CustError);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void Debug(string logger, object obj, Exception ex)
        {
            GetLogger(logger).Debug(obj, ex);

        }

        public static void DebugFormat(string logger, string format, object obj)
        {
            GetLogger(logger).DebugFormat(format, obj);

        }

        public static void DebugFormat(string logger, string format, params Object[] objArray)
        {
            GetLogger(logger).DebugFormat(format, objArray);

        }

        public static void DebugFormat(string logger, IFormatProvider iformatprovider, string format, params Object[] objArray)
        {
            GetLogger(logger).DebugFormat(iformatprovider, format, objArray);

        }

        public static void DebugFormat(string logger, string format, object obj1, object obj2)
        {
            GetLogger(logger).DebugFormat(format, obj1, obj2);
        }

        public static void DebugFormat(string logger, string format, object obj1, object obj2, object obj3)
        {
            GetLogger(logger).DebugFormat(format, obj1, obj2, obj3);
        }

        public static void Error(string logger, object obj)
        {
            GetLogger(logger).Error(obj);
        }

        public static void Error(string logger, object obj, Exception ex)
        {
            GetLogger(logger).Error(obj, ex);
        }

        public static void ErrorFormat(string logger, string format, object obj)
        {
            GetLogger(logger).ErrorFormat(format, obj);
        }

        public static void ErrorFormat(string logger, string format, params Object[] objArray)
        {
            GetLogger(logger).ErrorFormat(format, objArray);
        }
        public static void ErrorFormat(string logger, IFormatProvider iformatprovider, string format, params Object[] objArray)
        {
            GetLogger(logger).ErrorFormat(iformatprovider, format, objArray);
        }
        public static void ErrorFormat(string logger, string format, object obj1, object obj2)
        {
            GetLogger(logger).ErrorFormat(format, obj1, obj2);
        }
        public static void ErrorFormat(string logger, string format, object obj1, object obj2, object obj3)
        {
            GetLogger(logger).ErrorFormat(format, obj1, obj2, obj3);
        }

        public static void Fatal(string logger, object obj)
        {
            GetLogger(logger).Fatal(obj);
        }
        public static void Fatal(string logger, object obj, Exception ex)
        {
            GetLogger(logger).Fatal(obj, ex);
        }
        public static void FatalFormat(string logger, string format, object obj)
        {
            GetLogger(logger).FatalFormat(format, obj);
        }

        public static void FatalFormat(string logger, string format, params Object[] objArray)
        {
            GetLogger(logger).FatalFormat(format, objArray);
        }
        public static void FatalFormat(string logger, IFormatProvider iformatprovider, string format, params Object[] objArray)
        {
            GetLogger(logger).FatalFormat(iformatprovider, format, objArray);
        }
        public static void FatalFormat(string logger, string format, object obj1, object obj2)
        {
            GetLogger(logger).FatalFormat(format, obj1, obj2);
        }
        public static void FatalFormat(string logger, string format, object obj1, object obj2, object obj3)
        {
            GetLogger(logger).FatalFormat(format, obj1, obj2, obj3);
        }

        public static void Info(string logger, object obj)
        {
            GetLogger(logger).Info(obj);
        }
        public static void Info(string logger, object obj, Exception ex)
        {
            GetLogger(logger).Info(obj, ex);
        }

        public static void InfoFormat(string logger, string format, object obj)
        {
            GetLogger(logger).InfoFormat(format, obj);
        }

        public static void InfoFormat(string logger, string format, params Object[] objArray)
        {
            GetLogger(logger).InfoFormat(format, objArray);
        }
        public static void InfoFormat(string logger, IFormatProvider iformatprovider, string format, params Object[] objArray)
        {
            GetLogger(logger).InfoFormat(iformatprovider, format, objArray);
        }
        public static void InfoFormat(string logger, string format, object obj1, object obj2)
        {
            GetLogger(logger).InfoFormat(format, obj1, obj2);
        }
        public static void InfoFormat(string logger, string format, object obj1, object obj2, object obj3)
        {
            GetLogger(logger).InfoFormat(format, obj1, obj2, obj3);
        }
        public static void Warn(string logger, object obj)
        {
            GetLogger(logger).Warn(obj);
        }
        public static void Warn(string logger, object obj, Exception ex)
        {
            GetLogger(logger).Warn(obj, ex);
        }

        public static void WarnFormat(string logger, string format, object obj)
        {
            GetLogger(logger).WarnFormat(format, obj);
        }

        public static void WarnFormat(string logger, string format, params Object[] objArray)
        {
            GetLogger(logger).WarnFormat(format, objArray);
        }
        public static void WarnFormat(string logger, IFormatProvider iformatprovider, string format, params Object[] objArray)
        {
            GetLogger(logger).WarnFormat(iformatprovider, format, objArray);
        }
        public static void WarnFormat(string logger, string format, object obj1, object obj2)
        {
            GetLogger(logger).WarnFormat(format, obj1, obj2);
        }
        public static void WarnFormat(string logger, string format, object obj1, object obj2, object obj3)
        {
            GetLogger(logger).WarnFormat(format, obj1, obj2, obj3);
        }
        #endregion
    }
}
