using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ocean : MonoBehaviour
{
    [SerializeField] bool restrictBreath = true;
    [SerializeField] bool buoyancy = true;

    void OnTriggerEnter(Collider _collider)
    {
        if (restrictBreath)
        {
            Respiration respiration = _collider.transform.root.GetComponentInChildren<Respiration>();
            if (respiration != null && !respiration.amphibious)
                respiration.canBreathe = false;
        }

        if (buoyancy &&_collider.transform.root.TryGetComponent<GravityAttract>(out GravityAttract gBody))
        {
            gBody.enabled = false;
        }
    }

    void OnTriggerExit(Collider _collider)
    {
        if (restrictBreath)
        {
            Respiration respiration = _collider.transform.root.GetComponentInChildren<Respiration>();
            if (respiration != null && !respiration.amphibious)
                respiration.canBreathe = true;
        }
        
        if (buoyancy && _collider.transform.root.TryGetComponent<GravityAttract>(out GravityAttract gBody))
        {
            gBody.enabled = true;
        }
    }
}
