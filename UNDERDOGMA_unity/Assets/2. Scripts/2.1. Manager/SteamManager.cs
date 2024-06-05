// using UnityEngine;
// using Steamworks;

// public class SteamManager : MonoBehaviour
// {
//     private static SteamManager _instance;

//     void Awake()
//     {
//         if (_instance == null)
//         {
//             _instance = this;
//             DontDestroyOnLoad(gameObject);
//             if (!SteamAPI.Init())
//             {
//                 Debug.LogError("SteamAPI.Init() failed!");
//                 return;
//             }
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }

//     void OnEnable()
//     {
//         if (_instance == this)
//         {
//             SteamAPI.RunCallbacks();
//         }
//     }

//     void OnDisable()
//     {
//         if (_instance == this)
//         {
//             SteamAPI.Shutdown();
//         }
//     }

//     void Update()
//     {
//         if (_instance == this)
//         {
//             SteamAPI.RunCallbacks();
//         }
//     }
// }
