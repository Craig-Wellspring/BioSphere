using UnityEngine;

public class PlanetCore : MonoBehaviour
{
    public static PlanetCore Core { get; private set; }
    void Awake()
    {
        if (Core == null)
            Core = this;
        else
            Destroy(gameObject); //should never happen
    }


    public float gravity = -10f;
    public float alignSpeed = 50f;

    public void Attract(Transform body)
    {
        Vector3 gravityUp = (transform.position - body.position).normalized;

        body.GetComponent<Rigidbody>().AddForce(-gravityUp * gravity);
    }

    public void AlignWithGravity(Transform _body)
    {
        Vector3 gravityUp = (transform.position - _body.position).normalized;

        Quaternion targetRotation = Quaternion.FromToRotation(_body.up, -gravityUp) * _body.rotation;
        _body.rotation = Quaternion.Slerp(_body.rotation, targetRotation, alignSpeed * Time.deltaTime);
    }
}
