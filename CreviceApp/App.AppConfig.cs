using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreviceApp.App
{
    public class AppConfig // Todo: extract interface and make application to use CreviceAppConfig class.
    {
        public readonly CLIOption.Result CLIOption; // Todo: extract this option into this class.
        public readonly Core.Config.UserConfig UserConfig;
        public readonly UI.MainForm MainForm; // Todo: Should not have this instance as a member.

        public AppConfig() : this(App.CLIOption.ParseEnvironmentCommandLine())
        {
                        
        }

        public AppConfig(CLIOption.Result CLIOption)
        {
            this.CLIOption = CLIOption;
            this.UserConfig = new Core.Config.UserConfig();
            this.MainForm = new UI.MainForm(this);
            Directory.CreateDirectory(DefaultUserDirectory);
        }

        // %USERPROFILE%\\AppData\\Roaming\\Crevice\\CreviceApp
        public string DefaultUserDirectory
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Crevice",
                    "CreviceApp");
            }
        }

        // Parent directory of UserScriptFile.
        public string UserDirectory
        {
            get
            {
                return Directory.GetParent(UserScriptFile).FullName;
            }
        }

        public string UserScriptFile
        {
            get
            {
                var scriptPath = CLIOption.ScriptFile;
                if (Path.IsPathRooted(scriptPath))
                {
                    return scriptPath;
                }
                var uri = new Uri(new Uri(DefaultUserDirectory + "\\"), scriptPath);
                return uri.LocalPath;
            }
        }

        public string UserScriptCacheFile
        {
            get
            {
                return UserScriptFile + ".cache";
            }
        }

        public string ReadUserScriptFile()
        {
            if (!File.Exists(UserScriptFile))
            {
                return null;
            }
            return File.ReadAllText(UserScriptFile, Encoding.UTF8);
        }
        
        public string GetOrSetDefaultUserScriptFile(string defaultUserScriptString)
        {
            string userScriptString = ReadUserScriptFile();
            if (userScriptString == null)
            {
                WriteUserScriptFile(defaultUserScriptString);
                return defaultUserScriptString;
            }
            return userScriptString;
        }

        public void WriteUserScriptFile(string scriptFileString)
        {
            File.WriteAllText(UserScriptFile, scriptFileString, Encoding.UTF8);
        }


        /*
                 public string GetUserScriptString()
        {
            if (!Directory.Exists(UserDirectory))
            {
                Directory.CreateDirectory(UserDirectory);
            }
 
            if (!File.Exists(UserScriptFile))
            {
                File.WriteAllText(UserScriptFile, Encoding.UTF8.GetString(Properties.Resources.DefaultUserScript), Encoding.UTF8);
            }
            return File.ReadAllText(UserScriptFile, Encoding.UTF8);
        }
         */
    }
}
