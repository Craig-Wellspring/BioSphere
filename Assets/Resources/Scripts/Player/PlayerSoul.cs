using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Pathfinding;

public class PlayerSoul : MonoBehaviour
{
    #region Singleton
    public static PlayerSoul Cam { get; private set; }
    private void Awake()
    {
        if (Cam == null)
            Cam = this;
        else
            Destroy(gameObject); //should never happen
    }
    #endregion

   
    [Header("Camera Docks")]
    public Cinemachine.CinemachineVirtualCamera currentTarget;
    [Space(15)]
    public Cinemachine.CinemachineVirtualCamera lifeGuardian;
    public List<Cinemachine.CinemachineVirtualCamera> soullessCreatures;
    Cinemachine.CinemachineBrain camBrain;
    PlayerModule playerMod;
    FreeCamera camControl;


    private void Start()
    {
        camControl = GetComponent<FreeCamera>();
        camBrain = GetComponent<Cinemachine.CinemachineBrain>();
    }

    private void Update()
    {
        CameraControls();
        CustomControls();
    }

    private void CustomControls()
    {
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {          
        }
    }

    private void CameraControls()
    {
        // Return Key takes and releases control of currently viewed creature
        if (Input.GetKeyDown(KeyCode.Return))
        {
            playerMod = currentTarget.GetComponentInParent<PlayerModule>();
            if (playerMod != null)
            {
                if (playerMod.isControlled)
                {
                    playerMod.ReleaseControl();
                    playerMod = null;
                }
                else
                    playerMod.TakeControl();
            }
        }

        if (playerMod == null || !playerMod.isControlled)
        {
            // Up Arrow detaches camera
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (camControl.enabled)
                {
                    camBrain.enabled = true;
                    camControl.enabled = false;
                }
                else
                {
                    camBrain.enabled = false;
                    camControl.enabled = true;
                }
            }

            if (!camControl.enabled)
            {
                // Down Arrow returns Camera to following the Guardian
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (lifeGuardian != null && !lifeGuardian.enabled)
                    {
                        currentTarget.enabled = false;
                        currentTarget = lifeGuardian;
                        currentTarget.enabled = true;
                    }
                }

                if (soullessCreatures.Count > 0)
                {
                    // Right Arrow moves Camera to next target
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                        CycleCameraTarget(true);

                    // Left Arrow moves Camera to previous target
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                        CycleCameraTarget(false);

                }
            }
        }
    }

    private void CycleCameraTarget(bool _countUp)
    {
        //Attach Camera to next creature in the list
        int creatureIndex = 0;
        if (!lifeGuardian.enabled)
        {
            if (_countUp)
                creatureIndex = soullessCreatures.IndexOf(currentTarget) + 1;
            else creatureIndex = soullessCreatures.IndexOf(currentTarget) - 1;
        }

        if (creatureIndex < 0)
            creatureIndex = soullessCreatures.Count - 1;
        if (creatureIndex > soullessCreatures.Count - 1)
            creatureIndex = 0;

        currentTarget.enabled = false;
        currentTarget = soullessCreatures[creatureIndex];
        currentTarget.enabled = true;

        Debug.Log("Viewing creature #" + (creatureIndex + 1) + " : " + currentTarget.transform.root.name);
    }
}
