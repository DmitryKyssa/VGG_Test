using UnityEngine;

public class PickableData : ScriptableObject
{
    public string pickableName;
    public string pickableDescription;
    [ReadOnly] public PickableType pickableType;
    [ReadOnly] public int pickableValue;
    [ReadOnly] public WeaponType weaponType;
    [ReadOnly] public PatronType patronType;
}