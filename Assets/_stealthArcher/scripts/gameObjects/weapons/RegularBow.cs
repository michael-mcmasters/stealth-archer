using System;
using System.Collections.Generic;
using System.Linq;
using _stealthArcher.scripts.constants;
using _stealthArcher.scripts.models;
using DigitalRubyShared;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using TouchPhase = UnityEngine.TouchPhase;

namespace _stealthArcher.scripts.weaponPrefabs {
public class RegularBow : IWeapon {

    // private Trackpad trackpad;
    private Joystick trackpad;
    
    private GameObject playerObj;
    
    private GameObject aimerObj;
    private Mesh aimerMesh;
    private GameObject enemyAimIndicatorObj;
    private Collider targetEnemy;

    private Vector3 tapLastFramePosition;
    
    private float initialAimerWidth = 0.8f;
    private float currentAimerWidth = 0;
    [SerializeField] private float widthDecreaseSpeed = 0.005f;
    private float minAllowedWidth = 0.1f;
    
    [SerializeField] private float arrowSpeed = 10;
    [SerializeField] private float damage = 5;


    void Start() {
        this.playerObj = GameObjects.Player;
        this.trackpad = new Joystick(SpecificTouch.Right, (joystickEvent, joystickData) => {
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
        
        enemyAimIndicatorObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        enemyAimIndicatorObj.transform.localScale = Vector3.one;
        var mat = new Material(Shader.Find("Standard"));
        mat.color = Color.yellow;
        enemyAimIndicatorObj.GetComponent<Renderer>().material = mat;
    }

    private void ContinueAim(GameObject playerObj, JoystickData joystickData) {
        // Move aimer to player
        float aimerHeight = playerObj.transform.position.y + playerObj.transform.localScale.y;
        aimerObj.transform.position = new Vector3(playerObj.transform.position.x, aimerHeight, playerObj.transform.position.z);
        
        // Shrink aimer as it's held down
        if (currentAimerWidth > minAllowedWidth) {
            currentAimerWidth -= widthDecreaseSpeed;
        }
        int length = 3;
        SetAimerMesh(
            new Vector3(0, 0, 0),
            new Vector3(-currentAimerWidth, 0, length),
            new Vector3(currentAimerWidth, 0, length)
        );
        
        // Make aimer rotate with finger
        aimerObj.transform.rotation = joystickData.Rotation;
        
        // Detect and highlight an enemy being aimed at (if any)
        RaycastHit[] detectedEnemies = detectEnemies();
        targetEnemy = chooseEnemyToAimAt(detectedEnemies);
        if (targetEnemy != null) {
            enemyAimIndicatorObj.transform.position = new Vector3(targetEnemy.transform.position.x, targetEnemy.transform.position.y + (targetEnemy.transform.localScale.y * 0.5f), targetEnemy.transform.position.z);
        }
        
        // Rotate camera with touch - Use touch.x position (on screen) to determine how much to rotate camera
        // GameObject cameraAnchor = GameObjects.CameraAnchor;
        // if (tapLastFramePosition != null) {
        //     float yRotation = joystickData.tapCurrentPosition.x - tapLastFramePosition.x;
        //     Vector3 euler = cameraAnchor.transform.eulerAngles;
        //     euler.y += yRotation;
        //     cameraAnchor.transform.eulerAngles = euler;
        //
        //     tapLastFramePosition = joystickData.tapCurrentPosition;
        // }
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
        if (targetEnemy) {
            // Shoot at enemy
            Vector3 start = aimerObj.transform.position;
            Transform head = targetEnemy.transform.parent.Cast<Transform>().FirstOrDefault(t => t.name == Constants.HEAD);
            if (head == null) {
                throw new Exception("Did not find 'Head' gameobject as a child of 'Enemy' gameobject");
            }
            Vector3 end = head.transform.position;
            Quaternion spawnRotation = Quaternion.LookRotation((end - start).normalized);
            Arrow arrow = Instantiate(GameObjects.ArrowPrefab, start, spawnRotation).GetComponent<Arrow>();
            arrow.Shoot(new List<Vector3>() {start, end}, arrowSpeed, damage);
        }
        else {
            // Just shoot forward
            Vector3 start = aimerObj.transform.position;
            Vector3 end = VectorUtil.NewPointInDirection(aimerObj, 100);
            Quaternion spawnRotation = Quaternion.LookRotation((end - start).normalized);
            Arrow arrow = Instantiate(GameObjects.ArrowPrefab, start, spawnRotation).GetComponent<Arrow>();
            arrow.Shoot(new List<Vector3>() {start, end}, arrowSpeed, damage);
        }
        
        Destroy(aimerObj);
        Destroy(enemyAimIndicatorObj);
    }
    
    private void CancelAim() {
        Destroy(aimerObj);
        Destroy(enemyAimIndicatorObj);
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