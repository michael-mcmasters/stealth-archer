using _stealthArcher.scripts.constants;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _stealthArcher.scripts.gameObjects {
public class CameraDragRotateButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    private int pointerId;
    [SerializeField] private float sensitivity = 2.5f;
    private Vector3 tapLastFramePosition;
    private bool isHolding;
    
    void Update() {
        if (!isHolding) return;
        
        Vector3 tapCurrentPosition = ClickDetector.GetClick(pointerId).ClickPosition;
        float yRotation = (tapCurrentPosition.x - tapLastFramePosition.x) * sensitivity;
        Vector3 euler = GameObjects.CameraAnchor.transform.eulerAngles;
        euler.y += yRotation;
        GameObjects.CameraAnchor.transform.eulerAngles = euler;

        tapLastFramePosition = tapCurrentPosition;
    }

    public void OnPointerDown(PointerEventData eventData) {
        pointerId = eventData.pointerId;
        tapLastFramePosition = Vector3.zero;
        isHolding = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        isHolding = false;
    }

    
}
}