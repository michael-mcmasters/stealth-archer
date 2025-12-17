using System.Collections.Generic;
using _stealthArcher.scripts.constants;
using _stealthArcher.scripts.models;
using DigitalRubyShared;
using UnityEngine;
using TouchPhase = UnityEngine.TouchPhase;

namespace _stealthArcher.scripts.weaponPrefabs {
public class Bow : IWeapon {

    private Trackpad trackpad;

    private bool inputActive;

    [Header("Aiming")]
    private LineRenderer aimerLineRendererObj;
    private Material aimerLineRendererInitialMaterial;
    [SerializeField] private Material aimerLineRendererCancelMaterial;
    
    [Header("Shooting")]
    [SerializeField] private float arrowSpeed;
    [SerializeField] private float damage;

    private AimData aimData;
    
    private GameObject playerObj;


    void Start() {
        this.playerObj = GameObjects.Player;
        this.trackpad = new Trackpad(SpecificTouch.Right, (joystickEvent, joystickData) => {
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
        });
    }

    public override void HandleInput() {
        trackpad?.HandleInput();
    }

    private void BeginAim() {
        GameObject lr1 = new GameObject("LineRendererToCursorObj");
        aimerLineRendererObj = lr1.AddComponent<LineRenderer>();
        aimerLineRendererObj.transform.SetParent(playerObj.transform);
        aimerLineRendererObj.transform.localPosition = Vector3.zero;
        aimerLineRendererObj.transform.localRotation = Quaternion.identity;
        aimerLineRendererObj.transform.localScale = Vector3.one;
        aimerLineRendererObj.positionCount = 0;
        aimerLineRendererObj.startWidth = 0.1f;
        aimerLineRendererObj.endWidth = 0.1f;
        aimerLineRendererObj.material = new Material(Shader.Find("Sprites/Default"));
        
        aimerLineRendererInitialMaterial = aimerLineRendererObj.GetComponent<Renderer>().material;
    }

    private void ContinueAim(GameObject playerObj, Vector3 joystickPosition) {
        aimerLineRendererObj.GetComponent<Renderer>().material = aimerLineRendererInitialMaterial;
        
        float distance = Vector3.Distance(playerObj.transform.position, joystickPosition) * 0.5f;
        Vector3 midPoint = VectorUtil.NewPointInTargetsDirection(playerObj.transform.position, joystickPosition, distance);
        midPoint.y = playerObj.transform.position.y + 2;
        
        List<Vector3> bezierPath = BezierCurveGenerator.CreateBezierCurve(playerObj.transform.position, midPoint, joystickPosition);
        aimerLineRendererObj.positionCount = bezierPath.Count;
        for (int i = 0; i < bezierPath.Count; i++) {
            aimerLineRendererObj.SetPosition(i, bezierPath[i]);
        }
        
        this.aimData = new AimData(bezierPath);
    }
    

    private void HandleInCancelRange() {
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
        Arrow arrow = Instantiate(GameObjects.ArrowPrefab, spawnPosition, spawnRotation).GetComponent<Arrow>();
        
        Destroy(aimerLineRendererObj);
        arrow.Shoot(aimData.pathPoints, arrowSpeed, damage);
    }
    
}
}