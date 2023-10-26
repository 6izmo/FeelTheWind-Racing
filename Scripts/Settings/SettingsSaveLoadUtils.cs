using Newtonsoft.Json;
using UnityEngine;
using System.IO;


public class SettingsSaveLoadUtils
{
    private const string _settingsFileName = "settingsData.json";
    private static string s_SaveFolder => Path.Combine(Application.dataPath, "Scripts", "Settings");


    public static SettingsData LoadSettingsData()
    {
        string path = Path.Combine(s_SaveFolder, _settingsFileName);

        if (!File.Exists(path))
            return null;

        string serializedData = File.ReadAllText(path);

        if (string.IsNullOrEmpty(serializedData))
            return null;

        SettingsData data = JsonConvert.DeserializeObject<SettingsData>(serializedData);

        return data;
    }

    public static void SaveSettingsData(SettingsData dataModel)
    {
        if (dataModel == null)
            return;

        string serializedObject = JsonConvert.SerializeObject(dataModel, SettingsData.SerializeSettings());
        string path = Path.Combine(s_SaveFolder, _settingsFileName);

        CreateDirectoryIfNoteExists(s_SaveFolder);

        File.WriteAllText(path, serializedObject);
    }

    public static void CreateDirectoryIfNoteExists(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
}
