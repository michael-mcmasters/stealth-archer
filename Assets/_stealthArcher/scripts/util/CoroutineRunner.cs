using System.Collections;
using UnityEngine;

namespace _stealthArcher.scripts.util {
    
/* Utility class to run Coroutines when not in a MonoBehaviour */
public class CoroutineRunner : MonoBehaviour {
    private static CoroutineRunner _instance;

    public static CoroutineRunner Instance {
        get {
            if (_instance == null) {
                GameObject go = new GameObject("CoroutineRunner");
                _instance = go.AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(go);
            }

            return _instance;
        }
    }

    public static Coroutine Run(IEnumerator coroutine) {
        return Instance.StartCoroutine(coroutine);
    }
}
}