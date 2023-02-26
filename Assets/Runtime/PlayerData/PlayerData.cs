using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private const string FOLDER_NAME = "";

    /// <summary>
    /// Save user-profile
    /// </summary>
    /// <returns></returns>
    public static bool Save(string jsonData, string profileName)
    {
        string path = GetFilePath(profileName, FOLDER_NAME);

        Byte[] byteData = Encoding.ASCII.GetBytes(@jsonData);
        
        // attempt to save here data
        try
        {
            // save datahere
            File.WriteAllBytes(path, byteData);
            Debug.Log("<color=green>Save data to: </color>" + path);
           
        }
        catch (Exception e)
        {
            // write out error here
            Debug.LogError("Failed to save data to: " + path);
            Debug.LogError("Error " + e.Message);
        }
        return true;
    }

    public static string Load(string profileName)
    {
        string path = GetFilePath(profileName, FOLDER_NAME);

        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Debug.LogWarning("File or path does not exist! " + path);
            return "";
        }

        // load in the save data as byte array
        byte[] jsonDataAsBytes = null;

        try
        {
            jsonDataAsBytes = File.ReadAllBytes(path);
            Debug.Log("<color=green>Loaded all data from: </color>" + path);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to load data from: " + path);
            Debug.LogWarning("Error: " + e.Message);
            return "";
        }

        if (jsonDataAsBytes == null)
            return "";

        // convert the byte array to json
        string jsonData = Encoding.ASCII.GetString(jsonDataAsBytes);
        Debug.Log("Json Data: \n" + jsonData);
        return jsonData;
    }

    public static string GetString(string key, string profileName)
    {
        if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
        {
            Debug.LogWarning("Invalid key!");
            return "";
        }
        string jsonData = Load(profileName);

        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

        return (string)data[key];
    }

    public static bool SetString(string key, string value, string profileName)
    {
        string jsonData = Load(profileName);

        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

        data[key] = value;

        Save(JsonConvert.SerializeObject(data), profileName);

        return true;
    }

    /// <summary>
    /// Create file path for where a file is stored on the specific platform given a folder name and file name
    /// </summary>
    /// <param name="FileName"></param>
    /// <param name="FolderName"></param>
    /// <returns></returns>
    private static string GetFilePath(string FileName = "user.dat", string FolderName = "")
    { 
        string path = Path.Combine(Application.persistentDataPath, FolderName);
        path = Path.Combine(path, FileName);

        //Create Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        return path;
    }

    private static string GetValueByKey(string key, string jsonData)
    {
        jsonData = jsonData.Replace("{", "").Replace("}", ""); // remove {, }

        string[] records = jsonData.Split(",");
        
        for (int i = 0; i < records.Length; i++)
        {
            string[] valueData = records[i].Split("\"" + key + "\"");
            int splitPosition = records[i].IndexOf(":");
            if (splitPosition > 0)
            {
                string k = records[i].Substring(0, splitPosition).Trim();

                if (k.Replace("\"", "") == key)
                {
                    string v = records[i].Substring(splitPosition + 1);
                    return v.Replace("\"", "").Trim();
                }
            }
        }

        return "";
    }
}
