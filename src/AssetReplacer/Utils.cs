using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.Networking;
using AssetReplacer.AssetStore;
using System.Collections.Generic;
using AssetReplacer;
using BehaviorDesigner.Runtime.Tasks;
using System.Linq;

namespace AssetReplacer
{
    internal class Utils
    {
        internal static List<Material> initialisedMaterials = [];
        // Returns true on successful patch, false otherwise
        internal static Material[] materialCache = [];
        internal static void UpdateMaterialList()
        {
            materialCache = Resources.FindObjectsOfTypeAll<Material>();
        }
        internal static bool TryReplaceTexture2D(Texture2D ogTexture)
        {

            //This stops nesting bloat
            if (ogTexture is null)
                return false;

            //This will check for existence and assign all in one
            //Todo: De-nest this by returning early on !TryGetValue instead
            if (TextureStore.textureDict.TryGetValue(ogTexture.name, out var tex))
            {
                Plugin.Log.LogInfo("ogTexture.format: " + ogTexture.format);
                Plugin.Log.LogInfo("tex.format: " + tex.format);
                if (ogTexture.format != tex.format)
                {
                    Plugin.Log.LogInfo($"INFO! Remaking texture {ogTexture.name}, wants format {ogTexture.format}, have format {tex.format}");

                    //https://docs.unity3d.com/ScriptReference/Texture2D.SetPixels.html
                    List<TextureFormat> validFormats = new List<TextureFormat>(){
                                TextureFormat.Alpha8,
                                TextureFormat.ARGB32,
                                TextureFormat.ARGB4444,
                                TextureFormat.BGRA32,
                                TextureFormat.R16,
                                TextureFormat.R8,
                                TextureFormat.RFloat,
                                TextureFormat.RG16,
                                TextureFormat.RG32,
                                TextureFormat.RGB24,
                                TextureFormat.RGB48,
                                TextureFormat.RGB565,
                                TextureFormat.RGB9e5Float,
                                TextureFormat.RGBA32,
                                TextureFormat.RGBA4444,
                                TextureFormat.RGBA64,
                                TextureFormat.RGBAFloat,
                                TextureFormat.RGBAHalf,
                                TextureFormat.RGFloat,
                                TextureFormat.RGHalf,
                                TextureFormat.RHalf,
                                TextureFormat.DXT5
                            };

                    if (validFormats.Contains(ogTexture.format))
                    {
                        Texture2D newTex = new Texture2D(tex.width, tex.height, ogTexture.format, 1, false);
                        newTex.SetPixels(tex.GetPixels());
                        newTex.Apply();

                        TextureStore.textureDict[ogTexture.name] = newTex;
                        tex = newTex;

                    }
                    else
                    {
                        Plugin.Log.LogInfo("Failed to remake texture. Invalid format: " + Enum.GetName(typeof(TextureFormat), ogTexture.format));
                    }
                }
                if (tex.width == ogTexture.width && tex.height == ogTexture.height && tex.format == ogTexture.format)
                {
                    Graphics.CopyTexture(tex, ogTexture);

                    foreach (Material material in materialCache)
                    {
                        if (material.GetTexture(tex.name))
                        {
                            material.SetTexture(tex.name, tex);
                        }
                    }

                    Plugin.Log.LogInfo("OK! Replaced Texture " + ogTexture.name);
                    return true;
                }
                else
                {
                    Plugin.Log.LogError($"TEST Failed to replace texture {ogTexture.name} because of dimension or format mismatch. Original Texture: {ogTexture.width}w x {ogTexture.height}h {Enum.GetName(typeof(TextureFormat), ogTexture.format)}, Replacement Texture: {tex.width}w x {tex.height}h {Enum.GetName(typeof(TextureFormat), tex.format)}");
                }
            }
            else
            {
                Plugin.Log.LogDebug("FAIL! No Texture available for " + ogTexture.name);
            }
            return false;
        }
    }
}
