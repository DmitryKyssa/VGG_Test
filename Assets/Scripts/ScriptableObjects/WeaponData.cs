using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Resources/Weapons/NewWeapon", menuName = "ScriptableObjects/Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public float fireRate;
    public int damage;
    public float range;
    public float reloadTime;
    public int magazines;
    public int patronsPerMagazine;
}