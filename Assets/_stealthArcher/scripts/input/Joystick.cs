using System;
using _stealthArcher.scripts;
using _stealthArcher.scripts.constants;
using _stealthArcher.scripts.models;
using UnityEngine;

namespace _stealthArcher {
public class Joystick {
    
    private GameObject joystickOuterCircleObj;
    private GameObject joystickInnerCircleObj;
    private GameObject boundaryObj;
    
    private GameObject playerObj;
    private SpecificTouch specificTouch;
    private Action<JoystickEvent, JoystickData> callback;
    
    private int initialTouchIndex;
    private int touchIndex;
    private float touchSensitivity = 3;
    private Vector3 joystickInitialPosition;
    private float endDistance;

    private bool enableDebug = false;

    
    public Joystick(SpecificTouch specificTouch, Action<JoystickEvent, JoystickData> callback) {
        this.specificTouch = specificTouch;
        this.playerObj = GameObjects.Player;
        this.callback = callback;
        this.initialTouchIndex = -1;
        
        GameObject joystick = (specificTouch == SpecificTouch.Left) ? GameObject.Find("LeftJoystick") : GameObject.Find("RightJoystick");
        // GameObject joystick = (specificTouch == SpecificTouch.Left) ? GameObject.Find("RightJoystick") : GameObject.Find("LeftJoystick");
        joystickOuterCircleObj = joystick.transform.GetChild(0).gameObject;
        joystickInnerCircleObj = joystick.transform.GetChild(1).gameObject;
        joystickInitialPosition = joystick.transform.position;
        boundaryObj = joystickOuterCircleObj.transform.GetChild(0).gameObject;
        
        endDistance = Vector3.Distance(joystickOuterCircleObj.transform.position, boundaryObj.transform.position);
    }
    
    public void HandleInput() {
        if (initialTouchIndex == -1 && InputFacade.GetAnyTouchDown()) {
            initialTouchIndex = InputFacade.GetJoystickInitialTouchIndex(specificTouch);
        }

        if (initialTouchIndex != -1) {
            touchIndex = InputFacade.GetJoystickCurrentTouchIndex(initialTouchIndex);
            JoystickData joystickData = new JoystickData();
            if (InputFacade.GetTouchDown(touchIndex)) {
                MoveJoystick(InputFacade.GetTouchPosition(touchIndex));
                Vector3 bigCirclePositionToGround = GetJoystickToWorldPosition(joystickOuterCircleObj.transform.position);
                Vector3 smallCirclePositionToGround = GetJoystickToWorldPosition(joystickInnerCircleObj.transform.position);
                joystickData.AroundPlayerPosition = MimicTouchInputAroundPlayer(bigCirclePositionToGround, smallCirclePositionToGround);
                callback(JoystickEvent.Down, joystickData);
            }
            if (InputFacade.GetTouch(touchIndex)) {
                MoveJoystickWithTouch();
                Vector3 bigCirclePositionToGround = GetJoystickToWorldPosition(joystickOuterCircleObj.transform.position);
                Vector3 smallCirclePositionToGround = GetJoystickToWorldPosition(joystickInnerCircleObj.transform.position);
                joystickData.AroundPlayerPosition = MimicTouchInputAroundPlayer(bigCirclePositionToGround, smallCirclePositionToGround);
                joystickData.Direction = (smallCirclePositionToGround - bigCirclePositionToGround).normalized;
                joystickData.Rotation = Quaternion.LookRotation((smallCirclePositionToGround - bigCirclePositionToGround), Vector3.up);
                
                float startDistance = 0;
                float midDistance = Vector3.Distance(joystickOuterCircleObj.transform.position, joystickInnerCircleObj.transform.position);
                float percentage = ((midDistance - startDistance) / (endDistance - startDistance));
                joystickData.TiltPercentage = percentage;
                callback(JoystickEvent.Hold, joystickData);
                
                if (enableDebug) {
                    DebugUtil.DrawSphere(smallCirclePositionToGround, Color.gray, 0.5f);
                    DebugUtil.DrawSphere(bigCirclePositionToGround, Color.red);
                }
            }
            if (InputFacade.GetTouchUp(touchIndex)) {
                callback(JoystickEvent.Up, joystickData);
                MoveJoystick(joystickInitialPosition);
                initialTouchIndex = -1;
            }
        }
    }

    private void MoveJoystick(Vector3 screenPosition) {
        Debug.Log("screenPosition: " + screenPosition);
        joystickOuterCircleObj.transform.position = screenPosition;
        joystickInnerCircleObj.transform.position = screenPosition;
    }

    // Follows finger
    private void MoveJoystickWithTouch() {
        Debug.Log("MoveJoystickWithTouch");
        // Move inner circle
        joystickInnerCircleObj.transform.position = InputFacade.GetTouchPosition(touchIndex);
        float maxDistance = Vector3.Distance(joystickOuterCircleObj.transform.position, boundaryObj.transform.position);
        float distance = Vector3.Distance(joystickOuterCircleObj.transform.position, joystickInnerCircleObj.transform.position);

        if (distance > maxDistance) {
            float leftoverDistance = distance - maxDistance;
            Vector3 direction = (joystickInnerCircleObj.transform.position - joystickOuterCircleObj.transform.position).normalized;
            Vector3 newPosition = joystickOuterCircleObj.transform.position + direction * leftoverDistance;
            
            // transform.position works perfectly
            // If you want to use anchorPoint, just make sure to use the point in local position to the UI element.
            // Src: https://forum.unity.com/threads/moving-ui-element-with-position-or-recttransform-anchoredposition.899372/
            // ChatGPT also said to use this, but it didn't work: object1.position = joystickOuterCircleObj.GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main, newPosition);
            joystickOuterCircleObj.transform.position = newPosition;
        }
    }
    
    // Shoot ray from Joystick position to the ground
    private Vector3 GetJoystickToWorldPosition(Vector2 joystickPosition) {
        Ray ray = Camera.main.ScreenPointToRay(joystickPosition);
        LayerMask groundLayerMask = LayerMasks.GroundPlane;

        if (Physics.Raycast(ray, out RaycastHit hit, 300, groundLayerMask)) {
            return hit.point;
        }

        Debug.LogError("Ray did not hit anything");
        return Vector3.zero;
    }
    
    // Moves joystick position to the player as if they are the joystick
    public Vector3 MimicTouchInputAroundPlayer(Vector3 touchStartPosition, Vector3 touchCurrentPosition) {
        Vector3 dragDirection = (touchCurrentPosition - touchStartPosition).normalized;
        float dragDistance = Vector3.Distance(touchStartPosition, touchCurrentPosition);

        Vector3 underPlayerPosition = playerObj.transform.position;
        underPlayerPosition = new Vector3(playerObj.transform.position.x, 0, playerObj.transform.position.z);

        // Pretend touch started on the player and recreate current touch position using that direction and distance
        float distance = dragDistance * touchSensitivity;
        Vector3 touchCurrentPositionMimickedAroundPlayer = VectorUtil.NewPointInDirection(underPlayerPosition, dragDirection, distance);

        if (enableDebug) {
            DebugUtil.DrawSphere(touchCurrentPosition, Color.gray, 0.5f);
            DebugUtil.DrawSphere(touchCurrentPositionMimickedAroundPlayer, Color.white);
        }

        return touchCurrentPositionMimickedAroundPlayer;
    }
    
}
}