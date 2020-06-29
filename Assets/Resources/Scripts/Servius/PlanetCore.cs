using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetCore : MonoBehaviour
{
    public static PlanetCore Core { get; private set; }

    private void Awake()
    {
        if (Core == null)
        {
            Core = this;
        }
        else
        {
            Destroy(gameObject); //should never happen
        }
    }


    public float gravity = -10f;

    public void Attract(Transform body)
    {
        Vector3 gravityUp = (body.position - transform.position).normalized;
        Vector3 bodyUp = body.up;

        body.GetComponent<Rigidbody>().AddForce(gravityUp * gravity);
    }

    public void AlignWithGravity(Transform _body)
    {
        Vector3 gravityUp = (_body.position - transform.position).normalized;
        Vector3 bodyUp = _body.up;

        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * _body.rotation;
        _body.rotation = Quaternion.Slerp(_body.rotation, targetRotation, 50 * Time.deltaTime);
    }
}
