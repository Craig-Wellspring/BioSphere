using System.Collections;
using System.Collections.Generic;
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

    public Vector3 GravityVector(Vector3 _fromPos)
    {
        Vector3 gravityUp = (_fromPos - PlanetCore.Core.transform.position).normalized;
        return -gravityUp;
    }


    //Get gravity aligned rotation
    public Quaternion GravityUp()
    {
        Quaternion rot = Quaternion.FromToRotation(transform.root.up, (transform.root.position - Vector3.zero).normalized) * transform.root.rotation;

        return rot;
    }

    public Vector3 GetRandomPointOnCol(Collider _collider)
    {
        return new Vector3(
            Random.Range(_collider.bounds.min.x, _collider.bounds.max.x),
            Random.Range(_collider.bounds.min.y, _collider.bounds.max.y),
            Random.Range(_collider.bounds.min.z, _collider.bounds.max.z));
    }

    public GameObject ClosestColliderInList(List<Collider> _colliderList)
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
                    closest = collider.gameObject;
                }
            }
        }
        return closest;
    }

    public Vector3 DirFromAngle(float _angleInDegrees)
    {
        return new Vector3(Mathf.Sin(_angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(_angleInDegrees * Mathf.Deg2Rad));
    }
}
