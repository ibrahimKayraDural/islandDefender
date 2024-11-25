using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestReactor : MonoBehaviour
{
    [SerializeField] Material red;

    MeshRenderer MR;

    public void React()
    {
        GetComponent<MeshRenderer>().sharedMaterial = red;
    }
}
