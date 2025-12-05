using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace _stealthArcher.scripts.constants {

public class LayerMasks : MonoBehaviour {

    [SerializeField] private LayerMask groundPlane;             // Used for mouse clicks and such. Expands past the actual ground so that raycasts still hit something when "out of bounds".
    public static LayerMask GroundPlane => Instance.groundPlane;
    
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
    

    private static LayerMasks instance;
    public static LayerMasks Instance => (instance == null) ? instance = FindObjectOfType<LayerMasks>() : instance;
    
}

}