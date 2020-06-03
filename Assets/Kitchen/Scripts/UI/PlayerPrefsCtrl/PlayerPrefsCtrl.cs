using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsCtrl : BSingleton<PlayerPrefsCtrl>
{
    public float GetFloatValue (string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetFloat(key);
        }
        return 0.0f;
    }

    public bool GetBoolValue(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            var value = PlayerPrefs.GetInt(key);

            return value == 1;
        }
        return false;
    }
}
