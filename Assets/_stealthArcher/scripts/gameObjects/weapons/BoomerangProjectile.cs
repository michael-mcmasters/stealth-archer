using System;
using System.Collections;
using System.Collections.Generic;
using _stealthArcher.scripts.constants;
using Unity.VisualScripting;
using UnityEngine;

public class BoomerangProjectile : MonoBehaviour {

    private Rigidbody rigidbody;
    
    private float damage;

    private Collider alreadyHitCollider;        // Needed because arrow will bounce on enemy collider a few times. So this makes sure it only counts as 1 hit.

    private Coroutine traversePathCoroutine;


    private List<Vector3> points;
    private float movementSpeed;
    private float rotationSpeed = 20f;
    private int targetPointIndex;
    private Vector3 targetPoint;
    private bool atDestination;
    

    void Start() {
        this.rigidbody = GetComponent<Rigidbody>();
    }
    
    public void Shoot(List<Vector3> bezierCurve, float speed, float damage) {
        this.damage = damage;
        this.points = bezierCurve;
        this.movementSpeed = speed;
        this.targetPointIndex = 0;
        this.targetPoint = points[0];
        this.atDestination = false;
        // this.traversePathCoroutine = StartCoroutine(nameof(TraversePath), new System.Object[] {bezierCurve, speed});
    }

    void FixedUpdate() {
        if (rigidbody == null || atDestination) return;
        
        rigidbody.MovePosition(Vector3.MoveTowards(rigidbody.position, targetPoint, movementSpeed * Time.deltaTime));
        RotateTowards(targetPoint, rotationSpeed);

        if (VectorUtil.VectorsAreEqual(rigidbody.position, targetPoint)) {
            if (targetPointIndex + 1 < points.Count) {
                targetPoint = points[++targetPointIndex];
            } else {
                atDestination = true;
            }
        }
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

            if (VectorUtil.VectorsAreEqual(rigidbody.position, targetPoint)) {
                if (targetPointIndex + 1 < points.Count) {
                    targetPoint = points[++targetPointIndex];
                } else {
                    atDestination = true;
                }
            }
            
            // yield return CoroutineUtility.WaitForFixedUpdate;
            yield return new WaitForFixedUpdate();
        }
    }
    
    private void RotateTowards(Vector3 targetPathPoint, float rotationSpeed) {
        Vector3 direction = (targetPathPoint - rigidbody.position).normalized;
        if (direction == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        rigidbody.MoveRotation(Quaternion.Slerp(rigidbody.rotation, lookRotation, Time.deltaTime * rotationSpeed));
    }

    void OnCollisionEnter(Collision collision) {
        // if (collision.collider == alreadyHitCollider) return;
        
        // alreadyHitCollider = collision.collider;
        
        if (collision.collider.CompareTag(Tags.Enemy)) {
            Enemy enemy = collision.collider.GetComponent<Enemy>();
            if (enemy == null) {
                enemy = collision.collider.GetComponentInParent<Enemy>();
            }
    
            bool headshot = collision.collider.name == Constants.HEAD;
            
            enemy.TakeDamage(damage, headshot);

            
            // explosionForce	The force of the explosion (which may be modified by distance).
            // explosionPosition	The centre of the sphere within which the explosion has its effect.
            //     explosionRadius	The radius of the sphere within which the explosion has its effect.
            //     upwardsModifier	Adjustment to the apparent position of the explosion to make it seem to lift objects.
            //     mode	The method used to apply the force to its targets.
            
            // rigidbody.AddExplosionForce(100, rigidbody.position, 10);
        }
        else {
            atDestination = true;
        }
    }
    
    // void OnTriggerEnter(Collider collider) {
    //     // if (collider == alreadyHitCollider) return;
    //     
    //     // alreadyHitCollider = collider;
    //     
    //     if (collider.CompareTag(Tags.Instance.GetEnemy())) {
    //         Enemy enemy = collider.GetComponent<Enemy>();
    //         if (enemy == null) {
    //             enemy = collider.GetComponentInParent<Enemy>();
    //         }
    //
    //         bool headshot = collider.name == Constants.HEAD;
    //         
    //         enemy.TakeDamage(damage, headshot);
    //     }
    //     else {
    //         // Don't stop when hitting enemies,
    //         // but do stop when hitting something else
    //         StopCoroutine(traversePathCoroutine);
    //         atDestination = true;
    //     }
    // }
    
    // void OnCollisionEnter(Collision collision) {
    //     if (collision.collider == alreadyHitCollider) return;
    //     
    //     alreadyHitCollider = collision.collider;
    //     
    //     if (collision.collider.CompareTag(Tags.Instance.GetEnemy())) {
    //         Enemy enemy = collision.collider.GetComponent<Enemy>();
    //         if (enemy == null) {
    //             enemy = collision.collider.GetComponentInParent<Enemy>();
    //         }
    //
    //         bool headshot = collision.collider.name == Constants.HEAD;
    //         
    //         enemy.TakeDamage(damage, headshot);
    //     }
    //     
    //     // Attach to object
    //     // transform.parent = collision.transform;
    //     // Destroy(rigidbody);
    //     StopCoroutine(traversePathCoroutine);
    // }
}
