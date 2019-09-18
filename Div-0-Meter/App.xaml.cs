using System;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.IO;
using System.Text;

namespace Div_0_Meter
{

    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
        }

        public static string Crypt(string text, string key) //simple, but enough for bonus thingy
        {
            char[] arr1 = text.ToCharArray();
            Array.Reverse(arr1);
            byte[] txt = Encoding.Default.GetBytes(new string(arr1));
            byte[] ckey = Encoding.Default.GetBytes(key);
            byte[] res = new byte[text.Length];
            for (int i = 0; i < txt.Length; i++)
                res[i] = (byte)(txt[i] ^ key[i % key.Length]);
            char[] arr2 = Encoding.Default.GetString(res).ToCharArray();
            Array.Reverse(arr2);
            return new string(arr2);
        }

        static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            var name = args.Name.Substring(0, args.Name.IndexOf(',')) + ".dll";
            var resourceName = thisAssembly.GetManifestResourceNames().First(s => s.EndsWith(name));

            using (Stream stream = thisAssembly.GetManifestResourceStream(resourceName))
            {
                byte[] block = new byte[stream.Length];
                stream.Read(block, 0, block.Length);
                return Assembly.Load(block);
            }
        }
    }
}
