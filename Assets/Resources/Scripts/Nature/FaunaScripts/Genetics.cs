using UnityEngine;
using System.Collections.Generic;

public class Genetics : MonoBehaviour
{
    [SerializeField] int age = 0;

    [Space(10)]
    public int numberOfChildren;
    public GameObject parentCreature;
    public List<GameObject> ancestors;


    void Start()
    {
        InvokeRepeating("IncreaseAge", 1, 1);
    }

    void IncreaseAge()
    {
        age++;
    }

    public void PassDownGenes(Genetics _targetGenes)
    {
        _targetGenes.ancestors = ancestors;
        if (parentCreature != null)
            _targetGenes.ancestors.Add(parentCreature);
        _targetGenes.parentCreature = transform.root.gameObject;
    }

    public void CopyGenes(Genetics _targetGenes)
    {
        _targetGenes.age = age;
        _targetGenes.numberOfChildren = numberOfChildren;
        _targetGenes.parentCreature = parentCreature;
        _targetGenes.ancestors = ancestors;
    }
}
