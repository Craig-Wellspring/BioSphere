using UnityEngine;

[ExecuteAlways]
public class Orientation : MonoBehaviour
{
    [SerializeField] bool onStart = true;
    [SerializeField] bool onUpdate = false;
    [SerializeField] float rotationSpeed = 50f;

    public enum OrientTarget { Gravity, Parent };
    public OrientTarget orientTarget;

    void Start()
    {
        if (onStart)
            SnapOrient();

        if (!onUpdate)
            this.enabled = false;
    }

    void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, NewOrientation(), rotationSpeed * Time.deltaTime);
    }

    public void SnapOrient()
    {
        transform.rotation = NewOrientation();
    }

    Quaternion NewOrientation()
    {
        if (orientTarget == OrientTarget.Gravity)
            return Quaternion.FromToRotation(transform.up, -(Vector3.zero - transform.position).normalized) * transform.rotation;

        else if (orientTarget == OrientTarget.Parent)
            return Quaternion.FromToRotation(transform.up, (transform.root.GetComponentInChildren<MeshRenderer>().bounds.center - transform.position).normalized) * transform.rotation;

        else return Quaternion.identity;
    }
}
