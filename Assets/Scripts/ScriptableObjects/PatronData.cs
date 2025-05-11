using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Resources/PatronsDatas/NewPatron", menuName = "ScriptableObjects/Patron")]
public class PatronData : ScriptableObject
{
    public string patronName;
    public int damage;
    public Sprite icon;
    public ParticleSystem gunPointEffect;
}