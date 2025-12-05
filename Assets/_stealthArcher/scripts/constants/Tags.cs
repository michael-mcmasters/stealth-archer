using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace _stealthArcher.scripts.constants {

public class Tags : MonoBehaviour {

    [SerializeField] private string enemy;
    public static string Enemy => Tags.Instance.enemy;
    
    [SerializeField] private string hidden;
    public static string Hidden => Tags.Instance.hidden;
    

    private static Tags instance;
    public static Tags Instance => (instance == null) ? instance = FindObjectOfType<Tags>() : instance;
    
}

}