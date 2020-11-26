using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModule : MonoBehaviour
{
    public bool isControlled = false;
    GameObject aiModule;
    Animator baseAnim;
    Cinemachine.CinemachineVirtualCamera vCam;

    private bool playMode = true;
    private void OnApplicationQuit()
    {
        playMode = false;
    }

    void OnEnable()
    {
        //Cache
        aiModule = transform.root.GetComponentInChildren<BasicAIBrain>().gameObject;
        baseAnim = transform.root.GetComponent<Animator>();
        vCam = GetComponent<Cinemachine.CinemachineVirtualCamera>();

        // Register events
        transform.root.GetComponentInChildren<Vitality>().DeathOccurs += UnregisterCam;

        //Register Camera
        PlayerSoul.Cam?.soullessCreatures.Add(vCam);
    }

    void OnDisable()
    {
        if (playMode)
        {
            // Unregister events
            Vitality vitality = transform.root.GetComponentInChildren<Vitality>();
            if (vitality != null)
                vitality.DeathOccurs -= UnregisterCam;

            //Move Camera back to Guardian
            if (PlayerSoul.Cam.currentTarget == vCam)
            {
                PlayerSoul.Cam.currentTarget = PlayerSoul.Cam.lifeGuardian;
                PlayerSoul.Cam.currentTarget.enabled = true;
            }

            UnregisterCam();
        }
    }

    void UnregisterCam()
    {
        PlayerSoul.Cam.soullessCreatures.Remove(vCam);
    }

    public void TakeControl()
    {
        isControlled = true;

        //Enable Controller
        GetComponent<PlayerController>().enabled = true;
        transform.root.GetComponent<GravityAttract>().enabled = true;

        //Disable AI
        aiModule.GetComponent<BasicAIBrain>().ClearPathing();
        aiModule.gameObject.SetActive(false);

        if (UtilityFunctions.AnimatorHasParameter(baseAnim, "IsSinging"))
            baseAnim.SetBool("IsSinging", false);

        Metabolism metabolism = transform.root.GetComponentInChildren<Metabolism>();
        if (metabolism)
        {
            if (metabolism.isEating)
                metabolism.StopEating();

            if (!metabolism.dietList.HasFlag(Metabolism.FoodCategories.Meat))
                metabolism.dietList |= Metabolism.FoodCategories.Meat;
        }
    }

    public void ReleaseControl()
    {
        isControlled = false;

        //Disable Controller
        GetComponent<PlayerController>().enabled = false;
        transform.root.GetComponent<GravityAttract>().enabled = false;

        //Enable AI
        aiModule.gameObject.SetActive(true);
    }
}
