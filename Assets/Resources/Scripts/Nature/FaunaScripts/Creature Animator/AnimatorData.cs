using Pathfinding;
using UnityEngine;

public class AnimatorData : MonoBehaviour
{
    Animator anim;
    Metabolism meta;

    void Start()
    {
        anim = GetComponent<Animator>();
        meta = GetComponentInChildren<Metabolism>();



        meta.EatingBegins += BeginEating;
        meta.EatingEnds += CeaseEating;
    }

    private void OnDisable()
    {
        meta.EatingBegins -= BeginEating;
        meta.EatingEnds -= CeaseEating;
    }

    void BeginEating()
    {
        anim.SetBool("IsEating", true);
    }
    void CeaseEating()
    {
        anim.SetBool("IsEating", false);
    }
}
