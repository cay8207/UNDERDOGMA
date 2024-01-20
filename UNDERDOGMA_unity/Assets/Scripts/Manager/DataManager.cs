using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerData
{
    public string name;
}

public class DataManager : Singleton<DataManager>
{
    void Start()
    {
        string data = JsonUtility.ToJson(Instance);
        print(data);
    }
}
