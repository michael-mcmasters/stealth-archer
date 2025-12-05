using System.Collections.Generic;
using _stealthArcher.scripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _stealthArcher {

/**
 * This class wraps the Input class to simplify handling Touch and Mouse events
 * 
 * For example, instead of other classes checking for both input types:
 *      if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseDown(0)) { ... }
 * They can simply call:
 *      if (InputFacade.GetAnyTouchDown()) { ... }
 */
public class InputFacade {
    
    /**
     * Returns true if any touches or mouse clicks began being pressed down
     */
    public static bool GetAnyTouchDown() {
        for (int i = 0; i < Input.touchCount; i++) {
            if (Input.GetTouch(i).phase == TouchPhase.Began) {
                return true;
            }
        }
        if (Input.GetMouseButtonDown(0)) {
            return true;
        }

        return false;
    }

    /**
     * Returns true if any touches or mouse clicks are pressed down
     */
    public static bool GetAny() {
        for (int i = 0; i < Input.touchCount; i++) {
            if (Input.GetTouch(i).phase == TouchPhase.Stationary || Input.GetTouch(i).phase == TouchPhase.Moved) {
                return true;
            }
        }
        if (Input.GetMouseButton(0)) {
            return true;
        }

        return false;
    }
    
    /**
     * Returns true if any touches or mouse clicks are no longer pressed
     */
    public static bool GetAnyTouchUp() {
        for (int i = 0; i < Input.touchCount; i++) {
            if (Input.GetTouch(i).phase == TouchPhase.Ended) {
                return true;
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            return true;
        }

        return false;
    }

    public static bool GetTouchDown(int touchIndex) {
        if (Input.touchCount > 0) {
            return Input.GetTouch(touchIndex).phase == TouchPhase.Began;
        }
        return Input.GetMouseButtonDown(0);
    }
    
    public static bool GetTouch(int touchIndex) {
        if (Input.touchCount > 0) {
            return Input.GetTouch(touchIndex).phase == TouchPhase.Stationary || Input.GetTouch(touchIndex).phase == TouchPhase.Moved;
        }
        return Input.GetMouseButton(0);
    }
    
    public static bool GetTouchUp(int touchIndex) {
        if (Input.touchCount > 0) {
            return Input.GetTouch(touchIndex).phase == TouchPhase.Ended;
        }
        return Input.GetMouseButtonUp(0);
    }
    
    public static Vector3 GetTouchPosition(int touchIndex) {
        if (Input.touchCount > 0) {
            return Input.GetTouch(touchIndex).position;
        }
        return Input.mousePosition;
    }
    
    /**
     * Returns touch index if it is pressing the passed joystick
     * Returns -1 if not
     *
     * For example, if Left joystick is passed, and a touch down occured on the left half of the screen, returns that touch index as long as it's not touching a UI element or tappable game object
     * Note: If tappable game objects are getting in the way too often, may be a good idea for TouchIsOverTappableGameObject() to only return true if gameobject is within a few units of player
     */
    public static int GetJoystickInitialTouchIndex(SpecificTouch specificTouch) {
        float halfScreenWidth = Screen.width * 0.5f;
        
        if (Input.touchCount > 0) {
            for (int i = 0; i < Input.touchCount; i++) {
                if (Input.GetTouch(i).phase == TouchPhase.Began && !ClickDetector.TouchIsOverUiElement(Input.GetTouch(i).position) && !ClickDetector.TouchIsOverTappableGameObject(Input.GetTouch(i).position)) {
                    if (specificTouch == SpecificTouch.Left && Input.GetTouch(i).position.x < halfScreenWidth) {
                        return i;
                    }
                    else if (specificTouch == SpecificTouch.Right && Input.GetTouch(i).position.x > halfScreenWidth) {
                        return i;
                    }
                }
            }
        }
        else {
            // Return index 0 because mouse ClickDetector will just use the mouse anyway if there are no touches. And the mouse can only click the screen in one place at a time.
            if (Input.GetMouseButtonDown(0) && !ClickDetector.TouchIsOverUiElement(Input.mousePosition) && !ClickDetector.TouchIsOverTappableGameObject(Input.mousePosition)) {
                if (specificTouch == SpecificTouch.Left && Input.mousePosition.x < halfScreenWidth) {
                    return 0;
                }
                else if (specificTouch == SpecificTouch.Right && Input.mousePosition.x > halfScreenWidth) {
                    return 0;
                }
            }
        }
    
        return -1;
    }

    /**
     * Returns the current touch index that is touching the joystick
     * The touch index can change when other touches touch or let go
     * For example:
     *
     * Action -------------------- Right Touch Index
     * left & right touch down     1
     * left touch up               0
     * left touch down             1
     */
    public static int GetJoystickCurrentTouchIndex(int initialTouchIndex) {
        if (Input.touchCount <= 0) return 0;        // Probably is a mouse click
        
        if (Input.touchCount == 1) {
            // If touch was index 1, now has to be index 0 to not be out of bounds
            return 0;
        } else if (Input.touchCount == 2) {
            // But if other thumb touches again, and this was initially index 1, other thumb bumps this back up to index 1 again. So here we are using that index again. (Confusing, I know.)
            return initialTouchIndex;
        }

        // TODO:
        // If there are 3 or more touches, goes back to its original index.
        // To be safe, maybe can throw an exception, catch it, and disable all joysticks so player lets go all touches.
        return initialTouchIndex;
    }
}
}