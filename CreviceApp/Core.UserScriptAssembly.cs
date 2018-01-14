using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreviceApp
{

    public sealed class UserScriptAssembly
    {
        [Serializable]
        public class Cache
        {
            public string AppVersion { get; set; }
            public byte[] pe { get; set; }
            public byte[] pdb { get; set; }
            public string scriptFileHash { get; set; }
        }

        public class UserScriptAssemblyImpl
        {
            System.Security.Cryptography.MD5 md5;
            BinaryFormatter formatter;

            public UserScriptAssemblyImpl()
            {
                this.md5 = System.Security.Cryptography.MD5.Create();
                this.formatter = new BinaryFormatter();
            }

            public string CurrentAppVersion()
            {
                return string.Format("{0}-{1}", Application.ProductName, Application.ProductVersion);
            }

            private string Hash(string str)
            {
                var bytes = Encoding.UTF8.GetBytes(str);
                bytes = md5.ComputeHash(bytes);
                return BitConverter.ToString(bytes).Replace("-", string.Empty);
            }

            public bool IsCompatible(Cache cache, string userScriptCode)
            {
                return cache.AppVersion == CurrentAppVersion() &&
                       cache.scriptFileHash == Hash(userScriptCode);
            }

            public Cache CreateCache(string script, byte[] pe, byte[] pdb)
            {
                var cache = new Cache()
                {
                    AppVersion = CurrentAppVersion(),
                    pe = pe,
                    pdb = pdb,
                    scriptFileHash = Hash(script)
                };
                return cache;
            }

            public void Save(string path, Cache cache)
            {
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    formatter.Serialize(fs, cache);
                }
            }

            public Cache Load(string path)
            {
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    var data = formatter.Deserialize(fs);
                    return data as Cache;
                }
            }
        }

        private static UserScriptAssemblyImpl _singletonInstance = new UserScriptAssemblyImpl();
        private static UserScriptAssemblyImpl GetInstance()
        {
            return _singletonInstance;
        }
        private UserScriptAssembly() {}

        public static string CurrentAppVersion()
        {
            return GetInstance().CurrentAppVersion();
        }

        public static bool IsCompatible(Cache cache, string userScriptCode)
        {
            return GetInstance().IsCompatible(cache, userScriptCode);
        }

        public static Cache CreateCache(string userScriptCode, byte[] pe, byte[] pdb)
        {
            return GetInstance().CreateCache(userScriptCode, pe, pdb);
        }

        public static void Save(string path, Cache cache)
        {
            GetInstance().Save(path, cache);
        }

        public static Cache Load(string path)
        {
            return GetInstance().Load(path);
        }
    }
}
