using BepInEx;
using BepInEx.Logging;
using AssetReplacer.Settings;
using HarmonyLib;
using UnityEngine.SceneManagement;
using UnityEngine;
using AssetReplacer.AssetStore;
using BepInEx.Configuration;
using System.Reflection;
using System.IO;

namespace AssetReplacer;


[BepInPlugin(LCMPluginInfo.PLUGIN_GUID, LCMPluginInfo.PLUGIN_NAME, LCMPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log = null!;
    internal static AssetReplacerSettings Settings = null!;

    private void Awake()
    {
        Log = base.Logger;

        Log.LogInfo($"Plugin {LCMPluginInfo.PLUGIN_NAME} version {LCMPluginInfo.PLUGIN_VERSION} is loaded!");
    }

    private void Start()
    {
        /*
        Changes: Path resolution for texture dir
        Explanation: 
            The original code simply deleted the last 17 characters from the executing assembly's path
            so I assumed you wanted the current directory of the current assembly
            Info.Location always has your plugin's location so it'll work no matter how the user
            installs it.
        */
        string texturesFolderPath = Path.GetFullPath(Path.Combine(Info.Location.Replace(Path.GetFileName(Info.Location), ""), "Texture2D"));

        this.patchAssets(texturesFolderPath);
    }

    private void patchAssets(string path)
    {

        Log.LogInfo("Configured Texture Mod Directory: " + path);
        FileLoader.TextureModFolders.Add(path);

        TextureStore.Init();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {

        Texture2D[] textures = Resources.FindObjectsOfTypeAll<Texture2D>();

        foreach (Texture2D texture in textures)
        {
            Utils.TryReplaceTexture2D(texture);
        }
    }
}
