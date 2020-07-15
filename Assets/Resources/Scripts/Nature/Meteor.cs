using UnityEngine;

public class Meteor : AdvancedMonoBehaviour
{
    public GameObject seedObject;
    public GameObject guardian;
    public float energyStored;
    
    private void OnTriggerEnter(Collider other)
    {
        //When Meteor collides with the Atmosphere: Activate Trail particles and move ServerCam
        if (other.name == "Atmosphere")
        {
            //Activate Trail
            transform.Find("Trail").gameObject.SetActive(true);

            //Attach Camera to Meteor entering Atmosphere
            ServiusCam.Cam.transform.SetParent(transform.Find("CameraDock"), false);
            ServiusCam.Cam.ResetTransform();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //When Meteor collides with Ground: Destroy, detach ServerCam, and Plant Seed

        if (collision.collider.tag == "Ground")
        {

            //Create Explosion
            transform.Find("Impact").gameObject.SetActive(true);

            //Spawn Guardian
            guardian.transform.position = PointOnTerrainUnderPosition(transform.position);
            guardian.SetActive(true);
            guardian.transform.SetParent(null);


            //Spawn Seed with Energy onboard
            PlantSeed(energyStored);
            energyStored = 0;

            //Despawn self
            GetComponent<GravityAttract>().enabled = false;
            GetComponent<SphereCollider>().enabled = false;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            transform.Find("Trail").GetComponent<ParticleSystem>().Stop();
            transform.Find("Rock").gameObject.SetActive(false);

            Destroy(gameObject, 30);
        }
    }

    public void PlantSeed(float _passDownEnergy)
    {
        //Find seed planting location
        Quaternion newRot = Quaternion.FromToRotation(transform.root.up, (transform.root.position - Vector3.zero).normalized) * transform.root.rotation;
        //Plant Seedgrass
        GameObject newSeed = (GameObject)Instantiate(seedObject, PointOnTerrainUnderPosition(transform.position), newRot);
        newSeed.name = seedObject.name;

        //Pass on remaining energy
        FoodData seedFData = newSeed.GetComponentInChildren<FoodData>();
        seedFData.energyStored = _passDownEnergy - seedFData.nutritionalValue;
    }
}
