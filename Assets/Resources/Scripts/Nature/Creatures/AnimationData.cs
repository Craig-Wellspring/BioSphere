using Pathfinding;
using UnityEngine;

public class AnimationData : MonoBehaviour
{
    Animator anim;
    Metabolism meta;

    void Start()
    {
        anim = GetComponent<Animator>();
        meta = GetComponentInChildren<Metabolism>();



        meta.BeginEating += BeginEating;
        meta.CeaseEating += CeaseEating;
    }

    private void OnDisable()
    {
        meta.BeginEating -= BeginEating;
        meta.CeaseEating -= CeaseEating;
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
