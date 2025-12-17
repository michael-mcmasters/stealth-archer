using _stealthArcher.scripts.constants;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _stealthArcher.scripts.gameObjects {
public class CameraRotateButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    [SerializeField] private bool isCameraLeftButton;
    [SerializeField] private float speed = 1.2f;
    private bool isHolding;

    
    void Update() {
        if (!isHolding) return;
        
        Vector3 euler = GameObjects.CameraAnchor.transform.eulerAngles;
        if (isCameraLeftButton) {
            euler.y -= speed;
        } else {
            euler.y += speed;
        }
        GameObjects.CameraAnchor.transform.eulerAngles = euler;
    }

    public void OnPointerDown(PointerEventData eventData) {
        isHolding = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        isHolding = false;
    }

}
}