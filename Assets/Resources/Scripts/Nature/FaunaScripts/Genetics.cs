using UnityEngine;
using System.Collections.Generic;

public class Genetics : MonoBehaviour
{
    public int numberOfChildren;
    public GameObject parentCreature;
    public List<GameObject> ancestors;

    public void PassDownGenes(Genetics _targetGenes)
    {
        _targetGenes.ancestors = ancestors;
        if (parentCreature != null)
            _targetGenes.ancestors.Add(parentCreature);
        _targetGenes.parentCreature = transform.root.gameObject;

        //_targetGenes.GetComponent<OnDestroyEvent>().BeingDestroyed += RemoveSelf;
    }

    public void CopyGenes(Genetics _targetGenes)
    {
        _targetGenes.numberOfChildren = numberOfChildren;
        _targetGenes.parentCreature = parentCreature;
        _targetGenes.ancestors = ancestors;
    }

    /*void RemoveSelf()
    {

    }*/
}
