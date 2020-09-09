using UnityEngine;

public class GrowthData : MonoBehaviour
{
    [Header("Settings")]
    public float growthSpeed = 5;

    [Header("Half Grown")]
    public bool halfGrown = false;
    [Tooltip("Change the Tag of Root transform to this string when Half Grown"), SerializeField]
    string halfGrownTag;
    public GameObject activateObjectHG;
    [Tooltip("Detach from Parent when Half Grown")]
    public bool gainIndependenceHG = false;
    [Tooltip("Shade local terrain green by increment when Half Grown"),SerializeField] 
    float addTerrainColor = 0;


    [Header("Fully Grown")]
    public bool fullyGrown = false;
    [Tooltip("Change the Tag of Root transform to this string when Fully Grown"), SerializeField]
    string fullyGrownTag;
    public GameObject activateObjectFG;
    [Tooltip("Detach from Parent when FullyGrown")]
    public bool gainIndependenceFG = false;


    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        anim.SetFloat("GrowthSpeed", growthSpeed);
    }



    void HalfGrown()
    {
        if (halfGrownTag.Length > 0)
            transform.tag = halfGrownTag;
        halfGrown = true;

        if (anim.parameters.ToString().Contains("HalfGrown"))
            anim.SetBool("HalfGrown", true);

        if (activateObjectHG != null)
            activateObjectHG.SetActive(true);

        if (gainIndependenceHG)
            GainIndependence();

        if (addTerrainColor > 0)
        {
            gameObject.AddComponent<TerrainColorizer>();
            GetComponent<TerrainColorizer>().ColorizeTerrain(addTerrainColor);
        }
    }

    void FullyGrown()
    {
        if (fullyGrownTag.Length > 0)
            transform.tag = fullyGrownTag;
        fullyGrown = true;

        if (anim.parameters.ToString().Contains("FullyGrown"))
            anim.SetBool("FullyGrown", true);

        if (activateObjectFG != null)
            activateObjectFG.SetActive(true);

        if (gainIndependenceFG)
            GainIndependence();
    }

    void GainIndependence()
    {
        transform.SetParent(null);

        GravityAttract gBody = GetComponent<GravityAttract>();
        if (gBody)
            gBody.enabled = true;
    }
}