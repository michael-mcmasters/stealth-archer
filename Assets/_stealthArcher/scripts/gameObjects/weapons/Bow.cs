using System.Collections.Generic;
using _stealthArcher.scripts.constants;
using _stealthArcher.scripts.models;
using DigitalRubyShared;
using UnityEngine;
using TouchPhase = UnityEngine.TouchPhase;

namespace _stealthArcher.scripts.weaponPrefabs {
public class Bow : IWeapon {

    // private Trackpad joystick;
    private Joystick joystick;

    [Header("Aiming")]
    [SerializeField] private LineRenderer aimerLineRendererPrefab;
    private LineRenderer aimerLineRendererObj;
    private Material aimerLineRendererInitialMaterial;
    [SerializeField] private Material aimerLineRendererCancelMaterial;
    
    [Header("Shooting")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float arrowSpeed;
    [SerializeField] private float damage;

    private AimData aimData;
    
    private GameObject playerObj;


    void Start() {
        this.joystick = new Joystick(SpecificTouch.Right, Callback);
        this.playerObj = GameObjects.Player;
    }

    public override void HandleInput() {
        joystick?.HandleInput();
    }

    private void Callback(JoystickEvent joystickEvent, JoystickData joystickData) {
        switch (joystickEvent) {
            case JoystickEvent.Down:
                BeginAim();
                break;
            case JoystickEvent.Hold:
                ContinueAim(playerObj, joystickData.AroundPlayerPosition);
                break;
            case JoystickEvent.Up:
                AimData aimData = FinishAim();
                Shoot(aimData);
                break;
            case JoystickEvent.InCancelRange:
                ContinueAim(playerObj, joystickData.AroundPlayerPosition);
                HandleInCancelRange();
                break;
            case JoystickEvent.Cancel:
                CancelAim();
                break;
        }
    }
    
    private void BeginAim() {
        aimerLineRendererObj = Instantiate(aimerLineRendererPrefab, playerObj.transform.position, Quaternion.identity);
        aimerLineRendererInitialMaterial = aimerLineRendererObj.GetComponent<Renderer>().material;
    }

    // private void ContinueAim(GameObject playerObj, Vector3 joystickPosition) {
    //     aimerLineRendererObj.GetComponent<Renderer>().material = aimerLineRendererInitialMaterial;
    //     
    //     float distance = Vector3.Distance(playerObj.transform.position, joystickPosition) * 0.5f;
    //     Vector3 midPoint = VectorUtil.NewPointInTargetsDirection(playerObj.transform.position, joystickPosition, distance);
    //     midPoint.y = playerObj.transform.position.y + 5;
    //     
    //     List<Vector3> bezierPath = BezierCurveGenerator.CreateBezierCurve(playerObj.transform.position, midPoint, joystickPosition);
    //     
    //     aimerLineRendererObj.positionCount = bezierPath.Count;
    //     for (int i = 0; i < bezierPath.Count; i++) {
    //         aimerLineRendererObj.SetPosition(i, bezierPath[i]);
    //     }
    //     
    //     this.aimData = new AimData(bezierPath);
    // }

    private void ContinueAim(GameObject playerObj, Vector3 aroundPlayerPosition) {
        aimerLineRendererObj.GetComponent<Renderer>().material = aimerLineRendererInitialMaterial;

        float aimerHeight = playerObj.transform.position.y + playerObj.transform.localScale.y;
        
        float distance = 100f;
        Vector3 startPoint = playerObj.transform.position;
        startPoint.y = aimerHeight;
        aroundPlayerPosition.y = aimerHeight;
        
        Vector3 endPoint = VectorUtil.NewPointInTargetsDirection(startPoint, aroundPlayerPosition, distance);
        
        aimerLineRendererObj.positionCount = 2;
        aimerLineRendererObj.SetPosition(0, startPoint);
        aimerLineRendererObj.SetPosition(1, endPoint);
        
        this.aimData = new AimData(new List<Vector3>() {playerObj.transform.position, endPoint});
    }
    
    private void HandleInCancelRange() {
        // aimerLineRendererObj.positionCount = 0;
        aimerLineRendererObj.GetComponent<Renderer>().material = aimerLineRendererCancelMaterial;
    }

    private void CancelAim() {
        Destroy(aimerLineRendererObj);
    }
    
    private AimData FinishAim() {
        Destroy(aimerLineRendererObj);
        return aimData;
    }

    private void Shoot(AimData aimData) {
        Vector3 spawnPosition = aimData.pathPoints[0];
        Quaternion spawnRotation = Quaternion.LookRotation((aimData.pathPoints[1] - spawnPosition).normalized);
        Arrow arrow = Instantiate(arrowPrefab, spawnPosition, spawnRotation).GetComponent<Arrow>();
        
        Destroy(aimerLineRendererObj);
        arrow.Shoot(aimData.pathPoints, arrowSpeed, damage);
    }
    
}
}