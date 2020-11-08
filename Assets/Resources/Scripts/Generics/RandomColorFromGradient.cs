using UnityEngine;

public class RandomColorFromGradient : MonoBehaviour
{
    [SerializeField] Gradient gradient;

    void Start()
    {
        // Choose color at random from gradient to assign to all LODs
        Color selectedColor = gradient.Evaluate(Random.Range(0f, 1f));

        if (TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
            meshRenderer.materials[0].color = selectedColor;
        else
            foreach(MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>(true))
                mesh.materials[0].color = selectedColor;
    }
}
