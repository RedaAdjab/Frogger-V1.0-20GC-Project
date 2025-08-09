using UnityEngine;
using System;

public static class GenericPlayerPrefs
{
    public static void Save<T>(string key, T value)
    {
        Type type = typeof(T);

        if (type == typeof(int))
        {
            PlayerPrefs.SetInt(key, Convert.ToInt32(value));
        }
        else if (type == typeof(float))
        {
            PlayerPrefs.SetFloat(key, Convert.ToSingle(value));
        }
        else if (type == typeof(string))
        {
            PlayerPrefs.SetString(key, value.ToString());
        }
        else if (type == typeof(bool))
        {
            PlayerPrefs.SetInt(key, ((bool)(object)value) ? 1 : 0);
        }
        else
        {
            string json = JsonUtility.ToJson(value);
            PlayerPrefs.SetString(key, json);
        }

        PlayerPrefs.Save();
    }

    public static T Load<T>(string key, T defaultValue = default)
    {
        Type type = typeof(T);

        if (!PlayerPrefs.HasKey(key))
            return defaultValue;

        if (type == typeof(int))
        {
            return (T)(object)PlayerPrefs.GetInt(key);
        }
        else if (type == typeof(float))
        {
            return (T)(object)PlayerPrefs.GetFloat(key);
        }
        else if (type == typeof(string))
        {
            return (T)(object)PlayerPrefs.GetString(key);
        }
        else if (type == typeof(bool))
        {
            return (T)(object)(PlayerPrefs.GetInt(key) == 1);
        }
        else
        {
            string json = PlayerPrefs.GetString(key);
            try
            {
                return JsonUtility.FromJson<T>(json);
            }
            catch
            {
                return defaultValue;
            }
        }
    }

    public static bool HasKey(string key) => PlayerPrefs.HasKey(key);
    public static void DeleteKey(string key) => PlayerPrefs.DeleteKey(key);
    public static void DeleteAll() => PlayerPrefs.DeleteAll();
    public static void SaveChanges() => PlayerPrefs.Save();
}
