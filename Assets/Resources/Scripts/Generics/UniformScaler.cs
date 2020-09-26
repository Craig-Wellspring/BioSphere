using UnityEngine;

[ExecuteAlways]
public class UniformScaler : MonoBehaviour
{
    [Range(0f, 200f)]
    public float scale = 1f;

    public void UpdateScale()
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }

    void OnValidate()
    {
        UpdateScale();
    }
}
