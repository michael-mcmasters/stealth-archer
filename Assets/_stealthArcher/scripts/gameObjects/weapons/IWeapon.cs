using _stealthArcher.scripts.models;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace _stealthArcher.scripts.weaponPrefabs {
public abstract class IWeapon : MonoBehaviour {
    public abstract void HandleInput();
}
}