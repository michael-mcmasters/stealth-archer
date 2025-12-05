using System.Collections.Generic;
using UnityEngine;

namespace _stealthArcher.scripts.models {
public struct AimData {

    public List<Vector3> pathPoints;

    public AimData(List<Vector3> pathPoints) {
        this.pathPoints = pathPoints;
    }
    
}
}