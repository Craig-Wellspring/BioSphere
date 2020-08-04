using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModule : AdvancedMonoBehaviour
{
        GameObject aiModule;
        Animator animator;
        Metabolism metabolism;

    private void OnEnable(){
        aiModule = transform.root.GetComponentInChildren<BasicAIBrain>().gameObject;
        animator = transform.root.GetComponent<Animator>();
        metabolism = GetComponentInParent<Metabolism>();

        //Attach Camera
        ServiusCam.Cam.transform.SetParent(transform.Find("CameraDock"), false);
        ResetTransform(ServiusCam.Cam.transform);

        //Disable AI
        aiModule.SetActive(false);
    }

    private void OnDisable(){
        //Detach Camera
        ServiusCam.Cam.transform.SetParent(null);

        //Enable AI
        aiModule.SetActive(true);
    }

    private void Update(){
        if (Input.GetKeyDown(KeyCode.E) && !animator.GetBool("IsEating")){
            animator.SetTrigger("Bite");
        }
    }
}
