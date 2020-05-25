using UnityEngine;

public class Food : MonoBehaviour
{
    public float nutritionalValue = 1f;
    public float timeToEat = 5f;
    [Tooltip("Destroy the parent entity when eaten")]
    public bool destroyParent = true;
}
