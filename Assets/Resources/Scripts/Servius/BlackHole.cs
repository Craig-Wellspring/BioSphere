using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField] bool active = true;

    void OnTriggerEnter(Collider collider)
    {
        if (active)
        {
            Destroy(collider.transform.root.gameObject);
            Debug.Log(collider.transform.root.name + " was destroyed by a black hole.");
        }
    }
}
