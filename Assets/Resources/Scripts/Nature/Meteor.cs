using UnityEngine;

public class Meteor : MonoBehaviour
{
    public GameObject seedObject;
    public float energyStored;
    

    private void OnTriggerEnter(Collider other)
    {
        //When Meteor collides with the Atmosphere: Activate Trail particles and move ServerCam
        if (other.name == "Horizon")
        {
            //Activate Trail
            transform.Find("Trail").gameObject.SetActive(true);

            //Move camera to follow Meteor entering Atmosphere
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

            //Detach ServerCam
            ServiusCam.Cam.transform.SetParent(null);

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
        GameObject newSeed = (GameObject)Instantiate(seedObject, PointOnTerrainUnderPosition(), newRot);
        newSeed.name = seedObject.name;

        //Pass on remaining energy
        FoodData seedFData = newSeed.GetComponentInChildren<FoodData>();
        seedFData.energyStored = _passDownEnergy - seedFData.nutritionalValue;
    }


    public Vector3 PointOnTerrainUnderPosition()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position + (transform.position - Vector3.zero).normalized, -(transform.position - Vector3.zero).normalized, out hit, 2000, LayerMask.GetMask("Terrain")))
        {
            if (hit.collider.CompareTag("Ground"))
                return hit.point;
            else return Vector3.zero;
        }
        else return Vector3.zero;
    }
}
