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


namespace Crevice.UserScript
{
    using Crevice.Logging;

    public static class UserScript
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

        public class DirectoryWatcher : FileSystemWatcher
        {
            public DirectoryWatcher(
                ISynchronizeInvoke synchronizingObject, 
                string targetDirectory, 
                string filter, 
                NotifyFilters notifyFilters = NotifyFilters.FileName | NotifyFilters.LastWrite,
                bool watchSubdirectries = true)
            {
                this.Path = targetDirectory;
                this.Filter = filter;
                this.NotifyFilter = notifyFilters;
                this.IncludeSubdirectories = watchSubdirectries;
                this.SynchronizingObject = synchronizingObject;
            }
        }

        public class FileWatcher : FileSystemWatcher
        {
            public FileWatcher(
                ISynchronizeInvoke synchronizingObject,
                string targetFile,
                NotifyFilters notifyFilters = NotifyFilters.FileName | NotifyFilters.LastWrite
                )
            {
                this.Path = System.IO.Path.GetDirectoryName(targetFile);
                this.Filter = System.IO.Path.GetFileName(targetFile);
                this.NotifyFilter = notifyFilters;
                this.IncludeSubdirectories = false;
                this.SynchronizingObject = synchronizingObject;
            }
        }

        public static Script ParseScript(
            string userScriptString,
            string scriptSourceResolverBaseDirectory,
            string scriptMetadataResolverBaseDirectory
            )
        {
            using (Verbose.PrintElapsed("Parse UserScript"))
            {
                var script = CSharpScript.Create(
                    userScriptString,
                    ScriptOptions.Default
                        .WithSourceResolver(ScriptSourceResolver.Default.WithBaseDirectory(scriptSourceResolverBaseDirectory))
                        .WithMetadataResolver(ScriptMetadataResolver.Default.WithBaseDirectory(scriptMetadataResolverBaseDirectory))
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
                    globalsType: typeof(UserScriptExecutionContext));
                return script;
            }
        }

        public static UserScriptAssembly.Cache GenerateUserScriptAssemblyCache(string userScriptString, Script parsedScript)
        {
            using (Verbose.PrintElapsed("Generate UserScriptAssemblyCache"))
            {
                var compilation = parsedScript.GetCompilation();
                var peStream = new MemoryStream();
                var pdbStream = new MemoryStream();
                compilation.Emit(peStream, pdbStream);
                /*
#if DEBUG
                using (Verbose.PrintElapsed("Debug| Output UserScriptAssemblies"))
                {
                    File.WriteAllBytes((UserScriptFile + ".debug.dll"), peStream.GetBuffer());
                    File.WriteAllBytes((UserScriptFile + ".debug.pdb"), pdbStream.GetBuffer());
                }
#endif
*/
                return UserScriptAssembly.CreateCache(userScriptString, peStream.GetBuffer(), pdbStream.GetBuffer());
            }
        }

        public static System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> CompileUserScript(Script parsedScript)
        {
            using (Verbose.PrintElapsed("Compile UserScript"))
            {
                return parsedScript.Compile();
            }
        }

        public static void EvaluateUserScript(UserScriptExecutionContext ctx, Script parsedScript)
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

        public static Assembly LoadUserScriptAssembly(UserScriptAssembly.Cache userScriptAssemblyCache)
        {
            using (Verbose.PrintElapsed("Load UserScriptAssembly"))
            {
                return Assembly.Load(userScriptAssemblyCache.PE, userScriptAssemblyCache.PDB);
            }
        }

        public static void EvaluateUserScriptAssembly(UserScriptExecutionContext ctx, Assembly userScriptAssembly)
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

        public static void EvaluateUserScriptAssembly(UserScriptExecutionContext ctx, UserScriptAssembly.Cache userScriptAssemblyCache)
        {
            var userScriptAssembly = LoadUserScriptAssembly(userScriptAssemblyCache);
            EvaluateUserScriptAssembly(ctx, userScriptAssembly);
        }

        public static UserScriptAssembly.Cache LoadUserScriptAssemblyCache(string cachePath, string userScriptString)
        {
            using (Verbose.PrintElapsed("Load UserScriptAssemblyCache"))
            {
                if (!File.Exists(cachePath))
                {
                    Verbose.Print("UserScriptCacheFile: '{0}' did not exist.", cachePath);
                    return null;
                }
                var loadedCache = UserScriptAssembly.Load(cachePath);
                if (!UserScriptAssembly.IsCompatible(loadedCache, userScriptString))
                {
                    Verbose.Print("UserScriptCacheFile was discarded because the signature was not match with this application.");
                    return null;
                }
                return loadedCache;
            }
        }

        public static void SaveUserScriptAssemblyCache(string cachePath, UserScriptAssembly.Cache userScriptAssemblyCache)
        {
            using (Verbose.PrintElapsed("Save UserScriptAssemblyCache"))
            {
                UserScriptAssembly.Save(cachePath, userScriptAssemblyCache);
            }
        }
        
        private static string GetHighlightedUserScriptString(Microsoft.CodeAnalysis.Diagnostic error)
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

        public static string GetPrettyErrorMessage(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> errors)
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
