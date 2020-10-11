using UnityEngine;


[RequireComponent(typeof(SingleGradient))]
public class RandomColorFromGradient : MonoBehaviour
{
    void Start()
    {
        // Choose color at random from gradient to assign to all LODs
        Color selectedColor = GetComponent<SingleGradient>().gradient.Evaluate(Random.Range(0f, 1f));

        if (GetComponent<MeshRenderer>() != null)
            GetComponent<MeshRenderer>().materials[0].color = selectedColor;
        else
            foreach(MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>(true))
                mesh.materials[0].color = selectedColor;

        Destroy(GetComponent<SingleGradient>(), 0.01f);
        Destroy(this);
    }
}
