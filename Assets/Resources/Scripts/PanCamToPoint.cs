using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanCamToPoint : MonoBehaviour
{
    public float speed = 1;

    private Transform destination;


    private void Start()
    {
        destination = FindObjectOfType<CamPoint>().transform;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, destination.position, Time.deltaTime * speed);
        transform.rotation = Quaternion.Lerp(transform.rotation, destination.rotation, Time.deltaTime * speed);

        if (transform.position == destination.position)
            this.enabled = false;
    }
    
}
