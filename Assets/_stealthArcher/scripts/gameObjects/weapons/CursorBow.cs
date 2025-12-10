using System.Collections.Generic;
using _stealthArcher.scripts.constants;
using Unity.VisualScripting;
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
        // Create Cursor
        cursorObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        Destroy(cursorObj.GetComponent<Collider>());
        cursorObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
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
        // Shoot ray from screen to AroundPlayerPosition to detect what is under cursor
        Vector3 cursorPosition = Vector3.zero;
        Vector3 target = VectorUtil.NewPointInDirection(joystickData.AroundPlayerPosition, joystickData.Direction, 10); // push target forward a little so it doesn't start where player obj is
        Vector3 origin = Camera.main.transform.position;
        Vector3 direction = (target - origin).normalized;
        Ray ray = new Ray(origin, direction);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f)) {
            cursorPosition = hit.point;
        }
        
        // Cursor follows touch
        cursorObj.transform.position = cursorPosition;

        // Line follows circle
        float aimerHeight = playerObj.transform.position.y + playerObj.transform.localScale.y;
        lineRendererObj.positionCount = 2;
        lineRendererObj.SetPosition(0, new Vector3(playerObj.transform.position.x, aimerHeight, playerObj.transform.position.z));
        lineRendererObj.SetPosition(1, cursorPosition);
    }
    
    private void FinishAim() {
        Vector3 start = lineRendererObj.GetPosition(0);
        Vector3 end = lineRendererObj.GetPosition(1);
        Quaternion spawnRotation = Quaternion.LookRotation((end - start).normalized);
        Arrow arrow = Instantiate(GameObjects.ArrowPrefab, start, spawnRotation).GetComponent<Arrow>();
        
        Destroy(lineRendererObj);
        arrow.Shoot(new List<Vector3>() {start, end}, 25, 5);
    }

    private void CancelAim() {
        Debug.Log("CancelAim");
    }
}
}