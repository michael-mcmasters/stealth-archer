using System;
using System.Collections;
using System.Collections.Generic;
using _stealthArcher.scripts.constants;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour {

    private Rigidbody rigidbody;
    
    private float damage;

    private HashSet<Enemy> enemiesHit = new HashSet<Enemy>();

    private Coroutine traversePathCoroutine;
    

    void Start() {
        this.rigidbody = GetComponent<Rigidbody>();
    }
    
    public void Shoot(List<Vector3> bezierCurve, float speed, float damage) {
        this.damage = damage;
        this.traversePathCoroutine = StartCoroutine(nameof(TraversePath), new System.Object[] {bezierCurve, speed});
    }

    private IEnumerator TraversePath(System.Object[] param) {
        yield return null;      // Let Start() method run
        
        List<Vector3> points = (List<Vector3>) param[0];
        float movementSpeed = (float) param[1];
        float rotationSpeed = 20f;
        
        int targetPointIndex = 0;
        Vector3 targetPoint = points[targetPointIndex];
        
        bool atDestination = false;
        while (!atDestination) {
            rigidbody.MovePosition(Vector3.MoveTowards(rigidbody.position, targetPoint, movementSpeed * Time.deltaTime));
            RotateTowards(targetPoint, rotationSpeed);
            // Debug.Log(transform.position.y);

            if (VectorUtil.VectorsAreEqual(rigidbody.position, targetPoint)) {
                if (targetPointIndex + 1 < points.Count) {
                    targetPoint = points[++targetPointIndex];
                } else {
                    atDestination = true;
                    damage = 0;      // don't hurt enemies when walking over arrows
                }
            }
            
            yield return null;
        }
    }
    
    private void RotateTowards(Vector3 targetPathPoint, float rotationSpeed) {
        Vector3 direction = (targetPathPoint - rigidbody.position).normalized;
        if (direction == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        rigidbody.MoveRotation(Quaternion.Slerp(rigidbody.rotation, lookRotation, Time.deltaTime * rotationSpeed));
    }
    
    void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag(Tags.Enemy)) {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy == null) {
                enemy = collider.GetComponentInParent<Enemy>();
            }
            
            bool hitHead = checkHeadCollision();
            
            // Multiple colliders can be activated on same Enemy - This makes sure logic only runs once
            if (enemiesHit.Contains(enemy)) {
                Debug.Log("already detected");
                return;
            }
            enemiesHit.Add(enemy);
            
            enemy.TakeDamage(damage, hitHead);
        }
        
        Destroy(rigidbody);
        StopCoroutine(traversePathCoroutine);
    }
    
    private bool checkHeadCollision() {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        float distance = 1f;
        
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, distance);
        Debug.DrawRay(origin, direction * distance, Color.red);
        
        foreach (RaycastHit hit in hits) {
            if (hit.collider.name == Constants.HEAD) {
                return true;
            }
        }
        return false;
    }
    
}
