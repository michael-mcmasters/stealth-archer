using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using _stealthArcher.scripts.constants;
using _stealthArcher.scripts.gameObjects.headshotText;
using _stealthArcher.scripts.input;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour {

    [SerializeField] private float health;

    private Rigidbody rigidbody;

    private Vector3 targetPosition;

    [SerializeField] private GameObject attackIconObj;
    

    void Start() {
        GetComponent<Renderer>().material.color = Color.green;
        rigidbody = GetComponent<Rigidbody>();
        
        GetComponent<OnTapHandler>().OnTapCallback = OnTap;
    }

    void Update() {
        // rigidBody.velocity = targetPosition;
        if (targetPosition != Vector3.zero) {
            rigidbody.MovePosition(Vector3.MoveTowards(rigidbody.position, targetPosition, 1 * Time.deltaTime));
        }
    }
    
    public void TakeDamage(float damage, bool headshot) {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddExplosionForce(10000, rigidbody.position, 10);
        
        health -= damage;
        if (headshot) {
            health = 0;
        }

        
        if (health <= 15) {
            GetComponent<Renderer>().material.color = Color.yellow;
        }
        if (health <= 10) {
            GetComponent<Renderer>().material.color = Colors.Orange;
        }
        if (health <= 5) {
            GetComponent<Renderer>().material.color = Color.red;
        }
        if (health <= 0) {
            HeadshotText.Instance.Activate(this);
            Destroy(this.gameObject);
        }
    }

    public void OnHearWhistle() {
        Debug.Log("Heard Whistle");
        targetPosition = GameObjects.Player.transform.position;
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.Equals(GameObjects.Player)) {
            attackIconObj.SetActive(true);
        }
    }
    
    void OnTriggerExit(Collider col) {
        if (col.gameObject.Equals(GameObjects.Player)) {
            attackIconObj.SetActive(false);
        }
    }

    private void OnTap() {
        Debug.Log("OnTap called in Enemy!");

        if (attackIconObj.activeSelf) {
            TakeDamage(20, false);
        }
    }
    
}
