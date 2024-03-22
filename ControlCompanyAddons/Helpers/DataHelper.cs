using System.Collections.Generic;
using UnityEngine;

namespace ControlCompanyAddons.Helpers;

public class DataHelper : MonoBehaviour {
    private readonly Dictionary<string, object> _dataDictionary = [];

    public bool HasData(string key) {
        return _dataDictionary.ContainsKey(key);
    }

    public object GetData(string key) {
        return _dataDictionary.GetValueOrDefault(key);
    }

    public void SetData(string key, object data) {
        _dataDictionary.Remove(key);

        _dataDictionary.Add(key, data);
    }
}