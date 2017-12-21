using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CreviceApp
{
    public class CLIOption
    {
        [CommandLine.Option("nogui", 
            DefaultValue = false,
            HelpText = "Whether disable GUI features or not. Set to true if you use CreviceApp as a CUI application.")]
        public bool NoGUI
        {
            get; set;
        }
        
        [CommandLine.Option("script",
            DefaultValue = "default.csx",
            HelpText = "The path to the user script file. " + 
                       "Use this option if you need to change the default location of the user script. " + 
                       "If given value is relative path, CreviceApp will resolve it to absolute path based on the default folder (%USERPROFILE%\\AppData\\Roaming\\Crevice\\CreviceApp).")]
        public string Script
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
                                   "  CreviceApp.exe [--nogui] [--script path] [--help]");
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
            public bool NoGUI { internal set; get; }
            public string Script { internal set; get; }
            public bool parseSuccess { internal set; get; }
            public string helpMessage { internal set; get; }
            public string helpMessageHtml
            { get
                {
                    var html = HttpUtility.HtmlEncode(helpMessage);
                    html = html.Replace("\r\n", "<br>");
                    return html;
                }
            }

            public Result(bool NoGUI, string Script, bool parseSuccess, string helpMessage)
            {
                this.NoGUI = NoGUI;
                this.Script = Script;
                this.parseSuccess = parseSuccess;
                this.helpMessage = helpMessage;
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

            var result = new Result(cliOption.NoGUI, cliOption.Script, tryParse, helpMessage);
            return result;
        }

        public static Result ParseEnvironmentCommandLine()
        {
            var args = Environment.GetCommandLineArgs().ToList().Skip(1).ToArray();
            return Parse(args);
        }
    }
}
