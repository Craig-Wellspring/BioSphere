using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField] bool active = true;
    [SerializeField] bool destroy = false;

    void OnTriggerEnter(Collider _collider)
    {
        if (active)
        {
            if (destroy)
            {
                Destroy(_collider.transform.root.gameObject);
                Debug.LogWarning(_collider.transform.root.name + " was destroyed by a black hole.");
            }
            else
            {
                PlanetCore.Core.AlignWithGravity(_collider.transform.root, true);
                if (Physics.Raycast(new Ray(UtilityFunctions.PositionAbove(_collider.transform, 0, 300), UtilityFunctions.GravityVector(_collider.transform.position)), out RaycastHit hit, (Vector3.zero - _collider.transform.position).magnitude, LayerMask.GetMask("Geosphere")))
                {
                    _collider.transform.root.position = hit.point;
                    Debug.LogWarning(_collider.transform.root.name + " was moved to the surface by a black hole.", _collider.gameObject);
                }
            }
        }
    }
}
