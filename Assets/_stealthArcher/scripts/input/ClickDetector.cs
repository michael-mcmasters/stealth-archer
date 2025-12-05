using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using _stealthArcher.scripts;
using _stealthArcher.scripts.constants;
using _stealthArcher.scripts.input;

public class ClickDetector : MonoBehaviour {

    /**
     * "params" is C#'s syntax for a rest operator.
     * Pass 0 LayerMasks and it will default to the Ground LayerMask.
     * Pass 1 or more LayerMasks and it will combine them using bitwise operation.
     */
    public static ClickData GetClick(int touchIndex, params LayerMask[] layerMasks) {
        // If no LayerMask is given, use the Ground Plane LayerMask
        if (layerMasks.Length == 0) {
            layerMasks = new[] { LayerMasks.GroundPlane };
        }

        // Combine LayerMasks into 1 using bit math operation
        LayerMask combinedLayerMask = layerMasks[0];
        for (int i = 1; i < layerMasks.Length; i++) {
            combinedLayerMask |= layerMasks[i];
        }
        
        return ShootRayFromScreen(GetScreenTapPosition(touchIndex), combinedLayerMask);
    }
    
    // Returns tap or mouse position on screen.
    public static Vector2 GetScreenTapPosition(int touchIndex) {
        bool usingTouchInput = Input.touchCount >= 1;
        if (usingTouchInput) {
            return Input.GetTouch(touchIndex).position;
        }
        else {
            return Input.mousePosition;
        }
    }

    public static ClickData ShootRayFromScreen(Vector3 screenTapPos, LayerMask layerMask) {
        Ray rayStartPointAndDir = Camera.main.ScreenPointToRay(screenTapPos);
        if (layerMask == 0) {
            if (Physics.Raycast(rayStartPointAndDir, out RaycastHit hit, 300)) {
                return new ClickData(hit.point, hit.transform.gameObject);
            }
        } else {
            if (Physics.Raycast(rayStartPointAndDir, out RaycastHit hit, 300, layerMask)) {
                return new ClickData(hit.point, hit.transform.gameObject);
            }
        }
        return new ClickData();
    }

    public static bool TouchIsOverUiElement(Vector3 touchPosition) {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = touchPosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, raycastResults);
        
        // Ignore Joystick UI elements
        HashSet<GameObject> ignoredUiElements = new HashSet<GameObject>() {
            GameObjects.LeftJoystick,
            GameObjects.LeftJoystickOuterCircleObj,
            GameObjects.LeftJoystickInnerCircleObj,
            GameObjects.RightJoystick,
            GameObjects.RightJoystickOuterCircleObj,
            GameObjects.RightJoystickInnerCircleObj
        };
        raycastResults.RemoveAll(result => ignoredUiElements.Contains(result.gameObject));
        
        return raycastResults.Count > 0;
    }

    /**
     * Returns true if player is touching a GameObject that has the OnTapHandler MonoBehaviour
     */
    public static bool TouchIsOverTappableGameObject(Vector3 touchPosition) {
        GameObject objectClicked = ShootRayFromScreen(touchPosition, 0).ObjectClicked;
        return OnTapHandler.TappableGameObjects.Contains(objectClicked);
    }
    
}
