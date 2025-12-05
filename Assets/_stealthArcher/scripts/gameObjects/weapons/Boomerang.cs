using System.Collections.Generic;
using _stealthArcher.scripts.constants;
using _stealthArcher.scripts.models;
using DigitalRubyShared;
using UnityEngine;
using TouchPhase = UnityEngine.TouchPhase;

namespace _stealthArcher.scripts.weaponPrefabs {
public class Boomerang : IWeapon {

    private Trackpad trackpad;

    private bool inputActive;

    [Header("Aiming")]
    [SerializeField] private LineRenderer aimerLineRendererPrefab;
    private LineRenderer aimerLineRendererObj;
    [SerializeField] private GameObject boomerangeAimGroundVisualizerPrefab;
    private GameObject boomerangeAimGroundVisualizerObj;
    
    [Header("Shooting")]
    [SerializeField] private GameObject boomerangPrefab;
    [SerializeField] private float speed;
    [SerializeField] private float damage;

    private List<Vector3> trackpadPoints;
    
    private AimData aimData;
    
    private GameObject playerObj;


    void Start() {
        this.trackpad = new Trackpad(SpecificTouch.Right, Callback);
        this.playerObj = GameObjects.Player;
        this.trackpadPoints = new List<Vector3>();
    }

    public override void HandleInput() {
        trackpad?.HandleInput();
    }

    public void Callback(JoystickEvent joystickEvent, JoystickData joystickData) {
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
        }
    }
    
    private void BeginAim() {
        aimerLineRendererObj = Instantiate(aimerLineRendererPrefab, playerObj.transform.position, Quaternion.identity);
        boomerangeAimGroundVisualizerObj = Instantiate(boomerangeAimGroundVisualizerPrefab, playerObj.transform.position, Quaternion.identity);
        this.trackpadPoints = new List<Vector3>();
    }

    private void ContinueAim(GameObject playerObj, Vector3 joystickPosition) {
        joystickPosition.y += playerObj.transform.position.y;
        if (trackpadPoints.Count > 0 && Vector3.Distance(trackpadPoints[^1], joystickPosition) < 0.1f) return;
        
        trackpadPoints.Add(joystickPosition);

        aimerLineRendererObj.positionCount = trackpadPoints.Count;
        for (int i = 0; i < trackpadPoints.Count; i++) {
            aimerLineRendererObj.SetPosition(i, trackpadPoints[i]);
        }
        
        joystickPosition.y -= playerObj.transform.localScale.y * 0.5f;
        boomerangeAimGroundVisualizerObj.transform.position = joystickPosition;
        
        this.aimData = new AimData(new List<Vector3>(trackpadPoints));
    }
    
    private AimData FinishAim() {
        Destroy(aimerLineRendererObj.gameObject);
        Destroy(boomerangeAimGroundVisualizerObj);
        trackpadPoints.Clear();
        return aimData;
    }

    private void Shoot(AimData aimData) {
        Vector3 spawnPosition = aimData.pathPoints[0];
        Quaternion spawnRotation = Quaternion.LookRotation((aimData.pathPoints[1] - spawnPosition).normalized);
        BoomerangProjectile projectile = Instantiate(boomerangPrefab, spawnPosition, spawnRotation).GetComponent<BoomerangProjectile>();

        projectile.Shoot(aimData.pathPoints, speed, damage);
    }
    
}
}