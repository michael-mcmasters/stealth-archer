using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Colors : MonoBehaviour {
    
    [SerializeField] private Color red;
    public static Color Red => dropAlpha(Colors.Instance.red);
    
    [SerializeField] private Color white;
    public static Color White => dropAlpha(Colors.Instance.white);

    [SerializeField] private Color green;
    public static Color Green => dropAlpha(Colors.Instance.green);

    [SerializeField] private Color yellow;
    public static Color Yellow => dropAlpha(Colors.Instance.yellow);
    
    [SerializeField] private Color blue;
    public static Color Blue => dropAlpha(Colors.Instance.blue);

    [SerializeField] private Color lightBlue;
    public static Color LightBlue => dropAlpha(Colors.Instance.lightBlue);
    
    [SerializeField] private Color teal;
    public static Color Teal => dropAlpha(Colors.Instance.teal);
    
    [SerializeField] private Color orange;
    public static Color Orange => dropAlpha(Colors.Instance.orange);
    
    
    private static Colors instance;
    private static Colors Instance {
        get {
            if (instance == null) instance = FindObjectOfType<Colors>();
            return instance;
        }
    }
    
    // Drop the 'a' in rgba
    private static Color dropAlpha(Color color) {
        return new Color(color.r, color.g, color.b);
    }
}