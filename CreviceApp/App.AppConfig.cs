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
    public class AppConfig
    {
        public readonly CLIOption.Result CLIOption;

        public readonly UI.MainForm MainForm;

        public AppConfig() : this(App.CLIOption.ParseEnvironmentCommandLine())
        { }

        public AppConfig(CLIOption.Result CLIOption)
        {
            this.CLIOption = CLIOption;

            Directory.CreateDirectory(UserDirectory);
            
            this.MainForm = new UI.MainForm(this);
        }

        // %USERPROFILE%\\AppData\\Roaming\\Crevice4
        public string DefaultUserDirectory
            => Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Crevice4");

        // Parent directory of UserScriptFile.
        public string UserDirectory
            => Directory.GetParent(UserScriptFile).FullName;

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
            => UserScriptFile + ".cache";

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
            => File.WriteAllText(UserScriptFile, scriptFileString, Encoding.UTF8);
    }
}
