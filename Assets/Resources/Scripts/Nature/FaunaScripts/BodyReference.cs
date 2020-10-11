using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BodyReference : AdvancedMonoBehaviour
{
    [Header("Body Parts")]
    [Tooltip("The Creature's primary body collider which must be on the same layer as the root object")]
    public Collider mainBodyCollider;
    [Tooltip("The Creature's additional body colliders which must be on the CreatureAdl layer")]
    public List<Collider> adlBodyColliders;
    [Space(15)]
    public Transform eyes;
    [Space(10)]

    public Transform mouth;

    [Header("Corpse")]
    [Tooltip("GameObject that represents the Creature's corpse")]
    public GameObject corpse;


    void Start()
    {
        RegisterBodyColliders();
        RegisterCorpse();
        RegisterSensors();
    }
    


    void RegisterBodyColliders()
    {
        // Clear cache
        mainBodyCollider = null;
        adlBodyColliders = new List<Collider>();

        // Cache colliders
        foreach (Collider _col in transform.root.GetComponentsInChildren<Collider>(false))
        {
            if (_col.gameObject.layer == LayerMask.NameToLayer("CreatureAdl"))
            {
                adlBodyColliders.Add(_col);
                continue;
            }
            if (_col.gameObject.layer == transform.root.gameObject.layer)
                mainBodyCollider = _col;
        }
    }

    void RegisterCorpse()
    {
        // Cache corpse
        corpse = transform.root.Find("Corpse").gameObject;
    }

    void RegisterSensors()
    {
        // Cache eyes transform
        eyes = FindChildWithTag("Eyes", transform.root);

        // Cache mouth transform
        mouth = FindChildWithTag("Mouth", transform.root);
    }
}
