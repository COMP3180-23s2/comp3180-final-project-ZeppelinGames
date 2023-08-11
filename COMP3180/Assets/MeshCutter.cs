using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCutter : MonoBehaviour
{
    [SerializeField] private GameObject cut;
    [SerializeField] private int maxCuts;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < maxCuts; i++)
        {
            Cutter.Cut(cut, transform.position + Random.insideUnitSphere, Random.insideUnitSphere);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
