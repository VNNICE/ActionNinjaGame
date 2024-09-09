using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver 
{
    [SerializeField] private List<SerializableKeyValuePair<TKey, TValue>> clearData = new();
    public void OnBeforeSerialize() 
    {
        clearData.Clear();
        using var e = GetEnumerator();
        while (e.MoveNext()) 
        {
            clearData.Add(new SerializableKeyValuePair<TKey, TValue>(e.Current.Key, e.Current.Value));
        }
    }

    public void OnAfterDeserialize() 
    {
        Clear();
        foreach (var pair in clearData) 
        {
            this[pair.Key] = pair.Value;
        }
    }
}*/

[System.Serializable]
public class KeyValuePair<TKey, TValue>
{
    public TKey Key;
    public TValue Value;
    public KeyValuePair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}
[System.Serializable]
public class JsonDataList<TKey, TValue> 
{
    public List<KeyValuePair<TKey, TValue>> dataLists;
}

public static class DictionaryJsonUtility 
{
    public static string ToJson<TKey, TValue>(Dictionary<TKey, TValue> jsonDicData, bool pretty = false)
    {
        List<KeyValuePair<TKey, TValue>> dataList = new List<KeyValuePair<TKey, TValue>>();
        KeyValuePair<TKey, TValue> dictionaryData;
        foreach (TKey key in jsonDicData.Keys) 
        {
            dictionaryData = new KeyValuePair<TKey, TValue>(key, jsonDicData[key]);
            dataList.Add(dictionaryData);
        }
        JsonDataList<TKey, TValue> listJson = new JsonDataList<TKey, TValue>();
        listJson.dataLists =dataList;
        return JsonUtility.ToJson(listJson, pretty);
    }

    public static Dictionary<TKey, TValue> FromJson<TKey, TValue>(string jsonData) 
    {
        List<KeyValuePair<TKey, TValue>> dataList = JsonUtility.FromJson<List<KeyValuePair<TKey, TValue>>>(jsonData);
        Dictionary<TKey, TValue> returnDictionary = new Dictionary<TKey, TValue>();
        for (int i = 0; i < dataList.Count; i++) 
        {
            KeyValuePair<TKey, TValue> dictionaryData = dataList[i];
            returnDictionary[dictionaryData.Key] = dictionaryData.Value;
        }

        return returnDictionary;
    }
}
