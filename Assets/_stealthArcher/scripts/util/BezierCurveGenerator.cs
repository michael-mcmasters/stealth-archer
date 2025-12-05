using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveGenerator {
    
    // Bezier point algorithm from World Of Zero Youtube channel
    public static List<Vector3> CreateBezierCurve(Vector3 startPoint, Vector3 midPoint, Vector3 endPoint) {
        // Get distance between current mouse position and start position, set how many points in bezier curve
        float currentPosDistance = Vector3.Distance(endPoint, startPoint);
        float pointsInVertex = Mathf.Round(currentPosDistance + 4);

        var bezierPointsList = new List<Vector3>();
        for (float ratio = 0.5f / pointsInVertex; ratio < 1; ratio += 1.0f / pointsInVertex) {
            var tangentLineVertex1 = Vector3.Lerp(startPoint, midPoint, ratio);
            var tangentLineVertex2 = Vector3.Lerp(midPoint, endPoint, ratio);
            var bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
            bezierPointsList.Add(bezierPoint);
        }

        return bezierPointsList;
    }
    
    public static Vector3 GetMidPoint(Vector3 startPoint, Quaternion direction, Vector3 endPoint) {
        // Convert points to 2D because point of intersection calculation only works if points share the same y axis.
        endPoint.y = startPoint.y;

        // Midpoint is center of circle (radius). Get top of circle (diameter) by drawing two line segments and getting their point of intersection.
        Vector3 endPointReflectedDir = GetFirstLineSegment(startPoint, direction, endPoint);
        Vector3 roadFwdDir = GetSecondLineSegment(startPoint, direction);
        bool pointsIntersected = VectorUtil.PointOfIntersection2D(out Vector3 pointOfIntersection, endPoint, endPointReflectedDir, startPoint, roadFwdDir);

        Vector3 midPoint = startPoint;
        if (pointsIntersected) {
            midPoint = (pointOfIntersection + startPoint) * 0.5f;
        } else {
            // edge case
            // Note: Seems to happen if endPoint is on same x axis as startPoint.
            Debug.LogError("Could not calculate curved road's midPoint. Lines did not intersect. This should never happen.");
            // midPoint = GetAlternativeMidPoint(newRoad);
            return midPoint;
        }

        bool debugBezierCircularArc = false;
        if (debugBezierCircularArc) {
            int debugTime = 10;
            // Point of intersection (yellow) is where the blue line and green line meet.
            Debug.DrawLine(startPoint, endPoint, Color.blue, debugTime);
            Debug.DrawLine(endPoint, pointOfIntersection, Color.blue, debugTime);
            Debug.DrawLine(startPoint, pointOfIntersection, Color.red, debugTime);

            DebugExtension.DebugWireSphere(pointOfIntersection, Color.yellow, 1, debugTime);
            DebugExtension.DebugWireSphere(midPoint, Color.white, 0.5f, debugTime);
            DebugExtension.DebugCircle(midPoint, Color.yellow, Vector3.Distance(startPoint, midPoint), debugTime);
        }

        // Convert back to 3D.
        midPoint.y = (startPoint.y + endPoint.y) * 0.5f;
        return midPoint;
    }

    // Blue line gizmo.
    // First line segment starts at endPoint. To get line's direction take the direction of (endPoint - startPoint) and rotate it 90 degrees.
    private static Vector3 GetFirstLineSegment(Vector3 startPoint, Quaternion direction, Vector3 endPoint) {
        Vector3 endPointReflectedDir = endPoint - startPoint;

        Vector3 endPointLP = VectorUtil.toLocalPosition(startPoint, direction, endPoint);
        if (endPointLP.x < 0)
            endPointReflectedDir = (Quaternion.Euler(0, 90, 0) * endPointReflectedDir).normalized;
        else
            endPointReflectedDir = (Quaternion.Euler(0, 270, 0) * endPointReflectedDir).normalized;

        return endPointReflectedDir;
    }

    // Red line gizmo.
    // This segment starts at road's position and moves forward in road's forward direction converted to a 2D y axis
    // (Creates point in front of road, sets point's y axis to road position's y axis, and uses that to set direction of segment).
    private static Vector3 GetSecondLineSegment(Vector3 startPoint, Quaternion direction) {
        Vector3 inFrontOfRoad = VectorUtil.toWorldPosition(startPoint, direction, new Vector3(0, 0, 1));
        inFrontOfRoad.y = startPoint.y;
        Vector3 roadFwdDirection = (inFrontOfRoad - startPoint).normalized;

        return roadFwdDirection;
    }
}