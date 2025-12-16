using System;
using _stealthArcher.scripts.constants;
using _stealthArcher.scripts.models;
using DigitalRubyShared;
using UnityEngine;
using TouchPhase = UnityEngine.TouchPhase;

namespace _stealthArcher.scripts {
public class Trackpad {

    private GameObject playerObj;
    private SpecificTouch specificTouch;
    private Action<JoystickEvent, JoystickData> callback;
    
    private int initialTouchIndex;
    private int touchIndex;

    private Vector3 touchStartPositionScreen;
    private float touchSensitivity = 3;
    private bool inCancelRange;

    private bool touchDownAlreadyHandled;

    private bool enableDebug = true;

    
    public Trackpad(SpecificTouch specificTouch, Action<JoystickEvent, JoystickData> callback) {
        this.specificTouch = specificTouch;
        this.playerObj = GameObjects.Player;
        this.callback = callback;
        ResetVariables();
    }

    public void HandleInput() {
        if (initialTouchIndex == -1 && InputFacade.GetAnyTouchDown()) {
            ResetVariables();
            initialTouchIndex = InputFacade.GetJoystickInitialTouchIndex(specificTouch);
        }

        if (initialTouchIndex != -1) {
            touchIndex = InputFacade.GetJoystickCurrentTouchIndex(initialTouchIndex);
            JoystickData joystickData = new JoystickData();
            
            if (InputFacade.GetTouchDown(touchIndex)) {
                // Edge case where when player switches apps, touchdown and touch are called but touchup isn't, which was causing issues when switching back to the app
                if (touchDownAlreadyHandled) {
                    callback(JoystickEvent.Cancel, joystickData);
                    ResetVariables();
                    return;
                }
                touchDownAlreadyHandled = true;
                touchStartPositionScreen = InputFacade.GetTouchPosition(touchIndex);
                callback(JoystickEvent.Down, joystickData);
            }

            if (InputFacade.GetTouch(touchIndex)) {
                Vector3 touchGroundStartPosition = GetAdjustedStartPosition();
                Vector3 touchGroundCurrentPosition = ClickDetector.GetClick(touchIndex).ClickPosition;
                joystickData.AroundPlayerPosition = MimicTouchInputAroundPlayer(playerObj, touchGroundStartPosition, touchGroundCurrentPosition);
                joystickData.Direction = (touchGroundCurrentPosition - touchGroundStartPosition).normalized;
                joystickData.Rotation = Quaternion.LookRotation((touchGroundCurrentPosition - touchGroundStartPosition), Vector3.up);
                
                inCancelRange = Vector3.Distance(touchGroundStartPosition, touchGroundCurrentPosition) < 0.5f;
                if (!InputFacade.GetTouchDown(touchIndex) && inCancelRange) {
                    GameObjects.CancelText.SetActive(true);
                    callback(JoystickEvent.InCancelRange, joystickData);
                } else {
                    GameObjects.CancelText.SetActive(false);
                    callback(JoystickEvent.Hold, joystickData);
                }
            }

            if (InputFacade.GetTouchUp(touchIndex)) {
                if (inCancelRange) {
                    callback(JoystickEvent.Cancel, joystickData);
                } else {
                    callback(JoystickEvent.Up, joystickData);
                }
                ResetVariables();
            }
        }
    }
    
    private void ResetVariables() {
        this.initialTouchIndex = -1;
        this.inCancelRange = false;
        this.touchDownAlreadyHandled = false;        
        GameObjects.CancelText.SetActive(false);
    }
    
    /**
     * Returns current touch position as if the user touched the player object and dragged around it
     * Maps screen touches to the player object
     * 
     * Sensitivity determines how much a touch drag moves input around player.
     * For example, if sensitivity is 2, and the touch drags 3 units, the returned Vector3 will be 6 units from player's center position
     */
    // private Vector3 MimicTouchInputAroundPlayer(GameObject playerObj, int touchIndex) {
    private Vector3 MimicTouchInputAroundPlayer(GameObject playerObj, Vector3 touchStartPosition, Vector3 touchCurrentPosition) {
        // Get direction and distance of touches on screen
        Vector3 dragDirection = (touchCurrentPosition - touchStartPosition).normalized;
        float dragDistance = Vector3.Distance(touchStartPosition, touchCurrentPosition);

        Vector3 underPlayerPosition = playerObj.transform.position;
        underPlayerPosition = new Vector3(playerObj.transform.position.x, 0, playerObj.transform.position.z);

        // Pretend touch started on the player and recreate current touch position using that direction and distance
        Vector3 touchCurrentPositionMimickedAroundPlayer = VectorUtil.NewPointInDirection(underPlayerPosition, dragDirection, dragDistance * touchSensitivity);

        if (enableDebug) {
            DebugUtil.DrawSphere(touchCurrentPosition, Color.gray, 0.5f);
            DebugUtil.DrawSphere(touchCurrentPositionMimickedAroundPlayer, Color.white);
        }

        return touchCurrentPositionMimickedAroundPlayer;
    }
    
    /**
     * When player moves, the start position on the screen stays the same but the start position on the ground can change
     * This method re-shoots ray from screen to ground to get the new ground start position relevant to the player's position
     */
    private Vector3 GetAdjustedStartPosition() {
        Vector3 startPosition = ClickDetector.ShootRayFromScreen(touchStartPositionScreen, LayerMasks.Ground).ClickPosition;
        if (enableDebug) {
            DebugUtil.DrawSphere(startPosition, Color.red);
        }
    
        return startPosition;
    }
}
}