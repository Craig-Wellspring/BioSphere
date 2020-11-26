using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Cinemachine;

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
    public CinemachineVirtualCamera currentTarget;
    [Space(15)]
    public CinemachineVirtualCamera lifeGuardian;
    public List<CinemachineVirtualCamera> soullessCreatures;
    CinemachineBrain camBrain;
    PlayerModule playerMod;
    FreeCamera freeCam;


    void Start()
    {
        freeCam = GetComponent<FreeCamera>();
        camBrain = GetComponent<CinemachineBrain>();
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
                        SwitchCamTo(lifeGuardian);

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

                // KeypadPlus makes currently viewed creature lay an egg and takes control of that egg
                if (Input.GetKeyDown(KeyCode.KeypadPlus) && currentTarget != lifeGuardian)
                {
                    Reproduction ovary = currentTarget.transform.root.GetComponentInChildren<Reproduction>();
                    if (ovary)
                    {
                        GameObject egg = ovary.SpawnEgg(Mathf.Min(currentTarget.transform.root.GetComponent<EnergyData>().energyReserve, 50));
                        SwitchCamTo(egg.GetComponentInChildren<CinemachineVirtualCamera>());
                    }
                }
            }
        }
    }

    public void SwitchCamTo(CinemachineVirtualCamera _targetCam)
    {
        currentTarget.enabled = false;
        currentTarget = _targetCam;
        currentTarget.enabled = true;
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
            SwitchCamTo(soullessCreatures[creatureIndex]);

            Debug.Log("Viewing creature #" + (creatureIndex + 1) + " : " + currentTarget.transform.root.name);
        }
    }



    void CycleThroughSpecies(bool _countUp)
    {
        // Generate a list of available cams by GameObject and sort alphabetically
        List<GameObject> creatureList = new List<GameObject>();
        soullessCreatures.ForEach(x => creatureList.Add(x.transform.root.gameObject));
        creatureList.Sort();


        foreach (GameObject obj in creatureList)
        {
            if (_countUp)
            {
                if (System.String.Compare(obj.name, currentTarget.transform.root.name.Replace(" (Dead)", string.Empty)) > 0)
                {
                    SwitchCamTo(obj.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>());
                    break;
                }
            }
            else
            {
                if (System.String.Compare(obj.name, currentTarget.transform.root.name.Replace(" (Dead)", string.Empty)) < 0)
                {
                    SwitchCamTo(obj.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>());
                    break;
                }
            }


            Debug.Log("Viewing creature #" + soullessCreatures.IndexOf(currentTarget) + " : " + currentTarget.transform.root.name);
        }


        /*
        // Check if there are multiple species
        foreach (CinemachineVirtualCamera creature in soullessCreatures)
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
                    while (System.String.Compare(currentTarget.transform.root.name.Replace(" (Dead)", string.Empty), soullessCreatures[creatureIndex].transform.root.name, true) >= 0)
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
                        if (attempts > 1000) //soullessCreatures.Count)
                        {
                            Debug.Log("No other species exist");
                            break;
                        }
                    }
                }

                // Disable camera on this creature, move to target creature, and enable camera
                SwitchCamTo(soullessCreatures[creatureIndex]);


                Debug.Log("Viewing creature #" + (creatureIndex + 1) + " : " + currentTarget.transform.root.name);

                break;
            }
        }*/
    }
}
