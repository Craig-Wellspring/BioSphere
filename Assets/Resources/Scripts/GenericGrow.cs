using UnityEngine;

public class GenericGrow : MonoBehaviour
{
    [Header("Triggers")]
    [Tooltip("Detach from Parent when FullyGrown")]
    public bool gainIndependence = false;
    public bool fullyGrown = false;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    void EndGrowth()
    {
        fullyGrown = true;
        if (anim.parameterCount > 0)
            anim.SetBool("FullyGrown", true);
    }

    void GainIndependence()
    {
        if (gainIndependence)
            transform.SetParent(null);
    }
}