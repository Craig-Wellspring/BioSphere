using UnityEngine;

public class BlackHole : AdvancedMonoBehaviour
{
    [SerializeField] bool active = true;
    [SerializeField] bool destroy = false;

    void OnTriggerEnter(Collider collider)
    {
        if (active)
        {
            if (destroy)
            {
                Destroy(collider.transform.root.gameObject);
                Debug.LogWarning(collider.transform.root.name + " was destroyed by a black hole.");
            }
            else
            {
                if (Physics.Raycast(new Ray(collider.transform.position, -GravityVector(collider.transform.position)), out RaycastHit hit, 300, LayerMask.GetMask("Geosphere")))
                {
                    collider.transform.root.position = hit.point;
                    PlanetCore.Core.AlignWithGravity(collider.transform.root, true);
                    Debug.LogWarning(collider.transform.root.name + " was moved to the surface by a black hole.", collider.gameObject);
                }
            }
        }
    }
}
