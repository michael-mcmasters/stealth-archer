using System.Collections;
using UnityEngine;

namespace _stealthArcher.scripts.util {
    
/* Utility class for regular classes to run MonoBehaviour methods */
public class MonoBehaviourUtil : MonoBehaviour {
    private static MonoBehaviourUtil _instance;

    public static MonoBehaviourUtil Instance {
        get {
            if (_instance == null) {
                GameObject go = new GameObject("CoroutineRunner");
                _instance = go.AddComponent<MonoBehaviourUtil>();
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