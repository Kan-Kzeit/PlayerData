using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(UserProfile))]
public class PlayerPrefsEditor : Editor
{
    private Dictionary<string, string> _profileDetail;
    private UserProfile _userTarget;

    void OnEnable()
    {
        _userTarget = (UserProfile)target;
        string profileData = PlayerPrefsUtility.Load(_userTarget.name);

        _profileDetail = JsonConvert.DeserializeObject<Dictionary<string, string>>(profileData);
    }
    public override void OnInspectorGUI()
    {
        if (_profileDetail != null)
        {

            Dictionary<string, string> localStoreProfile = new Dictionary<string, string>();
            foreach (var data in _profileDetail)
            {
                localStoreProfile.Add(data.Key, EditorGUILayout.TextField(data.Key, data.Value));
            }

            //local to profile to update in next call
            foreach (var data in localStoreProfile)
            {
                _profileDetail[data.Key] = data.Value;
            }

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Delete"))
            {
                bool isClear = PlayerPrefsUtility.Clear(_userTarget.name);
                if (isClear) _profileDetail = null;
            }

            if (GUILayout.Button("Update"))
            {
                string output = "{\n";

                foreach (var data in _profileDetail)
                {
                    output += "\"" + data.Key + "\"" + " : " + "\"" + data.Value + "\"" + ",\n";
                }
                output += "}";

                PlayerPrefsUtility.Save(output, _userTarget.name);
            }

            if (GUILayout.Button("Print String Json"))
            {
                string output = "{\n";

                foreach (var data in _profileDetail)
                {
                    output += "\"" + data.Key + "\"" + " : " + "\"" + data.Value + "\"" + ",\n";
                }
                output += "}";

                Debug.Log(output);
            }

            GUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.LabelField("Don't found user-profile in PlayerPrefs.");
        }
    }

}
#endif