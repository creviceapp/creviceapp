using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crevice.Config
{
    using Crevice.UI;
    
    public class GlobalConfig
    {
        public readonly CLIOption.Result CLIOption;

        public readonly MainFormBase MainForm;

        public GlobalConfig()
            : this(Config.CLIOption.ParseEnvironmentCommandLine())
        { }

        public GlobalConfig(CLIOption.Result cliOption, MainFormBase mainForm = null)
        {
            CLIOption = cliOption;
            MainForm = mainForm;
            Directory.CreateDirectory(UserDirectory);
        }
        
        // %USERPROFILE%\\AppData\\Roaming\\Crevice4
        public string DefaultUserDirectory
            => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Crevice4");

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

        internal static string DisableIDESupportLoadDirectives(string userScriptString)
            => userScriptString.Replace("#load \"IDESupport", "//#load \"IDESupport");

        private string ReadUserScriptFile()
        {
            if (!File.Exists(UserScriptFile))
            {
                return null;
            }
            return File.ReadAllText(UserScriptFile, Encoding.UTF8);
        }
        
        public string GetOrSetDefaultUserScriptFile(string defaultUserScriptString, bool disableIDESupport = true)
        {
            string userScriptString = ReadUserScriptFile();
            if (userScriptString == null)
            {
                WriteUserScriptFile(defaultUserScriptString);
                userScriptString = defaultUserScriptString;
            }
            return disableIDESupport ? DisableIDESupportLoadDirectives(userScriptString) : userScriptString;
        }

        public void WriteUserScriptFile(string scriptFileString)
            => File.WriteAllText(UserScriptFile, scriptFileString, Encoding.UTF8);
    }
}
