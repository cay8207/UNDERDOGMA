using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 싱글턴화 시켜줍니다.
/// 사용법 ClassName : Singleton<ClassName>으로 상속
/// 이후 ClassName은 Singleton이 적용되어 Instance를 통해 접근 가능
/// Manager류 스크립트에 적용할 것
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject($"{typeof(T)} (Singleton)");
                    _instance = singletonObject.AddComponent<T>();
                    singletonObject.transform.parent = null;
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return _instance;
        }
    }
}