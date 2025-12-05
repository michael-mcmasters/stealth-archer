using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _stealthArcher.scripts.input {

/**
 * Checks if a GameObject has been tapped
 * The callback is only called onTapUp, and only if the distance from tapDown to tapUp is low, to ensure the user isn't actually inputting a Joystick action

 * To use, 
 * - Attach this to a GameObject
 * - In another class, set OnTapCallback
 */
public class OnTapHandler : MonoBehaviour {

    // A set of all GameObjects using this MonoBehaviour so we can track them
    public static HashSet<GameObject> TappableGameObjects { get; } = new HashSet<GameObject>();

    public Action OnTapCallback { set; get; }
    
    private Vector3 tapDownPosition;


    void Start() {
        TappableGameObjects.Add(this.gameObject);
    }
    
    void OnPointerDown(PointerEventData eventData) {
        tapDownPosition = ClickDetector.GetClick(eventData.pointerId).ClickPosition;
    }

    void OnPointerUp(PointerEventData eventData) {
        Vector3 tapUpPosition = ClickDetector.GetClick(eventData.pointerId).ClickPosition;
        if (Vector3.Distance(tapDownPosition, tapUpPosition) < 0.01f) {
            OnTapCallback();
        }
    }
    
    void OnMouseDown() {
        tapDownPosition = ClickDetector.GetClick(0).ClickPosition;
    }
    
    void OnMouseUp() {
        Vector3 tapUpPosition = ClickDetector.GetClick(0).ClickPosition;
        if (Vector3.Distance(tapDownPosition, tapUpPosition) < 0.01f) {
            OnTapCallback();
        }
    }

}
}