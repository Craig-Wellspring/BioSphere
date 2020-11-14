using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateRandom : MonoBehaviour
{
    [SerializeField] List<GameObject> listOfObjects = new List<GameObject>();

    void Start()
    {
        listOfObjects[Random.Range(0, listOfObjects.Count)].SetActive(true);
    }
}
