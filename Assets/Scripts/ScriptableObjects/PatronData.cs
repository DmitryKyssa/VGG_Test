using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Resources/PatronsDatas/NewPatron", menuName = "ScriptableObjects/Patron")]
public class PatronData : ScriptableObject
{
    public string patronName;
    public int damage;
    public ParticleSystem gunPointEffect;
}

public static class PatronDataExtensions
{
    public static string Stringify(this PatronData patronData)
    {
        return $"Patron Name: {patronData.patronName},\n Damage: {patronData.damage}";
    }
}