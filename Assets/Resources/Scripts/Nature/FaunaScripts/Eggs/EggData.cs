using UnityEngine;

[CreateAssetMenu(fileName = "New EggData", menuName = "Egg Data")]
public class EggData : ScriptableObject
{
    [SerializeField] string EggName = "Egg";
    public string eggName { get { return EggName; } }


    [SerializeField] GameObject OffspringCreature;
    public GameObject offspringCreature { get { return OffspringCreature; } }


    [SerializeField] Color EggColor = Color.white;
    public Color eggColor { get { return EggColor; } }


    [SerializeField] float EggSize = 1f;
    public float eggSize { get { return EggSize; } }
}
