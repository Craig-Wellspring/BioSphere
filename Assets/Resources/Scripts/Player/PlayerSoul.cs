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
    FreeCamera freeCam;


    void Start()
    {
        freeCam = GetComponent<FreeCamera>();
        camBrain = GetComponent<Cinemachine.CinemachineBrain>();
    }

    void Update()
    {
        KeypadControls();
    }


    void KeypadControls()
    {
        // KeypadEnter takes and releases control of currently viewed creature
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
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
            // Keypad0 detaches camera
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                if (freeCam.enabled)
                {
                    camBrain.enabled = true;
                    freeCam.enabled = false;
                }
                else
                {
                    camBrain.enabled = false;
                    freeCam.enabled = true;
                }
            }

            if (!freeCam.enabled)
            {
                // Keypad5 returns Camera to following the Guardian
                if (Input.GetKeyDown(KeyCode.Keypad5))
                {
                    if (lifeGuardian != null && !lifeGuardian.enabled)
                    {
                        currentTarget.enabled = false;
                        currentTarget = lifeGuardian;
                        currentTarget.enabled = true;

                        Debug.Log("Viewing: " + currentTarget.transform.root.name);
                    }
                }

                // Keypad4/Left moves Camera to previous individual
                if (Input.GetKeyDown(KeyCode.Keypad4))
                    CycleThroughIndividuals(false);

                // Keypad6/Right moves Camera to next individual
                if (Input.GetKeyDown(KeyCode.Keypad6))
                    CycleThroughIndividuals(true);


                // Keypad2/Down moves Camera to previous species
                if (Input.GetKeyDown(KeyCode.Keypad2))
                    CycleThroughSpecies(false);

                // Keypad8/Up moves Camera to next species
                if (Input.GetKeyDown(KeyCode.Keypad8))
                    CycleThroughSpecies(true);
            }
        }
    }

    void CycleThroughIndividuals(bool _countUp)
    {
        if (soullessCreatures.Count > 0)
        {
            // Attach Camera to next creature in the list
            int creatureIndex = 0;
            if (!lifeGuardian.enabled)
            {
                if (_countUp)
                    creatureIndex = soullessCreatures.IndexOf(currentTarget) + 1;
                else creatureIndex = soullessCreatures.IndexOf(currentTarget) - 1;

                // Loop the list
                if (creatureIndex < 0)
                    creatureIndex = soullessCreatures.Count - 1;
                if (creatureIndex > soullessCreatures.Count - 1)
                    creatureIndex = 0;


                // Find the next creature with the same name as the current target
                int attempts = 0;
                while (soullessCreatures[creatureIndex].transform.root.name != currentTarget.transform.root.name.Replace(" (Dead)", string.Empty))
                {
                    if (_countUp)
                        creatureIndex++;
                    else creatureIndex--;

                    // Loop the list
                    if (creatureIndex < 0)
                        creatureIndex = soullessCreatures.Count - 1;
                    if (creatureIndex > soullessCreatures.Count - 1)
                        creatureIndex = 0;

                    attempts++;
                    if (attempts > soullessCreatures.Count)
                    {
                        Debug.Log("No other individuals of this species exist");
                        break;
                    }
                }
            }


            // Disable camera on this creature, move to target creature, and enable camera
            currentTarget.enabled = false;
            currentTarget = soullessCreatures[creatureIndex];
            currentTarget.enabled = true;

            Debug.Log("Viewing creature #" + (creatureIndex + 1) + " : " + currentTarget.transform.root.name);
        }
    }


    void CycleThroughSpecies(bool _countUp)
    {
        // Check if there are multiple species
        foreach (Cinemachine.CinemachineVirtualCamera creature in soullessCreatures)
        {
            if (creature.transform.root.name != soullessCreatures[0].transform.root.name)
            {
                // Attach Camera to next creature in the list
                int creatureIndex = 0;
                if (!lifeGuardian.enabled)
                {
                    if (_countUp)
                        creatureIndex = soullessCreatures.IndexOf(currentTarget) + 1;
                    else creatureIndex = soullessCreatures.IndexOf(currentTarget) - 1;

                    // Loop the list
                    if (creatureIndex < 0)
                        creatureIndex = soullessCreatures.Count - 1;
                    if (creatureIndex > soullessCreatures.Count - 1)
                        creatureIndex = 0;

                    // Find the next creature with a different name than the current target
                    int attempts = 0;
                    while (soullessCreatures[creatureIndex].transform.root.name == currentTarget.transform.root.name.Replace(" (Dead)", string.Empty))
                    {
                        if (_countUp)
                            creatureIndex++;
                        else creatureIndex--;

                        // Loop the list
                        if (creatureIndex < 0)
                            creatureIndex = soullessCreatures.Count - 1;
                        if (creatureIndex > soullessCreatures.Count - 1)
                            creatureIndex = 0;

                        attempts++;
                        if (attempts > soullessCreatures.Count)
                        {
                            Debug.Log("No other species exist");
                            break;
                        }
                    }
                }

                // Disable camera on this creature, move to target creature, and enable camera
                currentTarget.enabled = false;
                currentTarget = soullessCreatures[creatureIndex];
                currentTarget.enabled = true;


                Debug.Log("Viewing creature #" + (creatureIndex + 1) + " : " + currentTarget.transform.root.name);

                break;
            }
        }
    }
}
