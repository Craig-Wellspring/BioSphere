using UnityEngine;

public class PlanetCore : MonoBehaviour
{
    #region Singleton
    public static PlanetCore Core { get; private set; }
    void Awake()
    {
        if (Core == null)
            Core = this;
        else
            Destroy(gameObject); //should never happen
    }
    #endregion


    public float gravity = -10f;
    public float alignSpeed = 50f;

    public void Attract(Rigidbody _body)
    {
        Vector3 gravityUp = (transform.position - _body.position).normalized;

        _body.AddForce(-gravityUp * gravity);
    }

    public void AlignWithGravity(Transform _body, bool _snap)
    {
        Vector3 gravityUp = (transform.position - _body.position).normalized;

        Quaternion targetRotation = Quaternion.FromToRotation(_body.up, -gravityUp) * _body.rotation;
        if (!_snap)
            _body.rotation = Quaternion.Slerp(_body.rotation, targetRotation, alignSpeed * Time.deltaTime);
    }
}
