using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Pathfinding;

public class PlayerSoul : AdvancedMonoBehaviour
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

    public Cinemachine.CinemachineVirtualCamera currentTarget;
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
            NNConstraint nn = NNConstraint.Default;

            nn.graphMask = 1 << 0 | 1 << 1 | 1 << 2 | 1 << 3 | 1 << 4 | 1 << 5 | 1 << 6 | 1 << 7 | 1 << 8 | 1 << 9 | 1 << 10 | 1 << 11 | 1 << 12 | 1 << 13 | 1 << 14 | 1 << 15 | 1 << 16 | 1 << 17 | 1 << 18 | 1 << 19 | 1 << 20 | 1 << 21 | 1 << 22 | 1 << 23;

            var info = AstarPath.active.GetNearest(transform.position, nn);
            Debug.Log(info.node.position);

            
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
