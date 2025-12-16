using _stealthArcher.scripts.constants;
using UnityEngine;

namespace _stealthArcher.scripts.gameObjects {
public class CameraAnchor : MonoBehaviour {

    void Update() {
        transform.position = GameObjects.Player.transform.position;
    }
    
}
}