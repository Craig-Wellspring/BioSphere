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

        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * body.rotation;
        body.rotation = Quaternion.Slerp(body.rotation, targetRotation, 50 * Time.deltaTime);
    }
}
