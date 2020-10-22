using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedMonoBehaviour : MonoBehaviour
{
    // Find a position in a random radius on a plane 25 units above the origin transform
    protected Vector3 PositionAbove(Transform _origin, float _radius = 0)
    {
        if (_radius > 0)
        {
            Vector3 newPos = _origin.position + (_origin.up * 25) + (_origin.right * Random.Range(-_radius, _radius)) + (_origin.forward * (Random.Range(-_radius, _radius)));
            return newPos;
        }
        else return _origin.position + (_origin.up * 25);
    }

    // Find a position on the Terrain directly beneath the position given
    protected (Vector3 position, GameObject terrainObject) TerrainUnderPosition(Vector3 _position)
    {
        if (Physics.Raycast(_position, GravityVector(_position), out RaycastHit hit, 500, LayerMask.GetMask("Geosphere")))
            return (hit.point, hit.transform.gameObject);
        else
        {
            Debug.LogError("PointOnTerrainUnderPosition Raycast hit nothing.", this);
            return (Vector3.zero, null);
        }
    }


    // Get Vector pointing toward global zero
    protected Vector3 GravityVector(Vector3 _fromPos)
    {
        Vector3 gravityVector = (Vector3.zero - _fromPos).normalized;
        return gravityVector;
    }


    // Get gravity aligned rotation
    protected Quaternion GravityOrientedRotation()
    {
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -GravityVector(transform.position)) * transform.rotation;

        return targetRotation;
    }


    protected GameObject ClosestObjectInList(List<GameObject> _objectList, bool _returnRoot)
    {
        GameObject closestObj = null;
        float closestDistanceSqr = Mathf.Infinity;
        foreach (GameObject _object in _objectList)
        {
            if (_object != null)
            {
                Vector3 directionToTarget = _object.transform.position - transform.position;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr && _object != this.gameObject)
                {
                    closestDistanceSqr = dSqrToTarget;
                    closestObj = _returnRoot ? _object.transform.root.gameObject : _object;
                }
            }
        }
        return closestObj;
    }


    #region Unused Functions
    /*

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


    public void LerpToPos(Transform _transform, Vector3 _toPos, float _duration)
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
    }


    protected Vector3 RandomPointOnCol(Collider _collider)
    {
        return new Vector3(
            Random.Range(_collider.bounds.min.x, _collider.bounds.max.x),
            Random.Range(_collider.bounds.min.y, _collider.bounds.max.y),
            Random.Range(_collider.bounds.min.z, _collider.bounds.max.z));
    }


    protected Vector3 DirFromAngle(float _angleInDegrees)
    {
        return new Vector3(Mathf.Sin(_angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(_angleInDegrees * Mathf.Deg2Rad));
    }


    protected Transform FindChildWithTag(string _tag, Transform _parent = null)
    {
        Transform returnChild = null;
        if (_parent == null)
            _parent = this.transform;

        foreach (Transform child in _parent.GetComponentsInChildren<Transform>(true))
            if (child.CompareTag(_tag))
            {
                returnChild = child;
                break;
            }

        return returnChild;
    }
    

    protected List<Transform> FindChildrenWithTag(string _tag, Transform _parent = null)
    {
        List<Transform> returnChildren = null;
        if (_parent == null)
            _parent = this.transform;

        foreach (Transform child in _parent.GetComponentsInChildren<Transform>(true))
            if (child.CompareTag(_tag))
                returnChildren.Add(child);

        return returnChildren;
    }
    */
    #endregion
}

