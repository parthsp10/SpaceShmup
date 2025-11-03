using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponDefinition {
    public eWeaponType type = eWeaponType.none;
    public string letter;
    public Color color = Color.white;
    public GameObject projectilePrefab;
    public float damageOnHit = 1;
    public float delayBetweenShots = 0.5f;
    public float velocity = 40;
}
