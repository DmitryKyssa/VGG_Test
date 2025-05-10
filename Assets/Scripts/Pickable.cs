using UnityEngine;

public class Pickable : MonoBehaviour
{
    [SerializeField] private PickableData pickableData;

    private void Start()
    {
        gameObject.SetTag(Tag.Pickable);
    }
}