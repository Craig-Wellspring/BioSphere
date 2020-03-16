using UnityEngine;

public class SeedgrassGenus : MonoBehaviour
{
    static int populationSize;
    public int overgrowthThreshold = 1000;
    public float fertilityModifier = 0.4f;
    private SpawnFruit fruitSpawner;

    void OnEnable()
    {
        populationSize += 1;

        //Adjust Leafgrass-A spawnrate based on population
        SpawnFruit[] fruitSpawners = this.GetComponents<SpawnFruit>();
        foreach(SpawnFruit spawner in fruitSpawners)
        {
            if (spawner.newFruit[0].name == "Leafgrass-A")
            {
                fruitSpawner = spawner;
                break;
            }
        }
        if (fruitSpawner != null)
            fruitSpawner.seedSuccessChance -= populationSize * fertilityModifier;
        if (fruitSpawner.seedSuccessChance <= 0.1f)
            fruitSpawner.seedSuccessChance = 0.1f;



        //Crash game if population grows too high
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
