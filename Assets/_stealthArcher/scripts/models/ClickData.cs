using UnityEngine;
using System.Collections;

public struct ClickData {

    public Vector3 ClickPosition { get; }
    public GameObject ObjectClicked { get; }

    public ClickData(Vector3 clickPosition, GameObject objectClicked) {
        this.ClickPosition = clickPosition;
        this.ObjectClicked = objectClicked;
    }

    // Happens when user taps map boundaries where the ground doesn't reach.
    public bool ClickedNothing() {
        return ObjectClicked == null;
    }

    public bool ClickedGround() {
        return ObjectClicked != null && ObjectClicked.transform.CompareTag("Ground");
    }
    
}