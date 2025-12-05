using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// There are too many ways to draw debug gizmos
// This abstracts all of the logic and exposes only the methods we care about

// Encapsulate all of the different ways to draw Debug gizmos
public class DebugUtil {
    
    public static void DrawLine(Vector3 start, Vector3 end, Color color, float thickness) {
        // UnityEditor.Handles.color = color;
        // UnityEditor.Handles.DrawLine(start, end, thickness);        // This sometimes throws null for some reason
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color, float thickness, float duration) {
        Debug.DrawLine(start, end, color, duration, false);
    }

    public static void DrawSphere(Vector3 point) {
        DebugExtension.DebugWireSphere(point, Color.cyan, 1, 0.1f);
    }

    public static void DrawSphere(Vector3 point, int duration) {
        DebugExtension.DebugWireSphere(point, Color.magenta, 1, duration);
    }
    
    public static void DrawSphere(Vector3 point, float scale, int duration) {
        DebugExtension.DebugWireSphere(point, Color.magenta, scale, duration);
    }
    
    public static void DrawSphere(Vector3 point, Color color) {
        DebugExtension.DebugWireSphere(point, color, 1, 0.1f);
    }
    
    public static void DrawSphere(Vector3 point, Color color, int duration) {
        DebugExtension.DebugWireSphere(point, color, 1, duration);
    }
    
    public static void DrawSphere(Vector3 point, Color color, float radius) {
        DebugExtension.DebugWireSphere(point, color, radius, 0.1f);
    }
    
    public static void DrawSphere(Vector3 point, Color color, float radius, float duration) {
        DebugExtension.DebugWireSphere(point, color, radius, duration);
    }
    
    public static void LogAll(params System.Object[] values) {
        Debug.Log("Logging objects ...");
        foreach (System.Object v in values) {
            Debug.Log(v);
        }
    }
    
    public static void DrawText(Vector3 position, string text) {
        DrawText(position, text, Color.white); 
    }
    
    public static void DrawText(Vector3 position, string text, Color color) {
        GUI.color = color;
        // Handles.Label(position, text);
    }
    
    public static void DrawText(Vector3 position, string text, Color color, int zoomDistance) {
        if (EditorCameraInRange(position, 10)) {
            GUI.color = color;
            // Handles.Label(position, text);
        }
        
        // Based off of code found here: https://forum.unity.com/threads/handles-label-with-constant-size-not-scale-based-on-distance-to-camera.781994/
        bool EditorCameraInRange(Vector3 position, float zoomDistance) {
            Vector3 cameraPos = Camera.current.WorldToScreenPoint(position);
            return cameraPos.x >= 0 && cameraPos.x <= Camera.current.pixelWidth && cameraPos.y >= 0 && cameraPos.y <= Camera.current.pixelHeight && cameraPos.z > 0 && cameraPos.z < zoomDistance;
        }
    }
    
    public static void DrawArrow(Vector3 pos, Vector3 direction, Color color) {
        DrawArrow(pos, direction, color, 0.1f);
    }
    
    // public static void DrawArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
    public static void DrawArrow(Vector3 pos, Vector3 direction, Color color, float duration) {
        float arrowHeadLength = 0.25f;
        float arrowHeadAngle = 20.0f;
        Debug.DrawRay(pos, direction, color, duration);
       
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
        Debug.DrawRay(pos + direction, right * arrowHeadLength, color, duration);
        Debug.DrawRay(pos + direction, left * arrowHeadLength, color, duration);
    }
    
    public static void SetGameObjectsColor(GameObject gameObject, Color color) {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer != null) {
            renderer.material.color = color;
        }
    }
}
