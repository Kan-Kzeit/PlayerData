using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class PlayerData
{
    private const string FOLDER_NAME = "";

    //local store profile loaded
    private static Dictionary<string, string> _localStoreData = new Dictionary<string, string>();

    public static bool Save(string jsonData, string profileName)
    {
        string path = GetFilePath(profileName, FOLDER_NAME);

        //Encrypt data
        AESEncryption.AESEncryptedText aes = AESEncryption.Encrypt(jsonData, AESEncryption.PASSWORD);
        jsonData = aes.EncryptedText + AESEncryption.IV_SYMBOL + aes.IV;

        Byte[] byteData = Encoding.ASCII.GetBytes(jsonData);
        
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

    //Save by local store data
    public static bool Save(string profileName)
    {
        if (!_localStoreData.ContainsKey(profileName)) return false;

        Save(_localStoreData[profileName], profileName);

        return true;
    }

    public static string Load(string profileName)
    {
        if (_localStoreData.ContainsKey(profileName)) return _localStoreData[profileName];

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
        string[] decrypt = jsonData.Split(AESEncryption.IV_SYMBOL);

        string decryptJsonData = AESEncryption.Decrypt(decrypt[0], decrypt[1], AESEncryption.PASSWORD);

        _localStoreData.Add(profileName, decryptJsonData);

        Debug.Log("Json Data: \n" + decryptJsonData);
        return decryptJsonData;
    }

    public static void DeleteAll(string profileName)
    {
        string path = GetFilePath(profileName, FOLDER_NAME);
        File.Delete(path);

        _localStoreData.Remove(profileName);
    }

    public static void DeleteKey(string key, string profileName)
    {
        string jsonData = Load(profileName);

        if (string.IsNullOrEmpty(jsonData)) return;

        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

        if (!data.Keys.Contains(key)) return;

        data.Remove(key);
        if (data.Keys.Count == 0)
        {
            DeleteAll(profileName);
            return;
        }

        _localStoreData[profileName] = JsonConvert.SerializeObject(data);
    }

    public static bool HasKey(string key, string profileName)
    {
        string jsonData = Load(profileName);

        if (string.IsNullOrEmpty(jsonData)) return false;

        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

        if (!data.Keys.Contains(key)) return false;

        return true;
    }

    public static float GetFloat(string key, string profileName, float defaultValueReturn = 0f)
    {
        return float.Parse(GetValue(key, profileName, defaultValueReturn));
    }

    public static void SetFloat(string key, float value, string profileName)
    {
        SetValue(key, value, profileName);
    }

    public static int GetInt(string key, string profileName, int defaultValueReturn = 0)
    {
        return int.Parse(GetValue(key, profileName, defaultValueReturn));
    }

    public static void SetInt(string key, int value, string profileName)
    {
        SetValue(key, value, profileName);
    }

    public static string GetString(string key, string profileName, string defaultValueReturn = "")
    {
        return GetValue(key, profileName, defaultValueReturn);
    }

    public static void SetString(string key, string value, string profileName)
    {
        SetValue(key, value, profileName);
    }

    private static void SetValue(string key, object value, string profileName)
    {
        if (string.IsNullOrEmpty(profileName) || string.IsNullOrWhiteSpace(profileName))
        {
            Debug.LogWarning("Invalid Profile Name!");
            return;
        }

        string jsonData = Load(profileName);

        Dictionary<string, object> data;

        if (string.IsNullOrEmpty(jsonData))
        {
            //create new profile
            data = new Dictionary<string, object>();
            data.Add(key, value);
            _localStoreData.Add(profileName, JsonConvert.SerializeObject(data));
        }
        else
        {
            data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

            if (!data.Keys.Contains(key))
            {
                data.Add(key, value);//new
            }
            else
            {
                data[key] = value;//update
            };

            _localStoreData[profileName] = JsonConvert.SerializeObject(data);
        }
    }

    private static string GetValue(string key, string profileName, object defaultValueReturn)
    {
        string jsonData = Load(profileName);

        if (string.IsNullOrEmpty(jsonData)) return defaultValueReturn.ToString();

        var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);

        if (!data.Keys.Contains(key)) return defaultValueReturn.ToString();

        return data[key];
    }

    private static string GetFilePath(string FileName = "user", string FolderName = "")
    { 
        string path = Path.Combine(Application.persistentDataPath, FolderName);
        path = Path.Combine(path, FileName + ".dat");

        //Create Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        return path;
    }
}
