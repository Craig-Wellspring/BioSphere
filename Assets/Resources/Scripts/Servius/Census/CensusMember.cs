using UnityEngine;

public class CensusMember : MonoBehaviour
{
    private bool playMode = true;

    private void OnApplicationQuit()
    {
        playMode = false;
    }

    void Start()
    {
        CensusMaster.Census.PopulationIncrease(gameObject.name);
    }

    void OnDisable()
    {
        if (playMode)
        {
            if (gameObject.name.Contains("(Dead)"))
                gameObject.name = gameObject.name.Replace(" (Dead)", "");

            CensusMaster.Census.PopulationDecrease(gameObject.name);
        }
    }
}
