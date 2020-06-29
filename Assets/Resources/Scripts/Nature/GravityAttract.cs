using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttract : MonoBehaviour
{
    private void FixedUpdate()
    {
        PlanetCore.Core.Attract(transform);
    }
}
