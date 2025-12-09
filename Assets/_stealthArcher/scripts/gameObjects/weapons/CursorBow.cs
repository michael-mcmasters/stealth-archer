using _stealthArcher.scripts.constants;
using UnityEngine;

namespace _stealthArcher.scripts.weaponPrefabs {

public class CursorBow : IWeapon {
    
    private Trackpad trackpad;
    
    private GameObject playerObj;
    private GameObject cursorObj;
    private LineRenderer lineRendererObj;
    
    void Start() {
        this.trackpad = new Trackpad(SpecificTouch.Right, (joystickEvent, joystickData) => {
            Debug.Log("RECEIVED");
            switch (joystickEvent) {
                case JoystickEvent.Down:
                    BeginAim(joystickData);
                    break;
                case JoystickEvent.Hold:
                    ContinueAim(playerObj, joystickData);
                    break;
                case JoystickEvent.Up:
                    FinishAim();
                    break;
            }
        });
        this.playerObj = GameObjects.Player;
    }

    public override void HandleInput() {
        trackpad?.HandleInput();
    }

    private void BeginAim(JoystickData joystickData) {
        Debug.Log("BeginAim");
        // Create Cursor
        cursorObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        cursorObj.transform.localScale = new Vector3(0.1f, 10, 0.1f);
        cursorObj.transform.position = joystickData.AroundPlayerPosition;
        
        // Create Line Renderer
        GameObject lineObj = new GameObject("DynamicLineRenderer");
        lineRendererObj = lineObj.AddComponent<LineRenderer>();
        lineRendererObj.positionCount = 0;
        lineRendererObj.startWidth = 0.1f;
        lineRendererObj.endWidth = 0.1f;
        lineRendererObj.material = new Material(Shader.Find("Sprites/Default"));
    }

    private void ContinueAim(GameObject playerObj, JoystickData joystickData) {
        Debug.Log("ContinueAim");
        // Aimer circle follows cursor
        cursorObj.transform.position = joystickData.AroundPlayerPosition;

        // Line follows circle
        float aimerHeight = playerObj.transform.position.y + playerObj.transform.localScale.y;
        lineRendererObj.positionCount = 2;
        lineRendererObj.SetPosition(0, new Vector3(playerObj.transform.position.x, aimerHeight, playerObj.transform.position.z));
        lineRendererObj.SetPosition(1, joystickData.AroundPlayerPosition);
    }

    private void FinishAim() {
        Debug.Log("FinishAim");
        Debug.Log("Shooting isn't completed yet");
    }

    private void CancelAim() {
        Debug.Log("CancelAim");
    }
}
}