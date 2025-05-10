using UnityEngine;

public enum PickableType
{
    Health,
    Magazines,
    Weapon
}

[CreateAssetMenu(fileName = "Assets/Resources/Pickables/NewPickable", menuName = "ScriptableObjects/Pickable")]
public class PickableData : ScriptableObject
{
    public string pickableName;
    public string pickableDescription;
    public Sprite pickableIcon;
    [ReadOnly] public PickableType pickableType;
    [ReadOnly] public int pickableValue;
    [ReadOnly] public WeaponType weaponType;
}