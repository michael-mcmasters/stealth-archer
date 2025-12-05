using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VectorUtil {
    // Convert world point to local position
    // (If you want relativePoint to be affected by scale, divide difference by the scale you want)
    public static Vector3 toLocalPosition(Vector3 basePoint, Quaternion baseForward, Vector3 relativePoint) {
        Vector3 difference = relativePoint - basePoint;
        return Quaternion.Inverse(baseForward) * new Vector3(difference.x, difference.y, difference.z);
    }

    public static Vector3 toLocalPosition(GameObject gameObject, Vector3 relativePoint) {
        return toLocalPosition(gameObject.transform.position, gameObject.transform.rotation, relativePoint);
    }

    // Creates new point local to base point and converts it to world position
    // (To calculate forwardDirection before passing, use Quaternion.LookRotation((pointB - pointA).normalize)
    public static Vector3 toWorldPosition(Vector3 basePoint, Quaternion forwardDirection, Vector3 newPosition) {
        return basePoint + forwardDirection * newPosition;
    }

    public static Vector3 toWorldPosition(GameObject gameObject, Vector3 newPosition) {
        return toWorldPosition(gameObject.transform.position, gameObject.transform.rotation, newPosition);
    }

    // Check if vectors are equal by checking their distance from one another.
    // Use this when (Vector1 == Vector2) returns false when it should return true because of floating point precision errors.
    public static bool VectorsAreEqual(Vector3 firstVector, Vector3 secondVector) {
        float distance = Vector3.Distance(firstVector, secondVector);
        return distance > -0.001f && distance < 0.001f;
    }

    // Returns new point x units away from the base point in the forward direction
    // (To get forwardDirection, use (pointB - pointA).normalize)
    public static Vector3 NewPointInDirection(Vector3 basePoint, Vector3 forwardDirection, float unitsToMove) {
        return basePoint + forwardDirection * unitsToMove;
    }

    public static Vector3 NewPointInDirection(GameObject gameObject, float unitsToMove) {
        return gameObject.transform.position + gameObject.transform.forward * unitsToMove;
    }
    
    // Similar to NewPointInDirection but gets the forwardDirection for you
    // Ex: Vector3 point = VectorUtil.NewPointTowardsTarget(startPoint, endPoint, distance)
    public static Vector3 NewPointInTargetsDirection(Vector3 basePoint, Vector3 targetPoint, float unitsToMove) {
        Vector3 forwardDirection = (targetPoint - basePoint).normalized;
        return NewPointInDirection(basePoint, forwardDirection, unitsToMove);
    }
    
    // Code is based off of this documentation: https://docs.unity3d.com/ScriptReference/Vector3.RotateTowards.html
    // Example: gameObject.transform.rotation = VectorUtil.RotateTowards(gameObject, targetPosition);
    public static Quaternion RotateTowards(GameObject obj, Vector3 targetPos) {
        Vector3 targetDirection = (targetPos - obj.transform.position).normalized;
        if (targetDirection.z == -1) {
            // This occurs when gameObject is facing exactly away from targetPos. I believe setting z to 1 will fix it but if you are getting weird errors, maybe remove this if condition
            // Debug.Log("While rotating object, targetDirection.z was set to -1. Setting it to 1 instead as this has resolved a weird error in the past", obj);
            targetDirection.z = 1;
        }
        Vector3 newDirection = Vector3.RotateTowards(obj.transform.forward, targetDirection, 1000, 1000);
        return Quaternion.LookRotation(newDirection);
    }
    
    // Code is based off of this documentation: https://docs.unity3d.com/ScriptReference/Vector3.RotateTowards.html
    // Ex VectorUtil.RotateTowards(gameObject, target.gameObject);
    public static Quaternion RotateTowards(GameObject obj, GameObject targetObj) {
        return RotateTowards(obj, targetObj.transform.position);
    }
    
    // Calculate point of intersection between two 2D lines (lines that share same y axis). Returns true if intersected and false if not.
    // For 3D, use ClosestPointsOnTwoLines(). Source: https://wiki.unity3d.com/index.php/3d_Math_functions.
    public static bool PointOfIntersection2D(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2) {
        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        // is coplanar, and not parrallel
        if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f) {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            return true;
        } else {
            intersection = Vector3.zero;
            return false;
        }
    }
    
    /// <summary>
    /// Note! This hasn't been tested. Double check it works for points really far away from centerAnchor. May need to make them evenly spaced away from intersection if it doesn't work
    /// centerAnchor: The point all monoBehaviours will be sorted around
    /// forward: The relative direction angle 0 will begin at
    /// </summary>
    public static List<MonoBehaviour> SortClockwise(Vector3 centerAnchor, Vector3 forward, IEnumerable<MonoBehaviour> monoBehaviours) {
        IDictionary<float, MonoBehaviour> sortedMap = new SortedDictionary<float, MonoBehaviour>();     // angle, monoBehaviour at that angle
        foreach (MonoBehaviour m in monoBehaviours) {
            Vector3 monoToAnchorLp = m.transform.position;
            float angle = Vector3.SignedAngle(monoToAnchorLp, forward, Vector3.up);
     
            // Vector3.SignedAngle only returns values from -180 -> 0 (if on left), to 0 -> 180 (if on right). This if condition converts everything to 0 to 360 like you would expect.
            if (angle < 0) {
                angle = 360 - angle * -1;
            }
            sortedMap.Add(angle, m);
        }
        List<MonoBehaviour> sortedList = sortedMap.Values.ToList<MonoBehaviour>();
        sortedList.Reverse();
        return sortedList;
    }
}
