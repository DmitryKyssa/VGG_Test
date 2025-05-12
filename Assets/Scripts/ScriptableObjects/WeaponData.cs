using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Resources/WeaponsDatas/NewWeapon", menuName = "ScriptableObjects/Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public float fireRate;
    public float range;
    public float reloadTime;
    public int magazines;
    public int patronsPerMagazine;
}

public static class WeaponDataExtensions
{
    public static string Stringify(this WeaponData weaponData)
    {
        return $"Weapon Name: {weaponData.weaponName},\n Fire Rate: {weaponData.fireRate},\n Range: {weaponData.range},\n Reload Time: {weaponData.reloadTime},\n Magazines: {weaponData.magazines},\n Patrons Per Magazine: {weaponData.patronsPerMagazine}";
    }
}