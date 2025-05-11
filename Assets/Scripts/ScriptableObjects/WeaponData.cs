using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Resources/WeaponsDatas/NewWeapon", menuName = "ScriptableObjects/Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public Sprite icon;
    public float fireRate;
    public float range;
    public float reloadTime;
    public int magazines;
    public int patronsPerMagazine;
}