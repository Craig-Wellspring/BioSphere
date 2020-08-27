using UnityEngine;

public class GrowthData : MonoBehaviour
{
    [Header("Settings")]
    public float growthSpeed = 5;
    [Tooltip("Shade local terrain green when Half Grown"),SerializeField] 
    bool colorizeTerrain = true;

    [Header("Half Grown")]
    public bool halfGrown = false;
    [Tooltip("Change the Tag of Root transform to this string when Half Grown"), SerializeField]
    private string halfGrownTag;
    public GameObject activateObjectHG;
    [Tooltip("Detach from Parent when Half Grown")]
    public bool gainIndependenceHG = false;


    [Header("Fully Grown")]
    public bool fullyGrown = false;
    [Tooltip("Change the Tag of Root transform to this string when Fully Grown"), SerializeField]
    private string fullyGrownTag;
    public GameObject activateObjectFG;
    [Tooltip("Detach from Parent when FullyGrown")]
    public bool gainIndependenceFG = false;


    private Animator anim;

    private void Start()
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
            transform.SetParent(null);

        if (colorizeTerrain)
            GetComponent<TerrainColorizer>().enabled = true;
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
            transform.SetParent(null);
    }
}