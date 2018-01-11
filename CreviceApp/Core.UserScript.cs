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
        public class CompilationError
        {
            public readonly string Id;
            public readonly string Category;
            public readonly string Message;
            public readonly string HelpLinkUri;
            public readonly Microsoft.CodeAnalysis.Text.TextSpan SourceSpan;

            public CompilationError(
                string Id, 
                string Category, 
                string Message, 
                string HelpLinkUri,
                Microsoft.CodeAnalysis.Text.TextSpan SourceSpan)
            {
                this.Id = Id;
                this.Category = Category;
                this.Message = Message;
                this.HelpLinkUri = HelpLinkUri;
                this.SourceSpan = SourceSpan;
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

        public string GetUserScriptCode()
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

        public Script ParseScript(string userScriptCode)
        {
            var stopwatch = new Stopwatch();
            Verbose.Print("Parsing UserScript...");
            stopwatch.Start();
            var script = CSharpScript.Create(
                userScriptCode,
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

        public UserScriptAssembly.Cache CompileUserScript(string userScriptCode, Script parsedScript)
        {
            var stopwatch = new Stopwatch();
            Verbose.Print("Compiling UserScript...");
            stopwatch.Start();
            var compilation = parsedScript.GetCompilation();
            stopwatch.Stop();
            Verbose.Print("UserScript compilation finished. ({0})", stopwatch.Elapsed);

            var peStream = new MemoryStream();
            var pdbStream = new MemoryStream();
            Verbose.Print("Genarating UserScriptAssembly...");
            stopwatch.Restart();
            compilation.Emit(peStream, pdbStream);
            stopwatch.Stop();
            Verbose.Print("UserScriptAssembly generation finished. ({0})", stopwatch.Elapsed);
            return UserScriptAssembly.CreateCache(userScriptCode, peStream.GetBuffer(), pdbStream.GetBuffer());
        }

        public IEnumerable<CompilationError> CompileUserScript(Script parsedScript)
        {
            var stopwatch = new Stopwatch();
            Verbose.Print("Compiling UserScript...");
            stopwatch.Start();
            var diagnostic = parsedScript.Compile();
            stopwatch.Stop();
            Verbose.Print("UserScript compilation finished. ({0})", stopwatch.Elapsed);
            var errors = from d in diagnostic
                         select new CompilationError(
                                    d.Id,
                                    d.Descriptor.Category,
                                    d.GetMessage(),
                                    d.Descriptor.HelpLinkUri,
                                    d.Location.SourceSpan);
            return errors;
        }

        public IEnumerable<Core.GestureDefinition> EvaluateUserScript(Script parsedScript, Core.UserScriptExecutionContext ctx)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            parsedScript.RunAsync(ctx).Wait();
            stopwatch.Stop();
            Verbose.Print("UserScript evaluation finished. ({0})", stopwatch.Elapsed);
            return ctx.GetGestureDefinition();
        }

        public IEnumerable<Core.GestureDefinition> EvaluateUserScriptAssembly(UserScriptAssembly.Cache cache, Core.UserScriptExecutionContext ctx)
        {
            var stopwatch = new Stopwatch();
            Verbose.Print("Loading UserScriptAssembly...");
            stopwatch.Start();
            var assembly = Assembly.Load(cache.pe, cache.pdb);
            stopwatch.Stop();
            Verbose.Print("UserScriptAssembly loading finished. ({0})", stopwatch.Elapsed);
            Verbose.Print("Evaluating UserScriptAssembly...");
            stopwatch.Restart();
            var type = assembly.GetType("Submission#0");
            var factory = type.GetMethod("<Factory>");
            var parameters = new object[] { new object[] { ctx, null } };
            var result = factory.Invoke(null, parameters);
            stopwatch.Stop();
            Verbose.Print("UserScriptAssembly evaluation finished. ({0})", stopwatch.Elapsed);
            return ctx.GetGestureDefinition();
        }

        public UserScriptAssembly.Cache GetUserScriptAssemblyCache(string userScriptCode, Script parsedScript)
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
                if (UserScriptAssembly.IsCompatible(loadedCache, userScriptCode))
                {
                    return loadedCache;
                }
                Verbose.Print("UserScriptAssemblyCache was discarded because of the signature was not match with this application.");
            }
            var generatedCache = CompileUserScript(userScriptCode, parsedScript);
            Verbose.Print("Saving UserScriptAssemblyCache...");
            stopwatch.Restart();
            UserScriptAssembly.Save(cacheFile, generatedCache);
            stopwatch.Stop();
            Verbose.Print("UserScriptAssemblyCache saving finished. ({0})", stopwatch.Elapsed);
            return generatedCache;
        }

        public IEnumerable<Core.GestureDefinition> GetGestureDef(Core.UserScriptExecutionContext ctx)
        {
            var userScriptCode = GetUserScriptCode();
            var parsedScript = ParseScript(userScriptCode);
            if (!Global.CLIOption.NoCache)
            {
                try
                {
                    var cache = GetUserScriptAssemblyCache(userScriptCode, parsedScript);
                    return EvaluateUserScriptAssembly(cache, ctx);
                }
                catch (Exception ex)
                {
                    Verbose.Print("Error occured when UserScript conpilation and save and restoration; fallback to the --nocache mode and continume");
                    Verbose.Print(ex.ToString());
                }
            }
            CompileUserScript(parsedScript);
            return EvaluateUserScript(parsedScript, ctx);
        }
    }
}
