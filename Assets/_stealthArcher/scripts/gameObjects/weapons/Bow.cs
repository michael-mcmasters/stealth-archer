using System.Collections.Generic;
using _stealthArcher.scripts.constants;
using Unity.VisualScripting;
using UnityEngine;

namespace _stealthArcher.scripts.weaponPrefabs {

public class Bow : IWeapon {
    
    private Trackpad trackpad;
    
    private GameObject playerObj;
    private GameObject cursorObj;
    private LineRenderer lineRendererToCursorObj;
    private LineRenderer lineRendererToGroundObj;

    
    void Start() {
        this.trackpad = new Trackpad(SpecificTouch.Right, (joystickEvent, joystickData) => {
            switch (joystickEvent) {
                case JoystickEvent.Down:
                    BeginAim(playerObj, joystickData);
                    break;
                case JoystickEvent.Hold:
                    ContinueAim(playerObj, joystickData);
                    break;
                case JoystickEvent.InCancelRange:
                    ContinueAim(playerObj, joystickData);
                    break;
                case JoystickEvent.Up:
                    FinishAim();
                    break;
                case JoystickEvent.Cancel:
                    FinishAim();
                    // CancelAim();
                    break;
            }
        });
        this.playerObj = GameObjects.Player;
    }

    public override void HandleInput() {
        trackpad?.HandleInput();
    }

    private void BeginAim(GameObject playerObj, JoystickData joystickData) {
        // Create Cursor
        cursorObj = Instantiate(GameObjects.Cursor);
        
        GameObject lr1 = new GameObject("LineRendererToCursorObj");
        lineRendererToCursorObj = lr1.AddComponent<LineRenderer>();
        lineRendererToCursorObj.transform.SetParent(playerObj.transform);
        lineRendererToCursorObj.transform.localPosition = Vector3.zero;
        lineRendererToCursorObj.transform.localRotation = Quaternion.identity;
        lineRendererToCursorObj.transform.localScale = Vector3.one;
        lineRendererToCursorObj.positionCount = 0;
        lineRendererToCursorObj.startWidth = 0.1f;
        lineRendererToCursorObj.endWidth = 0.1f;
        lineRendererToCursorObj.material = new Material(Shader.Find("Sprites/Default"));
        
        GameObject lr2 = new GameObject("LineRendererToGroundObj");
        lineRendererToGroundObj = lr2.AddComponent<LineRenderer>();
        lineRendererToGroundObj.transform.SetParent(playerObj.transform);
        lineRendererToGroundObj.transform.localPosition = Vector3.zero;
        lineRendererToGroundObj.transform.localRotation = Quaternion.identity;
        lineRendererToGroundObj.transform.localScale = Vector3.one;
        lineRendererToGroundObj.positionCount = 0;
        lineRendererToGroundObj.startWidth = 0.1f;
        lineRendererToGroundObj.endWidth = 0.1f;
        lineRendererToGroundObj.material = new Material(Shader.Find("Sprites/Default"));

        ContinueAim(playerObj, joystickData);
    }

    private void ContinueAim(GameObject playerObj, JoystickData joystickData) {
        float aimerHeight = playerObj.transform.position.y + playerObj.transform.localScale.y;
        
        // Make cursor start a few units forward of player
        Vector3 cursorPosition = Vector3.zero;
        Vector3 offset = VectorUtil.NewPointInDirection(lineRendererToCursorObj.gameObject, 8);
        offset.y = lineRendererToCursorObj.gameObject.transform.position.y;
        Vector3 target = joystickData.AroundPlayerPosition + VectorUtil.toLocalPosition(lineRendererToCursorObj.gameObject, offset);
        
        // Shoot ray from screen to AroundPlayerPosition to detect what is under cursor (ground, player, wall, etc)
        Vector3 origin = Camera.main.transform.position;
        Vector3 direction = (target - origin).normalized;
        Ray ray = new Ray(origin, direction);
        string gameObjectHit = "";
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f)) {
            cursorPosition = hit.point;
            gameObjectHit = hit.transform.name;
        }

        // Make CursorObj follow the touch hit point so player can visualize it
        cursorObj.transform.position = cursorPosition;

        // If aiming at ground, lift cursor hit position so that player can make head shots
        if (gameObjectHit == Constants.GROUND) {
            cursorPosition.y = 2;
        }

        // Line from aimer to cursor
        Vector3 aimerStartPoint = new Vector3(playerObj.transform.position.x, aimerHeight, playerObj.transform.position.z);
        Vector3 aimerEndPoint = AimerToCursorRay(aimerStartPoint, cursorPosition);
        lineRendererToCursorObj.positionCount = 2;
        lineRendererToCursorObj.SetPosition(0, aimerStartPoint);
        lineRendererToCursorObj.SetPosition(1, aimerEndPoint);
            
        // Line from cursor to ground
        Vector3 lr2AimerStartPoint = cursorPosition;
        Vector3 lr2AimerEndPoint = new Vector3(cursorPosition.x, -100, cursorPosition.z);
        lineRendererToGroundObj.positionCount = 2;
        lineRendererToGroundObj.SetPosition(0, lr2AimerStartPoint);
        lineRendererToGroundObj.SetPosition(1, lr2AimerEndPoint);
    }

    private Vector3 AimerToCursorRay(Vector3 aimerStartPoint, Vector3 cursorPosition) {
        Vector3 endPoint = cursorPosition;
        
        // Vector3 origin = lineRendererObj.transform.position;
        Vector3 origin = aimerStartPoint;
        Vector3 direction = (cursorPosition - aimerStartPoint).normalized;

        Ray ray = new Ray(origin, direction);

        // Draw debug ray (white = full ray, red = hit point)
        // Debug.DrawLine(origin, origin + direction * 1000f, Color.white); // full cast range

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f)) {
            endPoint = hit.point;

            // Draw hit point ray
            Debug.DrawLine(origin, endPoint, Color.red);
        }

        return endPoint;
    }
    
    private void FinishAim() {
        Vector3 start = lineRendererToCursorObj.GetPosition(0);
        Vector3 end = lineRendererToCursorObj.GetPosition(1);
        Quaternion spawnRotation = Quaternion.LookRotation((end - start).normalized);
        Arrow arrow = Instantiate(GameObjects.ArrowPrefab, start, spawnRotation).GetComponent<Arrow>();
        
        Destroy(cursorObj);
        Destroy(lineRendererToCursorObj);
        Destroy(lineRendererToGroundObj);
        arrow.Shoot(new List<Vector3>() {start, end}, 25, 5);
    }

    private void CancelAim() {
        Debug.Log("CancelAim");
        Destroy(cursorObj);
        Destroy(lineRendererToCursorObj);
        Destroy(lineRendererToGroundObj);
    }
}
}