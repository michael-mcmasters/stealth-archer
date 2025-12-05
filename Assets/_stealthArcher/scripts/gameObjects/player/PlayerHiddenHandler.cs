using System.Collections.Generic;
using _stealthArcher.scripts.constants;
using UnityEngine;

namespace _stealthArcher.scripts.gameObjects.player {
public class PlayerHiddenHandler : MonoBehaviour {
    
    private static PlayerHiddenHandler instance;
    public static PlayerHiddenHandler Instance => (instance == null) ? instance = FindObjectOfType<PlayerHiddenHandler>() : instance;
    
    // Keep track of colliders player is hiding inside so that one collider's OnTriggerExit doesn't make player visible when moving between hidden colliders
    private HashSet<Collider> collidersHidingInside = new HashSet<Collider>();
    
    
    void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag(Tags.Hidden)) {
            collidersHidingInside.Add(collider);
            DebugUtil.SetGameObjectsColor(this.gameObject, Colors.Yellow);
        }
    }

    void OnTriggerExit(Collider collider) {
        if (collider.CompareTag(Tags.Hidden)) {
            collidersHidingInside.Remove(collider);

            if (collidersHidingInside.Count == 0) {
                DebugUtil.SetGameObjectsColor(this.gameObject, Colors.White);
            }
        }
    }

    public bool PlayerIsHidden() {
        return collidersHidingInside.Count > 0;
    }
    
}
}