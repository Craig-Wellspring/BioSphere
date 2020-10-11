using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FoliageVariantData", menuName = "Foliage Variant Data")]
public class FoliageVariantData : ScriptableObject
{
    [SerializeField] string FoliageName = "Foliage";
    public string foliageName { get { return FoliageName; } }

    [SerializeField] Mesh TrunkMesh;
    public Mesh trunkMesh { get { return TrunkMesh; } }


    [Space(15)]
    [SerializeField] List<Vector3> MajorBudPositions;
    public List<Vector3> majorBudPositions { get { return MajorBudPositions; } }
    [SerializeField] List<Mesh> MajorBudMeshes;
    public List<Mesh> majorBudMeshes { get { return MajorBudMeshes; } }


    [Space(15)]
    [SerializeField] List<Vector3> MinorBudPositions;
    public List<Vector3> minorBudPositions { get { return MinorBudPositions; } }
    [SerializeField] List<Mesh> MinorBudMeshes;
    public List<Mesh> minorBudMeshes { get { return MinorBudMeshes; } }
}
