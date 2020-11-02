using UnityEngine;

public class Meteor : MonoBehaviour
{
    public GameObject guardian;

    void OnTriggerEnter(Collider other)
    {
        //When Meteor collides with the Atmosphere: Activate Trail particles and move ServerCam
        if (other.name == "Atmosphere")
        {
            //Activate Trail
            transform.Find("Trail").gameObject.SetActive(true);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //When Meteor collides with Ground: Destroy, detach ServerCam, and Plant Seed
        //Create Explosion
        transform.Find("Impact").gameObject.SetActive(true);

        //Spawn Guardian
        guardian.transform.position = UtilityFunctions.GroundBelowPosition(UtilityFunctions.PositionAbove(transform)).position;
        guardian.transform.SetParent(null);
        guardian.SetActive(true);
        PlayerSoul.Cam.currentTarget = guardian.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();


        //Despawn self
        GetComponent<GravityAttract>().enabled = false;
        GetComponent<SphereCollider>().enabled = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        transform.Find("Trail").GetComponent<ParticleSystem>().Stop();
        transform.Find("Rock").gameObject.SetActive(false);

        Destroy(gameObject, 30);

        //Deactivate Camera
        GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().enabled = false;
    }
}
