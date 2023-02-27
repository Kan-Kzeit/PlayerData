using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerDataTest
{
    private string jsonData = @"
        {
            ""testString"": ""Jonh"",
            ""testInt"": 18,
            ""testFloat"": 8.9999,
            ""testLongString"": ""A hero is someone who gives of himself, often putting his
                                    own life at great risk, for the greater good of others.
                                    However, such as in war situations""
        }";

    private string _profileName = "Profile_1";

    [Test]
    public void Step1_SaveLoadData()
    {
        PlayerPrefsUtility.Save(jsonData, _profileName, true);

        string jsonDataLoaded = PlayerPrefsUtility.Load(_profileName, true);

        Assert.AreEqual(jsonData, jsonDataLoaded); // json loaded equal json default
    }

    [Test]
    public void Step2_TestHasKey()
    {
        Assert.AreEqual(true, PlayerData.HasKey("testInt", _profileName));
        Assert.AreEqual(false, PlayerData.HasKey("wrongKey", _profileName));
        Assert.AreEqual(false, PlayerData.HasKey("testInt", "wrongProfile"));
    }

    [Test]
    public void Step3_TestGetAndSet()
    {
        Assert.AreEqual(18, PlayerData.GetInt("testInt", _profileName));
        Assert.AreEqual(8.9999f, PlayerData.GetFloat("testFloat", _profileName));
        Assert.AreEqual("Jonh", PlayerData.GetString("testString", _profileName));

        PlayerData.SetInt("testInt", 200, _profileName);
        PlayerData.SetString("testString", "StringToTest", _profileName);

        Assert.AreEqual("StringToTest", PlayerData.GetString("testString", _profileName));
        Assert.AreEqual(200, PlayerData.GetInt("testInt", _profileName));
    }

    [Test]
    public void Step4_TestDelete()
    {
        PlayerData.DeleteKey("testLongString", _profileName);
        Assert.AreEqual(false, PlayerData.HasKey("testLongString", _profileName));

        PlayerData.DeleteAll(_profileName);

        Assert.AreEqual("", PlayerPrefsUtility.Load(_profileName, true));
    }
}
