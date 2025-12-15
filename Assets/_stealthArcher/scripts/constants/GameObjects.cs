using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace _stealthArcher.scripts.constants {

public class GameObjects : MonoBehaviour {

    [Header("General")]
    [SerializeField] private GameObject player;
    public static GameObject Player => Instance.player;
    
    [SerializeField] private GameObject cancelText;
    public static GameObject CancelText => Instance.cancelText;

    [SerializeField] private GameObject arrowPrefab;
    public static GameObject ArrowPrefab => Instance.arrowPrefab;
    
    [SerializeField] private GameObject arrowPrefab2;
    public static GameObject ArrowPrefab2 => Instance.arrowPrefab2;
    
    [SerializeField] private GameObject bowPrefab;
    public static GameObject BowPrefab => Instance.bowPrefab;
    
    [SerializeField] private GameObject heavyBowPrefab;
    public static GameObject HeavyBowPrefab => Instance.heavyBowPrefab;
    
    [SerializeField] private GameObject cursorBowPrefab;
    public static GameObject CursorBowPrefab => Instance.cursorBowPrefab;
    
    [SerializeField] private GameObject cursorManualBowPrefab;
    public static GameObject CursorManualBowPrefab => Instance.cursorManualBowPrefab;
    
    [SerializeField] private GameObject regularBowPrefab;
    public static GameObject RegularBowPrefab => Instance.regularBowPrefab;
    
    [SerializeField] private GameObject boomerangPrefab;
    public static GameObject BoomerangPrefab => Instance.boomerangPrefab;
    
    [SerializeField] private GameObject boomerangProjectile;
    public static GameObject BoomerangProjectile => Instance.boomerangProjectile;
    
    
    [Header("UI")]
    [SerializeField] private GameObject cursor;
    public static GameObject Cursor => Instance.cursor;
    
    [SerializeField] private GameObject leftJoystick;
    public static GameObject LeftJoystick => Instance.leftJoystick;
    
    [SerializeField] private GameObject leftJoystickOuterCircleObj;
    public static GameObject LeftJoystickOuterCircleObj => Instance.leftJoystickOuterCircleObj;
    
    [SerializeField] private GameObject leftJoystickInnerCircleObj;
    public static GameObject LeftJoystickInnerCircleObj => Instance.leftJoystickInnerCircleObj;
    
    [SerializeField] private GameObject rightJoystick;
    public static GameObject RightJoystick => Instance.rightJoystick;
    
    [SerializeField] private GameObject rightJoystickOuterCircleObj;
    public static GameObject RightJoystickOuterCircleObj => Instance.rightJoystickOuterCircleObj;
    
    [SerializeField] private GameObject rightJoystickInnerCircleObj;
    public static GameObject RightJoystickInnerCircleObj => Instance.rightJoystickInnerCircleObj;

    [SerializeField] private GameObject headshotTextPrefab;
    public static GameObject HeadshotTextPrefab => Instance.headshotTextPrefab;
    
    
    private static GameObjects instance;
    public static GameObjects Instance => (instance == null) ? instance = FindObjectOfType<GameObjects>() : instance;
    
}

}