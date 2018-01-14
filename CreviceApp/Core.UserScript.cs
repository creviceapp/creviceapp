using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;


namespace CreviceApp
{
    public class UserScript
    {
        public class Watcher : FileSystemWatcher
        {
            public Watcher(string Path, ISynchronizeInvoke SynchronizingObject)
            {
                this.Path = Path;
                this.Filter = "*.csx";
                this.NotifyFilter = NotifyFilters.FileName |
                                    NotifyFilters.LastWrite;
                this.IncludeSubdirectories = true;
                this.SynchronizingObject = SynchronizingObject;
            }
        }

        protected readonly AppGlobal Global;

        public UserScript(AppGlobal Global)
        {
            this.Global = Global;
            Directory.CreateDirectory(UserDirectory);
        }
        
        public Watcher GetWatcher(ISynchronizeInvoke SynchronizingObject)
        {
            return new Watcher(UserDirectory, SynchronizingObject);
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
                var scriptPath = Global.CLIOption.ScriptFile;
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

        public Script ParseScript(string userScriptString)
        {
            var stopwatch = new Stopwatch();
            Verbose.Print("Parsing UserScript...");
            stopwatch.Start();
            var script = CSharpScript.Create(
                userScriptString,
                ScriptOptions.Default
                    .WithSourceResolver(ScriptSourceResolver.Default.WithBaseDirectory(UserDirectory))
                    .WithMetadataResolver(ScriptMetadataResolver.Default.WithBaseDirectory(UserDirectory))
                    .WithReferences(
                        typeof(object).Assembly,      // mscorlib.dll
                        typeof(Uri).Assembly,         // System.dll
                        typeof(Enumerable).Assembly,  // System.Core.dll
                        // For the reference to CreviceApp.exe, `Assembly.GetEntryAssembly()` does not work
                        // properly in the test environment. So instead of it, we should use `typeof(Program).Assembly)` here.
                        typeof(Program).Assembly),    // CreviceApp.exe
                globalsType: typeof(Core.UserScriptExecutionContext));
            stopwatch.Stop();
            Verbose.Print("UserScript parse finished. ({0})", stopwatch.Elapsed);
            return script;
        }

        public UserScriptAssembly.Cache GenerateUserScriptAssemblyCache(string userScriptString, Script parsedScript)
        {
            var stopwatch = new Stopwatch();
            Verbose.Print("Generating UserScriptAssemblyCache...");
            stopwatch.Start();
            var compilation = parsedScript.GetCompilation();
            stopwatch.Stop();
            Verbose.Print("UserScriptAssemblyCache generating finished. ({0})", stopwatch.Elapsed);

            var peStream = new MemoryStream();
            var pdbStream = new MemoryStream();
            Verbose.Print("Genarating UserScriptAssembly...");
            stopwatch.Restart();
            compilation.Emit(peStream, pdbStream);
#if DEBUG
            File.WriteAllBytes((UserScriptFile + ".debug.dll"), peStream.GetBuffer());
            File.WriteAllBytes((UserScriptFile + ".debug.pdb"), pdbStream.GetBuffer());
#endif
            stopwatch.Stop();
            Verbose.Print("UserScriptAssembly generation finished. ({0})", stopwatch.Elapsed);
            return UserScriptAssembly.CreateCache(userScriptString, peStream.GetBuffer(), pdbStream.GetBuffer());
        }

        public System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> CompileUserScript(Script parsedScript)
        {
            var stopwatch = new Stopwatch();
            Verbose.Print("Compiling UserScript...");
            stopwatch.Start();
            var diagnostic = parsedScript.Compile();
            stopwatch.Stop();
            Verbose.Print("UserScript compilation finished. ({0})", stopwatch.Elapsed);
            return diagnostic;
        }

        public void EvaluateUserScript(Core.UserScriptExecutionContext ctx, Script parsedScript)
        {
            var stopwatch = new Stopwatch();
            Verbose.Print("Evaluating UserScript...");
            stopwatch.Start();
            parsedScript.RunAsync(ctx).Wait();
            stopwatch.Stop();
            Verbose.Print("UserScript evaluation finished. ({0})", stopwatch.Elapsed);
        }

        public void EvaluateUserScriptAssembly(Core.UserScriptExecutionContext ctx, UserScriptAssembly.Cache userScriptAssemblyCache)
        {
            // todo: using(Verbose.StopWatch("Loading", "UserScriptAssembly")){}
            var stopwatch = new Stopwatch();
            Verbose.Print("Loading UserScriptAssembly...");
            stopwatch.Start();
            var assembly = Assembly.Load(userScriptAssemblyCache.pe, userScriptAssemblyCache.pdb);
            stopwatch.Stop();
            Verbose.Print("UserScriptAssembly loading finished. ({0})", stopwatch.Elapsed);
            Verbose.Print("Evaluating UserScriptAssembly...");
            stopwatch.Restart();
            var type = assembly.GetType("Submission#0");
            var factory = type.GetMethod("<Factory>");
            var parameters = new object[] { new object[] { ctx, null } };
            var result = factory.Invoke(null, parameters);
            (result as Task<object>).Wait();
            stopwatch.Stop();
            Verbose.Print("UserScriptAssembly evaluation finished. ({0})", stopwatch.Elapsed);
        }

        public UserScriptAssembly.Cache LoadUserScriptAssemblyCache(string userScriptString)
        {
            var stopwatch = new Stopwatch();
            var cacheFile = UserScriptCacheFile;
            if (File.Exists(cacheFile))
            {
                Verbose.Print("Loading UserScriptAssemblyCache...");
                stopwatch.Start();
                var loadedCache = UserScriptAssembly.Load(cacheFile);
                stopwatch.Stop();
                Verbose.Print("UserScriptAssemblyCache loading finished. ({0})", stopwatch.Elapsed);
                if (UserScriptAssembly.IsCompatible(loadedCache, userScriptString))
                {
                    return loadedCache;
                }
                Verbose.Print("UserScriptAssemblyCache was discarded because of the signature was not match with this application.");
            }
            return null;
        }

        public void SaveUserScriptAssemblyCache(UserScriptAssembly.Cache userScriptAssemblyCache)
        {
            var stopwatch = new Stopwatch();
            var cacheFile = UserScriptCacheFile;
            Verbose.Print("Saving UserScriptAssemblyCache...");
            stopwatch.Start();
            UserScriptAssembly.Save(cacheFile, userScriptAssemblyCache);
            stopwatch.Stop();
            Verbose.Print("UserScriptAssemblyCache saving finished. ({0})", stopwatch.Elapsed);
        }
        
        private string GetHighlightedUserScriptString(Microsoft.CodeAnalysis.Diagnostic error)
        {
            var lineSpan = error.Location.GetLineSpan();
            var start = lineSpan.StartLinePosition;
            var end = lineSpan.EndLinePosition;
            var lines = error.Location.SourceTree.GetText().Lines;
            var maxDigits = lines.Count.ToString().Length;
            var range = 5;
            var s = Math.Max(0, start.Line - range);
            var e = Math.Min(lines.Count - 1, end.Line + range);

            var sb = new StringBuilder();
            for (var i = s; i <= e;  i++)
            {
                var line = lines[i].ToString();
                var lineDigits = i.ToString().Length;
                var lineNumber = (new string('0', maxDigits - lineDigits) + i).Substring(0, maxDigits);
                sb.AppendFormat("{0}|{1}\r\n", lineNumber, line);
                if (i == start.Line)
                {
                    var h_start = start.Character;
                    var h_end = i == end.Line ? end.Character : line.Length;
                    sb.Append(new string(' ', maxDigits + 1));
                    sb.Append(new string(' ', h_start));
                    sb.Append(new string('~', Math.Max(1, h_end - h_start)));
                    sb.Append("\r\n");
                }
                else if (i == end.Line)
                {
                    var h_start = 0;
                    var h_end = end.Character;
                    sb.Append(new string(' ', maxDigits + 1));
                    sb.Append(new string(' ', h_start));
                    sb.Append(new string('~', Math.Max(1, h_end - h_start)));
                    sb.Append("\r\n");
                }
            }
            if (e == lines.Count - 1)
            {
                sb.Append("[EOF]\r\n");
            }
            var code = sb.ToString();
            return code;
        }

        public string GetPrettyErrorMessage(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> errors)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var error in errors)
            {
                var lineSpan = error.Location.GetLineSpan();
                var start = lineSpan.StartLinePosition;
                var end = lineSpan.EndLinePosition;

                sb.Append(new string('-', 80));
                sb.Append("\r\n");
                sb.AppendFormat("[{0} Error] {1}\r\n", error.Descriptor.Category, error.Id);
                sb.Append("\r\n");
                sb.AppendFormat("{0}\r\n", error.GetMessage());
                sb.Append("\r\n");
                sb.AppendFormat("Line {0}, Pos {1}\r\n", start.Line, start.Character);
                sb.Append(new string('=', 10));
                sb.Append("\r\n");
                sb.Append(GetHighlightedUserScriptString(error));
                sb.Append(new string('=', 10));
                sb.Append("\r\n");
            }
            sb.Append(new string('-', 80));
            sb.Append("\r\n");
            return sb.ToString();
        }
    }
}
