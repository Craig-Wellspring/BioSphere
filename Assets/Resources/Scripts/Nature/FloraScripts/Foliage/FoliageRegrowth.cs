using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoliageRegrowth : MonoBehaviour
{
    [SerializeField] int secondsDormant = 20;
    
    [Space(10)]
    [Tooltip("Destroys the root object when no Energy remains.")]
    [SerializeField] bool destroyWhenDepleted = true;
    [Tooltip("Seconds to wait before destroying root object. 0 is instant.")]
    [SerializeField] int decayTime = 120;

    public void ConsumeFoliage(GameObject _leaves)
    {
        EnergyData eData = transform.root.GetComponent<EnergyData>();
        FoodData newLeafFData = _leaves.GetComponent<FoodData>();

        if (eData.energyReserve > 0)
            RegenerateLeaves(_leaves);
        else if (destroyWhenDepleted)
            Destroy(transform.root.gameObject, decayTime);
    }

    void RegenerateLeaves(GameObject _leafObject)
    {
        _leafObject.tag = "Seedling";
        _leafObject.transform.localScale = Vector3.zero;
        RegrowLeaves(_leafObject, secondsDormant);
    }

    IEnumerator RegrowLeaves(GameObject _leafObject, int _delay)
    {
        yield return new WaitForSeconds(_delay);
        _leafObject.SetActive(true);
    }
}
