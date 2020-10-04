using UnityEngine;


[RequireComponent(typeof(SingleGradient))]
public class LODRandomColor : MonoBehaviour
{
    void Start()
    {
        // Choose color at random from gradient to assign to all LODs
        Color selectedColor = GetComponent<SingleGradient>().gradient.Evaluate(Random.Range(0f, 1f));

        foreach(MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>(true))
            mesh.material.color = selectedColor;

        Destroy(GetComponent<SingleGradient>(), 0.01f);
        Destroy(this);
    }
}
