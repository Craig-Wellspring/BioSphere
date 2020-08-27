using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureData : MonoBehaviour
{
    [Header("Body Parts")]
    [Tooltip("GameObjects that contain the Creature's body colliders")]
    public List<Collider> bodyColliders;
    [Tooltip("GameObject that represents the Creature's corpse")]
    [Space(10)]
    public GameObject corpse;
}
