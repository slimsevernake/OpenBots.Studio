using OpenBots.Core.Infrastructure;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System;
using System.Runtime.InteropServices;
using ExcelApplication = Microsoft.Office.Interop.Excel.Application;
using WordApplication = Microsoft.Office.Interop.Word.Application;

namespace OpenBots.Core.Utilities.CommonUtilities
{
    public static class InstanceMethods
    {
        public static void AddAppInstance(this object appObject, IEngine engine, string instanceName)
        {

            if (engine.AppInstances.ContainsKey(instanceName) && engine.EngineSettings.OverrideExistingAppInstances)
                engine.AppInstances.Remove(instanceName);

            else if (engine.AppInstances.ContainsKey(instanceName) && !engine.EngineSettings.OverrideExistingAppInstances)
            {
                throw new Exception("App Instance already exists and override has been disabled in engine settings! " +
                    "Enable override existing app instances or use unique instance names!");
            }

            try
            {
                engine.AppInstances.Add(instanceName, appObject);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static object GetAppInstance(this string instanceName, IEngine engine)
        {
            try
            {
                if (engine.AppInstances.TryGetValue(instanceName, out object appObject))
                    return appObject;

                throw new Exception("App Instance '" + instanceName + "' not found!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void RemoveAppInstance(this string instanceName, IEngine engine)
        {
            try
            {
                if (engine.AppInstances.ContainsKey(instanceName))
                    engine.AppInstances.Remove(instanceName);
                else
                    throw new Exception("App Instance '" + instanceName + "' not found!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool InstanceExists(this string instanceName, IEngine engine)
        {          
            try
            {
                if (engine.AppInstances.TryGetValue(instanceName, out object appObject))
                {                   
                    string appType = appObject.GetType().ToString();

                    switch (appType.ToString())
                    {
                        case "Microsoft.Office.Interop.Excel.ApplicationClass":
                            var excelApp = (ExcelApplication)appObject;
                            if (excelApp.Application != null)
                                return true;
                            break;
                        case "Microsoft.Office.Interop.Word.ApplicationClass":
                            var wordApp = (WordApplication)appObject;
                            if (wordApp.Application != null)
                                return true;
                            break;
                        case "OpenQA.Selenium.Chrome.ChromeDriver":
                            var chromeDriver = (ChromeDriver)appObject;
                            if (chromeDriver.CurrentWindowHandle != null)
                                return true;
                            break;
                        case "OpenQA.Selenium.Firefox.FirefoxDriver":
                            var firefoxDriver = (FirefoxDriver)appObject;
                            if (firefoxDriver.CurrentWindowHandle != null)
                                return true;
                            break;
                        case "OpenQA.Selenium.IE.InternetExplorerDriver":
                            var ieDriver = (InternetExplorerDriver)appObject;
                            if (ieDriver.CurrentWindowHandle != null)
                                return true;
                            break;
                        default:
                            throw new InvalidComObjectException($"App instance '{appType}' not supported.");
                    }                                                             
                }
                return false;                    
            }
            catch (Exception ex)
            {
                if (ex is InvalidComObjectException)
                    throw ex;

                return false;
            }
        }
    }
}
