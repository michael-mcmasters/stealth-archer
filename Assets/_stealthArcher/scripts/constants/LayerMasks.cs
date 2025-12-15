using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace _stealthArcher.scripts.constants {

public class LayerMasks : MonoBehaviour {

    [SerializeField] private LayerMask ground;
    public static LayerMask Ground => Instance.ground;
    
    [SerializeField] private LayerMask player;
    public static LayerMask Player => Instance.player;
    
    [SerializeField] private LayerMask enemy;
    public static LayerMask Enemy => Instance.enemy;
    
    [SerializeField] private LayerMask arrow;
    public static LayerMask Arrow => Instance.arrow;
    
    [SerializeField] private LayerMask hidden;
    public static LayerMask Hidden => Instance.hidden;
    
    [SerializeField] private LayerMask detectPlayerAimer;
    public static LayerMask DetectPlayerAimer => Instance.detectPlayerAimer;
    

    private static LayerMasks instance;
    public static LayerMasks Instance => (instance == null) ? instance = FindObjectOfType<LayerMasks>() : instance;
    
    
    // Returns int value of LayerMask ... This only works for 1 LayerMask (not multiple layers at once)
    public int LayerMaskToLayer(LayerMask mask) {
        for (int i = 0; i < 32; i++) {
            if ((mask.value & (1 << i)) != 0) {
                return i;
            }
        }
        return -1;  // No valid layer found
    }
    
}

}