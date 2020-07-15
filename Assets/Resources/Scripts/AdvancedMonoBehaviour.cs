using UnityEngine;

public class AdvancedMonoBehaviour : MonoBehaviour
{
    public Vector3 PointOnTerrainUnderPosition(Vector3 _position)
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(_position + (_position - Vector3.zero).normalized, -(_position - Vector3.zero).normalized, out hit, 2000, LayerMask.GetMask("Terrain")))
        {
            if (hit.collider.CompareTag("Ground"))
                return hit.point;
            else return Vector3.zero;
        }
        else return Vector3.zero;
    }
    

    public Vector3 GravityVector(Vector3 _fromPos)
    {
        Vector3 gravityUp = (_fromPos - PlanetCore.Core.transform.position).normalized;
        return -gravityUp;
    }


    public Vector3 GetRandomPointOnCol(Collider _collider)
    {
        return new Vector3(
            Random.Range(_collider.bounds.min.x, _collider.bounds.max.x),
            Random.Range(_collider.bounds.min.y, _collider.bounds.max.y),
            Random.Range(_collider.bounds.min.z, _collider.bounds.max.z));
    }
}
