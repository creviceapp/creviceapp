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
        public class EvaluationAbortedException : Exception
        {
            public EvaluationAbortedException(Exception innerException) : 
                base("UserScript evaluation was aborted!", innerException)
            {
                // Todo: Display preview of the point the exception thrown on UserScript and how to debug the dll
                // generated from UserScript with ILSpy.
            }
        }

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
            using (Verbose.PrintElapsed("Parse UserScript"))
            {
                var script = CSharpScript.Create(
                    userScriptString,
                    ScriptOptions.Default
                        .WithSourceResolver(ScriptSourceResolver.Default.WithBaseDirectory(UserDirectory))
                        .WithMetadataResolver(ScriptMetadataResolver.Default.WithBaseDirectory(UserDirectory))
                        .WithReferences(
                            // mscorlib.dll
                            typeof(object).Assembly,
                            // System.dll
                            typeof(Uri).Assembly,
                            // System.Core.dll
                            typeof(Enumerable).Assembly,
                            // CreviceApp.exe
                            // Note: For the reference to CreviceApp.exe, `Assembly.GetEntryAssembly()` does not work
                            // properly in the test environment. So instead of it, we should use `typeof(Program).Assembly)` here.
                            typeof(Program).Assembly),    
                    globalsType: typeof(Core.UserScriptExecutionContext));
                return script;
            }
        }

        public UserScriptAssembly.Cache GenerateUserScriptAssemblyCache(string userScriptString, Script parsedScript)
        {
            using (Verbose.PrintElapsed("Generate UserScriptAssemblyCache"))
            {
                var compilation = parsedScript.GetCompilation();
                var peStream = new MemoryStream();
                var pdbStream = new MemoryStream();
                compilation.Emit(peStream, pdbStream);
#if DEBUG
                using (Verbose.PrintElapsed("Debug| Output UserScriptAssemblies"))
                {
                    File.WriteAllBytes((UserScriptFile + ".debug.dll"), peStream.GetBuffer());
                    File.WriteAllBytes((UserScriptFile + ".debug.pdb"), pdbStream.GetBuffer());
                }
#endif
                return UserScriptAssembly.CreateCache(userScriptString, peStream.GetBuffer(), pdbStream.GetBuffer());
            }
        }

        public System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> CompileUserScript(Script parsedScript)
        {
            using (Verbose.PrintElapsed("Compile UserScript"))
            {
                return parsedScript.Compile();
            }
        }

        public void EvaluateUserScript(Core.UserScriptExecutionContext ctx, Script parsedScript)
        {
            using (Verbose.PrintElapsed("Evaluate UserScript"))
            {
                try
                {
                    parsedScript.RunAsync(ctx).Wait();
                }
                catch (AggregateException ex)
                {
                    throw new EvaluationAbortedException(ex.InnerException);
                }
            }
        }

        public Assembly LoadUserScriptAssembly(UserScriptAssembly.Cache userScriptAssemblyCache)
        {
            using (Verbose.PrintElapsed("Load UserScriptAssembly"))
            {
                return Assembly.Load(userScriptAssemblyCache.pe, userScriptAssemblyCache.pdb);
            }
        }

        public void EvaluateUserScriptAssembly(Core.UserScriptExecutionContext ctx, Assembly userScriptAssembly)
        {
            using (Verbose.PrintElapsed("Evaluate UserScriptAssembly"))
            {
                var type = userScriptAssembly.GetType("Submission#0");
                var factory = type.GetMethod("<Factory>");
                var parameters = new object[] { new object[] { ctx, null } };
                try
                {
                    var result = factory.Invoke(null, parameters);
                    (result as Task<object>).Wait();
                }
                catch (AggregateException ex)
                {
                    throw new EvaluationAbortedException(ex.InnerException);
                }
            }
        }

        public void EvaluateUserScriptAssembly(Core.UserScriptExecutionContext ctx, UserScriptAssembly.Cache userScriptAssemblyCache)
        {
            var userScriptAssembly = LoadUserScriptAssembly(userScriptAssemblyCache);
            EvaluateUserScriptAssembly(ctx, userScriptAssembly);
        }

        public UserScriptAssembly.Cache LoadUserScriptAssemblyCache(string userScriptString)
        {
            using (Verbose.PrintElapsed("Load UserScriptAssemblyCache"))
            {
                var cacheFile = UserScriptCacheFile;
                if (!File.Exists(cacheFile))
                {
                    Verbose.Print("UserScriptCacheFile: '{0}' did not exist.", cacheFile);
                    return null;
                }
                var loadedCache = UserScriptAssembly.Load(UserScriptCacheFile);
                if (!UserScriptAssembly.IsCompatible(loadedCache, userScriptString))
                {
                    Verbose.Print("UserScriptCacheFile was discarded because the signature was not match with this application.");
                    return null;
                }
                return loadedCache;
            }
        }

        public void SaveUserScriptAssemblyCache(UserScriptAssembly.Cache userScriptAssemblyCache)
        {
            using (Verbose.PrintElapsed("Save UserScriptAssemblyCache"))
            {
                UserScriptAssembly.Save(UserScriptCacheFile, userScriptAssemblyCache);
            }
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
