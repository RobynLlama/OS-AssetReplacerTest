using BehaviorDesigner.Runtime.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Experimental.Rendering;
using UnityEngine;
using AssetReplacer.AssetStore;


namespace AssetReplacer
{
    internal static class FileLoader
    {
        public static List<string> TextureModFolders = new List<string>();
        public static List<string> AudioModFolders = new List<string>();

        internal static void LoadTextures()
        {
            foreach (string path in TextureModFolders)
            {
                string textureDir = path;
                try
                {
                    Plugin.Log.LogInfo("textureDir: " + textureDir);
                    foreach (string filepath in Directory.EnumerateFiles(textureDir, "*.*", SearchOption.AllDirectories))
                    {
                        Plugin.Log.LogInfo("filepath: " + filepath);
                        Plugin.Log.LogDebug("Found file " + Path.GetFileNameWithoutExtension(filepath) + " at " + filepath.Replace(textureDir + "\\", ".\\"));
                        Texture2D texture2D = new Texture2D(2, 2, GraphicsFormat.R8G8B8A8_UNorm, 1, TextureCreationFlags.None);
                        texture2D.LoadImage(File.ReadAllBytes(filepath));
                        TextureStore.textureDict[Path.GetFileNameWithoutExtension(filepath)] = texture2D;
                    }
                }
                catch (Exception e)
                {
                    Plugin.Log.LogError("Error loading Textures. Please make sure you configured the mod folders correctly and it doesn't contain any unrelated files.");
                    Plugin.Log.LogError(e.GetType() + " " + e.Message);
                }
            }
            Plugin.Log.LogInfo("Textures loaded successfully.");
        }

        private static string getAssetDir(string modName, string assetType)
        {

           return Path.Combine(BepInEx.Paths.PluginPath, modName, assetType);
        }
    }
}