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

    private string _profileName1 = "Profile_1";
    private string _profileName2 = "Profile_2";

    [Test]
    public void Step1_SaveLoadData()
    {
        PlayerPrefsUtility.Save(jsonData, _profileName1, true);

        string jsonDataLoaded = PlayerPrefsUtility.Load(_profileName1, true);

        Assert.AreEqual(@jsonData, jsonDataLoaded); // json loaded equal json default
    }

    [Test]
    public void Step2_TestHasKey()
    {
        Assert.AreEqual(true, PlayerData.HasKey("testInt", _profileName1));
        Assert.AreEqual(false, PlayerData.HasKey("wrongKey", _profileName1));
        Assert.AreEqual(false, PlayerData.HasKey("testInt", "wrongProfile"));
    }

    [Test]
    public void Step3_TestGetAndSet()
    {
        Assert.AreEqual(18, PlayerData.GetInt("testInt", _profileName1));
        Assert.AreEqual(8.9999f, PlayerData.GetFloat("testFloat", _profileName1));
        Assert.AreEqual("Jonh", PlayerData.GetString("testString", _profileName1));

        PlayerData.SetInt("testInt", 200, _profileName1);
        PlayerData.SetString("testString", "StringToTest", _profileName1);
        PlayerData.Save(_profileName1);

        Assert.AreEqual("StringToTest", PlayerData.GetString("testString", _profileName1));
        Assert.AreEqual(200, PlayerData.GetInt("testInt", _profileName1));
    }

    [Test]
    public void Step4_Add1000key()
    {
        for (int i = 0; i < 1000; i++)
        {
            PlayerData.SetInt("key_" + i, i, _profileName1);
        }

        Assert.AreEqual(true, PlayerData.Save(_profileName1));
        Assert.AreEqual(true, PlayerData.HasKey("key_100", _profileName1));
    }

    [Test]
    public void Step5_Remove1000key()
    {
        for (int i = 0; i < 1000; i++)
        {
            PlayerData.DeleteKey("key_" + i, _profileName1);
        }

        Assert.AreEqual(true, PlayerData.Save(_profileName1));

        Assert.AreEqual(false, PlayerData.HasKey("key_100", _profileName1)); 
    }

    [Test]
    public void Step6_AddAndRemove1000key()
    {
        for(int i = 0; i < 1000; i++)
        {
            PlayerData.SetInt("key_" + i, i, _profileName2);
        }

        Assert.AreEqual(true, PlayerData.Save(_profileName2));

        for (int i = 0; i < 1000; i++)
        {
            PlayerData.DeleteKey("key_" + i, _profileName2);
        }

        Assert.AreEqual(false, PlayerData.Save(_profileName2));
    }


    [Test]
    public void Step7_TestDelete()
    {
        PlayerData.DeleteKey("testLongString", _profileName1);
        PlayerData.Save(_profileName1);

        Assert.AreEqual(false, PlayerData.HasKey("testLongString", _profileName1));

        PlayerData.DeleteAll(_profileName1);

        Assert.AreEqual("", PlayerPrefsUtility.Load(_profileName1, true));

        PlayerData.DeleteAll(_profileName2);
        PlayerData.DeleteAll("unknowProfile");
    }
}
