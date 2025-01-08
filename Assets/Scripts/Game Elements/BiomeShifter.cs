using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Biome
{
    [System.Serializable]
    public class BiomeMaterialData
    {
        public Texture MainTexture;
        public float Metallic = 0;
        public float Smoothness = 0.5f;
        public Vector2 Tiling = Vector2.zero;
    }

    public class BiomeShifter : MonoBehaviour
    {
        [SerializeField] float _TransitionDuration = 1;
        [SerializeField] MeshRenderer _MeshRenderer;

        Material _mat;

        void Awake()
        {
            _mat = _MeshRenderer.material;
        }

        public void ShiftBiome(BiomeMaterialData bmd)
        {
            if (Handle_FillProgress != null) StopCoroutine(Handle_FillProgress);

            BiomeMaterialData oldMat = new BiomeMaterialData();

            oldMat.MainTexture = _mat.GetTexture("_MainTex");
            oldMat.Metallic = _mat.GetFloat("_Metallic");
            oldMat.Smoothness = _mat.GetFloat("_Smoothness");
            oldMat.Tiling = _mat.GetVector("_Tiling");

            _mat.SetTexture("_OldMainTex", oldMat.MainTexture);
            _mat.SetFloat("_OldMetallic", oldMat.Metallic);
            _mat.SetFloat("_OldSmoothness", oldMat.Smoothness);
            _mat.SetVector("_OldTiling", oldMat.Tiling);

            _mat.SetFloat("Fill", 0);

            _mat.SetTexture("_MainTex", bmd.MainTexture);
            _mat.SetFloat("_Metallic", bmd.Metallic);
            _mat.SetFloat("_Smoothness", bmd.Smoothness);
            _mat.SetVector("_Tiling", bmd.Tiling);

            Handle_FillProgress = FillProgress();
            StartCoroutine(Handle_FillProgress);
        }

        IEnumerator Handle_FillProgress;
        IEnumerator FillProgress()
        {
            float step = .05f;
            float progress = 0;

            while(progress < _TransitionDuration)
            {
                progress += step;
                _mat.SetFloat("_Fill", progress / _TransitionDuration);
                yield return new WaitForSeconds(step);
            }

            _mat.SetFloat("_Fill", 1);
        }
    }
}
