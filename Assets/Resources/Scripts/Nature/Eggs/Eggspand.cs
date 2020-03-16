using UnityEngine;

public class Eggspand : MonoBehaviour
{

    public float growTimeSeconds = 3f;
    public float growRate = 0.1f;
    public float maxSize = 5f;
    public float massChange = 0.2f;
    public bool canHatch = true;
    public int hatchStage = 1;
    public float hatchSize = 1f;
    public GameObject stage1Creature;


    private Vector3 scaleChange;
    private Vector3 currentScale;
    private float growTime = 0;
    private Rigidbody eggBody;

    void Start()
    {
        scaleChange = new Vector3(growRate, growRate, growRate);
        eggBody = this.GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        if (transform.GetChild(0).transform.localScale.y <= maxSize)
        {
            growTime += Time.deltaTime;
            if (growTime > growTimeSeconds)
            {
                growTime = 0;
                //Gain size and mass
                transform.GetChild(0).transform.localScale += scaleChange;
                eggBody.mass += massChange;

                //Hatch if possible
                if (transform.GetChild(0).transform.localScale.y >= hatchSize && canHatch)
                    Hatch(hatchStage);
            }
        }
    }

    private void Hatch(int stage)
    {
        if (stage == 1)
        {
            //spawn stage 1 creature
            GameObject tmp = Instantiate(stage1Creature, transform.position, stage1Creature.transform.rotation);
            tmp.name = stage1Creature.name;
        }

        currentScale = transform.GetChild(0).transform.localScale;
        GameObject topshell = transform.GetChild(1).gameObject;
        GameObject bottomshell = transform.GetChild(2).gameObject;

        topshell.SetActive(true);
        bottomshell.SetActive(true);
        topshell.transform.localScale = currentScale;
        bottomshell.transform.localScale = currentScale;
        topshell.transform.SetParent(null);
        bottomshell.transform.SetParent(null);

        Destroy(this.gameObject);

    }
}
