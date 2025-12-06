using UnityEngine;

namespace _stealthArcher.scripts {
public struct JoystickData {

    public Vector3 AroundPlayerPosition;
    public Vector3 Direction;
    public Quaternion Rotation;
    public float TiltPercentage;        // Is a decimal value from 0 to 1

}
}