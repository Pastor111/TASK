using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "New Weapon")]
public class Weapon : ScriptableObject
{
    public string Name;
    public int magazineSize;
    public enum WeaponType{HitScan, Projectile}
    public WeaponType weaponType;
    public enum FireType{Semi_Auto, Full_Auto}
    public FireType fireType;
    public float fireDelay;
    public float Innacuracy;
    public RuntimeAnimatorController controller;
    public AudioClip ShootingSound;
    public AudioProfilerClipInfo EmptySound;
    [Range(0.0f, 3.0f)]
    public float ShakeMultiplier;
    //public Vector3 Recoil;
}
