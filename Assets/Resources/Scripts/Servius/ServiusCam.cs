using UnityEngine;

public class ServiusCam : MonoBehaviour
{
    public static ServiusCam Cam { get; private set; }

    private void Awake()
    {
        if (Cam == null)
        {
            Cam = this;
        }
        else
        {
            Destroy(gameObject); //should never happen
        }
    }

    public void ResetTransform()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
