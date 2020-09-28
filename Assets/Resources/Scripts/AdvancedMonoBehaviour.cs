using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedMonoBehaviour : MonoBehaviour
{
    // Find a random position on the terrain in a radius around origin transform
    protected Vector3 RandomGroundPos(Transform _origin, float _radius)
    {
        Vector3 newPoint = PointOnTerrainUnderPosition(RandomLocalPos(_origin, _radius));
        //while (newPoint == null || newPoint == Vector3.zero)
        //    newPoint = PointOnTerrainUnderPosition(RandomLocalPos(_origin, _radius));
            
        return newPoint;
    }


    // Find a position in a random radius on a plane 5 units above the origin transform
    protected Vector3 RandomLocalPos(Transform _origin, float _radius)
    {
        if (_radius > 0)
        {
            Vector3 newPos = _origin.position + (_origin.up * 25) + (_origin.right * Random.Range(-_radius, _radius)) + (_origin.forward * (Random.Range(-_radius, _radius)));
            return newPos;
        }
        else return _origin.position + (_origin.up * 25);
    }

    // Find a position on the Terrain directly beneath the position
    protected Vector3 PointOnTerrainUnderPosition(Vector3 _position)
    {   
        if (Physics.Raycast(_position, GravityVector(_position), out RaycastHit hit, 500, LayerMask.GetMask("Geosphere")))
        {
            if (hit.collider.CompareTag("Ground"))
                return hit.point;
            else
            {
                Debug.LogError("PointOnTerrainUnderPosition Raycast hit something not tagged as Ground.", this);
                return Vector3.zero;
            }
        }
        else
        {
            Debug.LogError("PointOnTerrainUnderPosition Raycast hit nothing.", this);
            return Vector3.zero;
        }
    }
    

    // Find the Terrain object directly beneath the position
    protected GameObject TerrainUnderPosition(Vector3 _position)
    {
        if (Physics.Raycast(_position, GravityVector(_position), out RaycastHit hit, 200, LayerMask.GetMask("Geosphere")))
        {
            if (hit.collider.CompareTag("Ground"))
                return hit.transform.gameObject;
            else
            {
                Debug.LogError("TerrainUnderPosition Raycast hit something not tagged as Ground.", this);
                return null;
            }
        }
        else
        {
            Debug.LogError("TerrainUnderPosition Raycast hit nothing.", this);
            return null;
        }
    }




    public void ResetTransform(Transform _transform)
    {
        _transform.localPosition = Vector3.zero;
        _transform.localRotation = Quaternion.identity;
    }
    public void ResetTransform(Transform _transform, bool _resetScale)
    {
        _transform.localPosition = Vector3.zero;
        _transform.localRotation = Quaternion.identity;
        if (_resetScale)
            _transform.localScale = Vector3.one;
    }

    /*public void LerpToPos(Transform _transform, Vector3 _toPos, float _duration)
    {
        StartCoroutine(LerpToPosition(_transform, _toPos, _duration));
    }
    IEnumerator LerpToPosition(Transform _transform, Vector3 _toPos, float _duration)
    {
        float time = 0;

        while (time < _duration)
        {
            _transform.position = Vector3.Lerp(_transform.position, _toPos, time / _duration);
            time += Time.deltaTime;
            yield return null;
        }
        _transform.position = _toPos;
    }*/

    protected Vector3 GravityVector(Vector3 _fromPos)
    {
        Vector3 gravityVector = (Vector3.zero - _fromPos).normalized;
        return gravityVector;
    }


    //Get gravity aligned rotation
    protected Quaternion GravityOrientedRotation()
    {
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -GravityVector(transform.position)) * transform.rotation;

        return targetRotation;
    }

    protected Vector3 RandomPointOnCol(Collider _collider)
    {
        return new Vector3(
            Random.Range(_collider.bounds.min.x, _collider.bounds.max.x),
            Random.Range(_collider.bounds.min.y, _collider.bounds.max.y),
            Random.Range(_collider.bounds.min.z, _collider.bounds.max.z));
    }

    protected GameObject ClosestObjInColliderList(List<Collider> _colliderList, bool _returnRoot)
    {
        GameObject closest = null;
        float closestDistanceSqr = Mathf.Infinity;
        foreach (Collider collider in _colliderList)
        {
            if (collider != null)
            {
                Vector3 directionToTarget = collider.transform.position - transform.position;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr && collider.gameObject != this.gameObject)
                {
                    closestDistanceSqr = dSqrToTarget;
                    closest = _returnRoot ? collider.transform.root.gameObject : collider.gameObject;
                }
            }
        }
        return closest;
    }

    protected Vector3 DirFromAngle(float _angleInDegrees)
    {
        return new Vector3(Mathf.Sin(_angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(_angleInDegrees * Mathf.Deg2Rad));
    }
}
