using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModule : AdvancedMonoBehaviour
{
    public bool isControlled = false;
    GameObject aiModule;

    private bool playMode = true;
    private void OnApplicationQuit()
    {
        playMode = false;
    }

    private void OnEnable()
    {
        //Register
        PlayerSoul.Cam.soullessCreatures.Add(GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>());

        aiModule = transform.root.GetComponentInChildren<BasicAIBrain>().gameObject;
    }

    private void OnDisable()
    {
        if (playMode)
        {
            //Move Camera back to Guardian
            if (PlayerSoul.Cam.currentTarget == GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>())
            {
                PlayerSoul.Cam.currentTarget = PlayerSoul.Cam.lifeGuardian;
                PlayerSoul.Cam.currentTarget.enabled = true;
            }

            //Unregister
            PlayerSoul.Cam.soullessCreatures.Remove(GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>());
        }
    }

    public void TakeControl()
    {
        isControlled = true;

        //Enable Controller
        GetComponent<PlayerController>().enabled = true;

        //Disable AI
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
