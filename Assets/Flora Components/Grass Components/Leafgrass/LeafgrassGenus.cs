using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafgrassGenus : MonoBehaviour
{
    static int populationSize;
    public int overgrowthThreshold = 1000;
    public float fertilityModifier = 0.4f;

    void OnEnable()
    {

        populationSize += 1;

        if (this.GetComponent<SpawnFruit>() != null)
            this.GetComponent<SpawnFruit>().seedSuccessChance -= populationSize * fertilityModifier;


        if (populationSize > overgrowthThreshold)
        {
            Debug.Log("Crash due to " + gameObject.name + " overgrowth");
            UnityEditor.EditorApplication.isPlaying = false;
        }
        //Debug.Log(gameObject.name + " population increased to " + populationSize);
    }

    void OnDisable()
    {
        populationSize -= 1;
        //Debug.Log(gameObject.name + " population decreased to " + populationSize);
    }
}
