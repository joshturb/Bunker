#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System;

/// <summary>
/// Contains tools used by attachment icon modification.
/// </summary>
public static class AttachmentIconUtils
{
    /// <summary>
    /// Saves provided virtual texture as a new asset.
    /// </summary>
    /// <param name="virtual2D">Texture to save</param>
    /// <returns>Texture object found in assets.</returns>
    public static Texture2D SaveIcon(Texture2D virtual2D, string assetPath, string filename)
    {
        byte[] encoded = virtual2D.EncodeToPNG();
        assetPath += "/" + filename + ".png";

        string systemPath = Application.dataPath;
        systemPath = systemPath.Remove(systemPath.LastIndexOf('/') + 1);
        systemPath += assetPath;
        systemPath = systemPath.Replace('/', Path.DirectorySeparatorChar);

        if (File.Exists(systemPath))
            File.Delete(systemPath);

        File.WriteAllBytes(systemPath, encoded);

        AssetDatabase.Refresh();

        TextureImporter typeChanger = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        typeChanger.textureType = TextureImporterType.Sprite;
        AssetDatabase.ImportAsset(assetPath);

        return AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
    }
}

#endif
