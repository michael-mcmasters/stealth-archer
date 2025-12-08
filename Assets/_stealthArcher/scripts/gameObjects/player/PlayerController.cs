using System.Collections;
using System.Collections.Generic;
using _stealthArcher;
using _stealthArcher.scripts;
using _stealthArcher.scripts.constants;
using _stealthArcher.scripts.models;
using _stealthArcher.scripts.weaponPrefabs;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerMovementHandler))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private PlayerMovementHandler playerMovementHandler;
    private IWeapon selectedWeapon;

    private bool debug_whistle;
    
    
    void Start() {
        this.playerMovementHandler = GetComponent<PlayerMovementHandler>();
        this.selectedWeapon = Instantiate(GameObjects.HeavyBowPrefab).GetComponent<IWeapon>();
    }

    void Update() {
        if (Input.GetKey(KeyCode.Alpha1)) {
            SelectWeapon(GameObjects.BowPrefab);
        }
        if (Input.GetKey(KeyCode.Alpha2)) {
            SelectWeapon(GameObjects.BoomerangPrefab);
        }
        if (Input.GetKey(KeyCode.Alpha3)) {
            SelectWeapon(GameObjects.HeavyBowPrefab);
        }

        selectedWeapon.HandleInput();
        playerMovementHandler.HandleMovement();
    }

    public void OnBowButton() {
        SelectWeapon(GameObjects.BowPrefab);
    }

    public void OnHeavyBowButton() {
        SelectWeapon(GameObjects.HeavyBowPrefab);
    }
    
    public void OnBoomerangeButton() {
        SelectWeapon(GameObjects.BoomerangPrefab);
    }
    
    public void OnWhistleButton() {
        int whistleRange = 6;

        HashSet<Enemy> enemiesWhistledAt = new HashSet<Enemy>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, whistleRange, LayerMasks.Enemy);
        foreach (Collider c in colliders) {
            Enemy enemy = c.name.Contains(Constants.ENEMY)
                ? c.gameObject.GetComponent<Enemy>()
                : c.GetComponentInParent<Enemy>();

            if (!enemiesWhistledAt.Contains(enemy)) {
                enemy.OnHearWhistle();
                enemiesWhistledAt.Add(enemy);
            }
        }
        
        if (debug_whistle) {
            DebugUtil.DrawSphere(transform.position, Color.cyan, whistleRange, 10);
        }
    }

    private void SelectWeapon(GameObject weaponPrefab) {
        Destroy(selectedWeapon.gameObject);
        selectedWeapon = Instantiate(weaponPrefab).GetComponent<IWeapon>();
    }
    
}
