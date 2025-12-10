using System.Collections.Generic;
using _stealthArcher.scripts.constants;
using _stealthArcher.scripts.models;
using DigitalRubyShared;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using TouchPhase = UnityEngine.TouchPhase;

namespace _stealthArcher.scripts.weaponPrefabs {
public class HeavyBow : IWeapon {

    private Trackpad trackpad;
    // private Joystick joystick;
    
    private GameObject playerObj;
    // private float aimerHeight = 2f;
    
    private GameObject aimerObj;
    private Mesh aimerMesh;

    private float initialAimerWidth = 0.8f;
    private float currentAimerWidth = 0;
    [SerializeField] private float widthDecreaseSpeed = 0.005f;
    private float minAllowedWidth = 0.1f;
    
    [SerializeField] private float arrowSpeed;
    [SerializeField] private float damage;

    private Collider lastEnemyCollider;
    private Vector3 lockedOnTouchGroundPosition;
    private LineRenderer lineRendererObj;


    void Start() {
        this.playerObj = GameObjects.Player;
        // this.trackpad = new Trackpad(SpecificTouch.Right, (joystickEvent, joystickData) => {
        //     switch (joystickEvent) {
        //         case JoystickEvent.Down:
        //             BeginAim();
        //             break;
        //         case JoystickEvent.Hold:
        //             ContinueAim(playerObj, joystickData);
        //             break;
        //         case JoystickEvent.Up:
        //             Shoot();
        //             break;
        //         case JoystickEvent.InCancelRange:
        //             ContinueAim(playerObj, joystickData);
        //             // HandleInCancelRange();       // Make red to indicate you're about to cancel
        //             break;
        //         case JoystickEvent.Cancel:
        //             CancelAim();
        //             break;
        //     }
        // });
        this.trackpad = new Trackpad(SpecificTouch.Right, (joystickEvent, joystickData) => {
            switch (joystickEvent) {
                case JoystickEvent.Down:
                    BeginAim();
                    break;
                case JoystickEvent.Hold:
                    ContinueAim(playerObj, joystickData);
                    break;
                case JoystickEvent.Up:
                    Shoot();
                    break;
                case JoystickEvent.InCancelRange:
                    ContinueAim(playerObj, joystickData);
                    // HandleInCancelRange();       // Make red to indicate you're about to cancel
                    break;
                case JoystickEvent.Cancel:
                    CancelAim();
                    break;
            }
        });
    }
    
    public override void HandleInput() {
        trackpad?.HandleInput();
    }
    
    private void BeginAim() {
        aimerObj = new GameObject("Aimer");
        MeshFilter mf = aimerObj.AddComponent<MeshFilter>();
        MeshRenderer mr = aimerObj.AddComponent<MeshRenderer>();
        aimerMesh = new Mesh();
        mf.mesh = aimerMesh;
        mr.material = new Material(Shader.Find("Standard"));
        
        // Create Line Renderer
        GameObject lineObj = new GameObject("DynamicLineRenderer");
        lineRendererObj = lineObj.AddComponent<LineRenderer>();
        lineRendererObj.positionCount = 0;
        lineRendererObj.startWidth = 0.1f;
        lineRendererObj.endWidth = 0.1f;
        lineRendererObj.material = new Material(Shader.Find("Sprites/Default"));

        currentAimerWidth = initialAimerWidth;
    }

    // private void ContinueAim(GameObject playerObj, JoystickData joystickData) {
    //     float aimerHeight = playerObj.transform.position.y + playerObj.transform.localScale.y;
    //     aimerObj.transform.position = new Vector3(playerObj.transform.position.x, aimerHeight, playerObj.transform.position.z);
    //     
    //     if (currentAimerWidth > minAllowedWidth) {
    //         currentAimerWidth -= widthDecreaseSpeed;
    //     }
    //     
    //     int length = 3;
    //     SetAimerMesh(
    //         new Vector3(0, 0, 0),
    //         new Vector3(-currentAimerWidth, 0, length),
    //         new Vector3(currentAimerWidth, 0, length)
    //     );
    //     
    //     aimerObj.transform.rotation = aimerObj.transform.rotation = joystickData.Rotation;
    //     checkEnemies(playerObj, joystickData);
    //
    //     // aimerObj.transform.rotation = joystickData.Rotation;
    //     // Vector3 euler = aimerObj.transform.rotation.eulerAngles;
    //     // euler.x = 17.63f;   // aim it down
    //     // aimerObj.transform.rotation = Quaternion.Euler(euler);
    // }

    // private void ContinueAim(GameObject playerObj, JoystickData joystickData) {
    //     float aimerHeight = playerObj.transform.position.y + playerObj.transform.localScale.y;
    //     aimerObj.transform.position = new Vector3(playerObj.transform.position.x, aimerHeight, playerObj.transform.position.z);
    //     
    //     if (currentAimerWidth > minAllowedWidth) {
    //         currentAimerWidth -= widthDecreaseSpeed;
    //     }
    //     
    //     int length = 3;
    //     SetAimerMesh(
    //         new Vector3(0, 0, 0),
    //         new Vector3(-currentAimerWidth, 0, length),
    //         new Vector3(currentAimerWidth, 0, length)
    //     );
    //     
    //     aimerObj.transform.rotation = aimerObj.transform.rotation = joystickData.Rotation;
    //     
    //     RaycastHit[] detectedEnemies = detectEnemies(playerObj, joystickData);
    //     Collider enemy = chooseEnemyToAimAt(detectedEnemies);
    //
    //     aimerObj.transform.LookAt(enemy.transform);
    // }
    
    private void ContinueAim(GameObject playerObj, JoystickData joystickData) {
        float aimerHeight = playerObj.transform.position.y + playerObj.transform.localScale.y;
        aimerObj.transform.position = new Vector3(playerObj.transform.position.x, aimerHeight, playerObj.transform.position.z);
        
        if (currentAimerWidth > minAllowedWidth) {
            currentAimerWidth -= widthDecreaseSpeed;
        }
        
        int length = 3;
        SetAimerMesh(
            new Vector3(0, 0, 0),
            new Vector3(-currentAimerWidth, 0, length),
            new Vector3(currentAimerWidth, 0, length)
        );
        
        aimerObj.transform.rotation = joystickData.Rotation;
        
        RaycastHit[] detectedEnemies = detectEnemies(playerObj, joystickData);
        if (currentAimerWidth < 0.5f && detectedEnemies.Length > 0) {
            Collider enemyCollider = chooseEnemyToAimAt(detectedEnemies);
            if (enemyCollider != lastEnemyCollider) {
                lastEnemyCollider = enemyCollider;
                lockedOnTouchGroundPosition = joystickData.AroundPlayerPosition;
            
                // Look at collider
                // aimerObj.transform.LookAt(enemyCollider.transform);
            }
        
            // Player aims up/down
            // get initial and current as local pos to play
            // take z axis to determine how far up/down has moved
            Vector3 touchDownLp = VectorUtil.toLocalPosition(playerObj, lockedOnTouchGroundPosition);
            Vector3 touchCurrentLp = VectorUtil.toLocalPosition(playerObj, joystickData.AroundPlayerPosition);
            float zDifference = touchDownLp.z - touchCurrentLp.z;

            // Move aimer up/down
            aimerObj.transform.LookAt(enemyCollider.transform);
            Vector3 euler = aimerObj.transform.rotation.eulerAngles;
            euler.x += zDifference;
            aimerObj.transform.rotation = Quaternion.Euler(euler);
        
            // Visualize with a Line
            Vector3 origin = aimerObj.transform.position;
            Vector3 direction = aimerObj.transform.forward;
            Ray ray = new Ray(origin, direction);
            Vector3 rayEndPoint = Vector3.zero;
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f)) {
                rayEndPoint = hit.point;
            }
        
            lineRendererObj.positionCount = 2;
            lineRendererObj.SetPosition(0, aimerObj.transform.position);
            lineRendererObj.SetPosition(1, rayEndPoint);
        }
        else {
            lineRendererObj.positionCount = 0;
            aimerObj.transform.rotation = aimerObj.transform.rotation = joystickData.Rotation;
        }
        
    }
    
    private RaycastHit[] detectEnemies(GameObject playerObj, JoystickData joystickData) {
        float width = 0.05f;
        float height = 20;
        float range = 100f;
        
        Vector3 origin = aimerObj.transform.position;
        Vector3 forward = aimerObj.transform.forward;
        Vector3 halfExtents = new Vector3(width, height, 0.5f);     // z doesn't matter here because the box 'slides' from origin, forward.

        RaycastHit[] detectedEnemies = Physics.BoxCastAll(origin, halfExtents, forward, Quaternion.identity, range, LayerMasks.DetectPlayerAimer);
        foreach (RaycastHit hit in detectedEnemies) {
            // Debug.Log(hit.collider.name);
        }

        bool debug = true;
        if (debug) {
            // Left side of box
            Debug.DrawLine(
                VectorUtil.toWorldPosition(aimerObj, new Vector3(-width, 0, 0)),
                VectorUtil.toWorldPosition(aimerObj, new Vector3(-width, 0, range)),
                Color.red
            );
            
            // Right side of box
            Debug.DrawLine(
                VectorUtil.toWorldPosition(aimerObj, new Vector3(width, 0, 0)),
                VectorUtil.toWorldPosition(aimerObj, new Vector3(width, 0, range)),
                Color.red
            );
        }

        return detectedEnemies;
    }

    private Collider chooseEnemyToAimAt(RaycastHit[] detectedEnemies) {
        return detectedEnemies[0].collider;
    }
    
    private void Shoot() {
        Vector3 start = aimerObj.transform.position;
        Vector3 end = VectorUtil.NewPointInDirection(aimerObj, 100);
        Quaternion spawnRotation = Quaternion.LookRotation((end - start).normalized);
        Arrow arrow = Instantiate(GameObjects.ArrowPrefab, start, spawnRotation).GetComponent<Arrow>();
        
        Destroy(aimerObj);
        Destroy(lineRendererObj);
        arrow.Shoot(new List<Vector3>() {start, end}, arrowSpeed, damage);
    }
    
    private void CancelAim() {
        Destroy(aimerObj);
        Destroy(lineRendererObj);
    }
    
    public void SetAimerMesh(Vector3 start, Vector3 topLeft, Vector3 topRight) {
        Vector3[] vertices = new Vector3[3];
        int[] triangles = new int[3];

        // Assign vertices
        vertices[0] = start;
        vertices[1] = topLeft;
        vertices[2] = topRight;

        // Define triangle (clockwise or counter-clockwise)
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        // Apply to mesh
        aimerMesh.Clear();
        aimerMesh.vertices = vertices;
        aimerMesh.triangles = triangles;

        // Recalculate for lighting & visibility
        aimerMesh.RecalculateNormals();
        aimerMesh.RecalculateBounds();
    }
    
}
}