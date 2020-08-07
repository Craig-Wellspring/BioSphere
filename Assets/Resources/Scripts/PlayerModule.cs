using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModule : AdvancedMonoBehaviour
{
    GameObject aiModule;

    private void OnEnable()
    {
        //Attach Camera
        ServiusCam.Cam.transform.SetParent(transform.Find("CameraDock"), false);
        ResetTransform(ServiusCam.Cam.transform);

        //Disable AI
        aiModule = transform.root.GetComponentInChildren<BasicAIBrain>().gameObject;
        aiModule.SetActive(false);
    }

    private void OnDisable(){
        //Detach Camera
        ServiusCam.Cam.transform.SetParent(FindObjectOfType<CamPoint>().transform);

        //Enable AI
        aiModule.SetActive(true);
    }
}
