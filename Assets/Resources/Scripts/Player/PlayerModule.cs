using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PlayerModule : AdvancedMonoBehaviour
{
    public bool isControlled = false;
    GameObject aiModule;
    Animator baseAnim;
    Vitality vitality;
    Cinemachine.CinemachineVirtualCamera vCam;

    private bool playMode = true;
    private void OnApplicationQuit()
    {
        playMode = false;
    }

    private void OnEnable()
    {
        //Cache
        aiModule = transform.root.GetComponentInChildren<BasicAIBrain>().gameObject;
        baseAnim = transform.root.GetComponent<Animator>();
        vitality = GetComponentInParent<Vitality>();
        vCam = GetComponent<Cinemachine.CinemachineVirtualCamera>();


        //Register
        vitality.DeathOccurs += Death;

        PlayerSoul.Cam?.soullessCreatures.Add(vCam);
    }

    private void OnDisable()
    {
        if (playMode)
        {
            //Move Camera back to Guardian
            if (PlayerSoul.Cam.currentTarget == vCam)
            {
                PlayerSoul.Cam.currentTarget = PlayerSoul.Cam.lifeGuardian;
                PlayerSoul.Cam.currentTarget.enabled = true;
            }

            //Unregister
            vitality.DeathOccurs -= Death;

            PlayerSoul.Cam.soullessCreatures.Remove(vCam);
        }
    }

    void Death()
    {
        gameObject.SetActive(false);
    }

    public void TakeControl()
    {
        //Enable Controller
        isControlled = true;
        GetComponent<PlayerController>().enabled = true;

        //Disable AI
        transform.root.GetComponent<AIDestinationSetter>().target = null;

        if (baseAnim.GetBool("IsSinging"))
            baseAnim.SetBool("IsSinging", false);
        if (baseAnim.GetBool("IsEating"))
            GetComponentInParent<Metabolism>().StopEating();


        aiModule.gameObject.SetActive(false);
    }

    public void ReleaseControl()
    {
        isControlled = false;

        //Disable Controller
        GetComponent<PlayerController>().enabled = false;

        //Enable AI
        aiModule.gameObject.SetActive(true);
    }
}
