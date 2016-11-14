using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FolderMonitor.Common.Plugin
{
    public class PluginManager
    {
        public static List<IFileLoader> LoadEnabledFileLoaders(string directoryName, string allowedPlugins)
        {
            List<IFileLoader> loaders = new List<IFileLoader>();

            foreach (string fileFound in Directory.GetFiles(directoryName))
            {
                FileInfo file = new FileInfo(fileFound);

                if (file.Extension.Equals(".dll"))
                {
                    IFileLoader newPlugin = CreatePlugin(fileFound);
                    if (newPlugin != null && allowedPlugins.Contains( newPlugin.Name))
                        loaders.Add(newPlugin);
                }
            }


            return loaders;
        }

        private static IFileLoader CreatePlugin(string FileName)
        {
            Assembly pluginAssembly = Assembly.LoadFrom(FileName);
            IFileLoader newPlugin = null;

            foreach (Type pluginType in pluginAssembly.GetTypes())
            {
                if (pluginType.IsPublic)
                {
                    if (!pluginType.IsAbstract)
                    {
                        Type typeInterface = pluginType.GetInterface("IFileLoader", true);

                        if (typeInterface != null)
                        {
                            newPlugin = (IFileLoader)Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString()));

                        }

                        typeInterface = null;
                    }
                }
            }
            return newPlugin;
        }
    }
}
