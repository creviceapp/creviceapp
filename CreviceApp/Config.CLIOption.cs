using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace Crevice.Config
{
    public class CLIOption
    {
        [CommandLine.Option('g', "nogui", 
            DefaultValue = false,
            HelpText = "Disable GUI features. Set to true if you use Crevice as a CUI application.")]
        public bool NoGUI
        {
            get; set;
        }

        [CommandLine.Option('n', "nocache",
            DefaultValue = false,
            HelpText = "Disable user script assembly caching. Strongly recommend this value to false because compiling task consumes CPU resources every startup of application if true.")]
        public bool NoCache
        {
            get; set;
        }
        
        [CommandLine.Option('s', "script",
            DefaultValue = "default.csx",
            HelpText = "Path to user script file. " +
                       "Use this option if you need to change the default location of user script file. " + 
                       "If given value is relative path, Crevice will resolve it to absolute path based on the default directory (%USERPROFILE%\\AppData\\Roaming\\Crevice\\CreviceApp).")]
        public string ScriptFile
        {
            get; set;
        }

        [CommandLine.Option('p', "priority",
            DefaultValue = ProcessPriorityClass.High,
            HelpText = "Process priority. " +
                       "Acceptable values are the following: AboveNormal, BelowNormal, High, Idle, Normal, RealTime.")]
        public ProcessPriorityClass ProcessPriority
        {
            get; set;
        }

        [CommandLine.Option('V', "verbose",
            DefaultValue = false,
            HelpText = "Show details about running application.")]
        public bool Verbose
        {
            get; set;
        }

        [CommandLine.Option('v', "version",
            DefaultValue = false,
            HelpText = "Display product version.")]
        public bool Version
        {
            get; set;
        }

        [CommandLine.ParserState]
        public CommandLine.IParserState LastParserState { get; set; }

        [CommandLine.HelpOption]
        public string GetUsage()
        {
            var help = new CommandLine.Text.HelpText();
            help.AdditionalNewLineAfterOption = true;
            help.AddDashesToOption = true;

            help.AddPreOptionsLine("Usage:\r\n" +
                                   "  crevice4.exe [--nogui] [--script path] [--help]");
            if (LastParserState?.Errors.Any() == true)
            {
                var errors = help.RenderParsingErrorsText(this, 2);
                if (!string.IsNullOrEmpty(errors))
                {
                    help.AddPreOptionsLine("");
                    help.AddPreOptionsLine("Error(s):");
                    help.AddPreOptionsLine(errors);
                    help.AddPreOptionsLine("");
                }
            }
            help.AddOptions(this);
            return help;
        }
        
        public class Result
        {
            private CLIOption CLIOption { set; get; }

            public bool ParseSuccess { internal set; get; }

            public string HelpMessage { internal set; get; }

            public Result(CLIOption cliOption, bool parseSuccess, string helpMessage)
            {
                this.CLIOption = cliOption;
                this.ParseSuccess = parseSuccess;
                this.HelpMessage = helpMessage;
            }

            public bool NoGUI
            {
                get { return CLIOption.NoGUI; }
            }

            public bool NoCache
            {
                get { return CLIOption.NoCache; }
            }
            
            public bool Verbose
            {
                get { return CLIOption.Verbose; }
            }
            
            public bool Version
            {
                get { return CLIOption.Version; }
            }

            public ProcessPriorityClass ProcessPriority
            {
                get { return CLIOption.ProcessPriority; }
            }

            public string ScriptFile
            {
                get { return CLIOption.ScriptFile; }
            }

            public string VersionMessage
            {
                get
                {
                    return string.Format("{0} Version {1}", Application.ProductName, Application.ProductVersion);
                }
            }
            
            public string helpMessageHtml
            {
                get
                {
                    var html = HttpUtility.HtmlEncode(HelpMessage);
                    html = html.Replace("\r\n", "<br>");
                    return html;
                }
            }
        }

        public static Result Parse(string[] args)
        {
            var cliOption = new CLIOption();
            var stringWriter = new StringWriter();
            var parser = new CommandLine.Parser(with => with.HelpWriter = stringWriter);

            var tryParse = parser.ParseArguments(args, cliOption);
            stringWriter.Close();
            var helpMessage = stringWriter.ToString();

            var result = new Result(cliOption, tryParse, helpMessage);
            return result;
        }

        public static Result ParseEnvironmentCommandLine()
        {
            var args = Environment.GetCommandLineArgs().ToList().Skip(1).ToArray();
            return Parse(args);
        }
    }
}
