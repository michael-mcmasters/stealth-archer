using System.Collections.Generic;
using _stealthArcher.scripts.constants;
using _stealthArcher.scripts.models;
using DigitalRubyShared;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using TouchPhase = UnityEngine.TouchPhase;

namespace _stealthArcher.scripts.weaponPrefabs {
public class Bow : IWeapon {

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
    
    [SerializeField] private float arrowSpeed = 10;
    [SerializeField] private float damage = 5;


    void Start() {
        this.playerObj = GameObjects.Player;
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

        currentAimerWidth = initialAimerWidth;
    }

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
        RaycastHit[] detectedEnemies = detectEnemies();
        Collider targetEnemy = chooseEnemyToAimAt(detectedEnemies);

        if (targetEnemy != null) {
            aimerObj.transform.LookAt(targetEnemy.transform);
            HighlightTargetEnemy(targetEnemy);
            
        } else {
            aimerObj.transform.rotation = joystickData.Rotation;
        }
        
        // Quaternion prevRotation = aimerObj.transform.rotation;
        // aimerObj.transform.rotation = Quaternion.Euler(aimerObj.transform.rotation.x, prevRotation.y, prevRotation.z);
        
        // aimerObj.transform.rotation = joystickData.Rotation;
        // Vector3 euler = aimerObj.transform.rotation.eulerAngles;
        // euler.x = 17.63f;   // aim it down
        // aimerObj.transform.rotation = Quaternion.Euler(euler);
        
        // aimerObj.transform.rotation = joystickData.Rotation;
        // Vector3 toTarget = targetEnemy.bounds.center - aimerObj.transform.position;
        // Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);
        // float angleDown = Vector3.SignedAngle(toTargetXZ, toTarget, aimerObj.transform.right);
        // aimerObj.transform.Rotate(angleDown, 0f, 0f, Space.Self);
    }
    
    private void HighlightTargetEnemy(Collider targetEnemy) {
        Vector3 indicatorPosition = targetEnemy.transform.position;
        indicatorPosition.y = targetEnemy.transform.localScale.y + 1;
        DebugExtension.DebugWireSphere(indicatorPosition, Color.magenta, 0.2f);
    }

    private RaycastHit[] detectEnemies() {
        float width = 0.5f;
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
        Collider targetCollider = null;

        float closestXToCenter = Mathf.Infinity;
        foreach (RaycastHit hit in detectedEnemies) {
            float xPositionLp = VectorUtil.toLocalPosition(aimerObj, hit.collider.transform.position).x;
            float distanceFromCenter = Mathf.Abs(xPositionLp);

            if (distanceFromCenter < closestXToCenter) {
                closestXToCenter = distanceFromCenter;
                targetCollider = hit.collider;
            }
        }

        return targetCollider;
    }
    
    private void Shoot() {
        Vector3 start = aimerObj.transform.position;
        Vector3 end = VectorUtil.NewPointInDirection(aimerObj, 100);
        Quaternion spawnRotation = Quaternion.LookRotation((end - start).normalized);
        Arrow arrow = Instantiate(GameObjects.ArrowPrefab, start, spawnRotation).GetComponent<Arrow>();
        
        Destroy(aimerObj);
        arrow.Shoot(new List<Vector3>() {start, end}, arrowSpeed, damage);
    }
    
    private void CancelAim() {
        Destroy(aimerObj);
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