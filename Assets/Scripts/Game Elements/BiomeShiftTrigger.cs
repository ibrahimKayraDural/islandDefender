using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Biome
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class BiomeShiftTrigger : MonoBehaviour
    {
        [SerializeField] BiomeMaterialData _BiomeMaterialData;
        [SerializeField] BiomeShifter _BiomeShifter;
        [SerializeField] List<string> _TagsToCheck = new List<string>() { "OverworldPlayer" };

        void Awake()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            Collider col = GetComponent<Collider>();

            rb.isKinematic = true;
            col.isTrigger = true;

            if (_BiomeShifter == null)
            {
                _BiomeShifter = FindObjectOfType<BiomeShifter>();
                if (_BiomeShifter == null) Debug.LogError("Biome Shifter could not be found.");
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (_TagsToCheck.Contains(other.gameObject.tag) == false) return;

            _BiomeShifter.ShiftBiome(_BiomeMaterialData);
        }
    }
}
