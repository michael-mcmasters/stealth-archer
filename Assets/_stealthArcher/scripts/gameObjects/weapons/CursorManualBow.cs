using System.Collections.Generic;
using _stealthArcher.scripts.constants;
using Unity.VisualScripting;
using UnityEngine;

namespace _stealthArcher.scripts.weaponPrefabs {

public class CursorManualBow : IWeapon {
    
    private Trackpad trackpad;
    
    private GameObject playerObj;
    private GameObject cursorObj;
    private LineRenderer lineRendererObj;

    // private GameObject enemy;
    
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
        
        // Create Line Renderer
        GameObject lineObj = new GameObject("DynamicLineRenderer");
        lineRendererObj = lineObj.AddComponent<LineRenderer>();
        lineRendererObj.transform.SetParent(playerObj.transform);
        lineRendererObj.transform.localPosition = Vector3.zero;
        lineRendererObj.transform.localRotation = Quaternion.identity;
        lineRendererObj.transform.localScale = Vector3.one;
        lineRendererObj.positionCount = 0;
        lineRendererObj.startWidth = 0.1f;
        lineRendererObj.endWidth = 0.1f;
        lineRendererObj.material = new Material(Shader.Find("Sprites/Default"));
        
        // Set Cursor point
        // Vector3 farPoint = VectorUtil.NewPointInDirection(playerObj.transform.position, playerObj.transform.forward, 10);
        // Vector3 farPointLp = VectorUtil.toLocalPosition(playerObj, farPoint);
        // initialPoint =  joystickData.AroundPlayerPosition + farPointLp;
        
        // GameObject[] enemies = GameObject.FindGameObjectsWithTag(Tags.Enemy);
        // float lowestDistance = float.PositiveInfinity;
        // foreach (GameObject e in enemies) {
        //     float distance = Vector3.Distance(playerObj.transform.position, e.transform.position);
        //     if (distance < lowestDistance) {
        //         lowestDistance = distance;
        //         enemy = e;
        //     }
        // }
        // if (enemies.Length > 0) {
        //     enemy = enemies[0];
        // }
        
        // RaycastHit[] detectedEnemies = detectEnemies();
        // if (detectedEnemies.Length > 0) {
        //     enemy = chooseEnemyToAimAt(detectedEnemies).gameObject;
        // }

        ContinueAim(playerObj, joystickData);
    }

    private void ContinueAim(GameObject playerObj, JoystickData joystickData) {
        // shoot ray from screen to cursor - get hit
        // shoot ray from aimerObj to hit
        
        float aimerHeight = playerObj.transform.position.y + playerObj.transform.localScale.y;
        
        // Shoot ray from screen to AroundPlayerPosition to detect what is under cursor
        Vector3 cursorPosition = Vector3.zero;
        // Vector3 enemyPosition = enemy.transform.position;
        Vector3 offset = VectorUtil.NewPointInDirection(lineRendererObj.gameObject, 8);
        offset.y = lineRendererObj.gameObject.transform.position.y;
        Vector3 target = joystickData.AroundPlayerPosition + VectorUtil.toLocalPosition(lineRendererObj.gameObject, offset);
        
        Vector3 origin = Camera.main.transform.position;
        Vector3 direction = (target - origin).normalized;
        Ray ray = new Ray(origin, direction);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f)) {
            cursorPosition = hit.point;
        }
        
        // Cursor follows touch
        cursorObj.transform.position = cursorPosition;

        // Line points to cursor
        Vector3 aimerStartPoint = new Vector3(playerObj.transform.position.x, aimerHeight, playerObj.transform.position.z);
        Vector3 aimerEndPoint = AimerToCursorRay(aimerStartPoint, cursorPosition);
        
        lineRendererObj.positionCount = 2;
        lineRendererObj.SetPosition(0, aimerStartPoint);
        lineRendererObj.SetPosition(1, aimerEndPoint);
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
        Vector3 start = lineRendererObj.GetPosition(0);
        Vector3 end = lineRendererObj.GetPosition(1);
        Quaternion spawnRotation = Quaternion.LookRotation((end - start).normalized);
        Arrow arrow = Instantiate(GameObjects.ArrowPrefab, start, spawnRotation).GetComponent<Arrow>();
        
        Destroy(cursorObj);
        Destroy(lineRendererObj);
        arrow.Shoot(new List<Vector3>() {start, end}, 25, 5);
    }

    private void CancelAim() {
        Debug.Log("CancelAim");
        Destroy(cursorObj);
        Destroy(lineRendererObj);
    }
}
}