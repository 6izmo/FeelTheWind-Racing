using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class MapSaveLoadUtils 
{
    private const string _windowFileName = "minimapWindowData.json";

    public const string _screenshotsFileName = "map_screen_*.png";

    private static string s_SaveFolder => Path.Combine(Application.dataPath, "Sources", "Minimap", "SaveData"); 

    public static MinimapWindowDataModel LoadWindowData()
    {
        string path = Path.Combine(s_SaveFolder, _windowFileName);

        if (!File.Exists(path))
            return null;

        string serializedData = File.ReadAllText(path);

        if (string.IsNullOrEmpty(serializedData))
            return null;
        
        MinimapWindowDataModel data = JsonConvert.DeserializeObject<MinimapWindowDataModel>(serializedData);

        return data;
    }

    public static void SaveWindowData(MinimapWindowDataModel dataModel)
    {
        if (dataModel == null)
            return;

        string serializedObject = JsonConvert.SerializeObject(dataModel, MinimapWindowDataModel.SerializeSettings());

        string path = Path.Combine(s_SaveFolder, _windowFileName);
        CreateDirectoryIfNoteExists(s_SaveFolder);

        File.WriteAllText(path, serializedObject);

        Debug.LogWarning($"Save window data with path {path}");
    }

    public static void CreateDirectoryIfNoteExists(string path)
    {
        if(!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
    
    public static void SaveTexture(Texture2D screen, int i)
    {
        CreateDirectoryIfNoteExists(s_SaveFolder);
        string path = Path.Combine(s_SaveFolder, _screenshotsFileName.Replace("*", i.ToString()));

        byte[] textureBytes = screen.EncodeToPNG();
        File.WriteAllBytes(path, textureBytes);

        Debug.LogWarning($"Save texture at path {path}");
    }
    public static Texture2D[] LoadAllMapTextures()
    {
        string path = Path.Combine(s_SaveFolder, _screenshotsFileName);
        string relativePath = path.Substring(path.IndexOf("Assets", StringComparison.Ordinal));

        List<Texture2D> textures = new List<Texture2D>();
        int counter = 0;

        string texturePath = relativePath.Replace("*", counter.ToString());

        while (File.Exists(texturePath))
        {
            string localPath = texturePath.Substring(texturePath.IndexOf("Assets", StringComparison.Ordinal));

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(localPath);

            if (texture)
                textures.Add(texture);

            counter++;
            texturePath = relativePath.Replace("*", counter.ToString());
        }

        return textures.ToArray();
    }

    public static void ClearDirectory(string directory)
    {
        if (Directory.Exists(directory))
            Directory.Delete(directory, true);

        CreateDirectoryIfNoteExists(directory);
    }
}
