using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace Mekaiju
{
    [Serializable]
    public class MeshTrailConfig
    {
        [Header("Mesh related")]
        public float refreshRate  = 0.075f;
        public float destroyDelay = 1f;

        [Header("Shader related")]
        public Material material;
    }

    public class MeshTrailTut : MonoBehaviour
    {
        public MeshTrailConfig config;

        private SkinnedMeshRenderer[] _skinnedMeshRenderers;

        public void Trigger(float time)
        {
            StartCoroutine(_ActivateTrail(time));
        }

        private IEnumerator _ActivateTrail(float time)
        {
            float t_totalTime = time;
            while (time > 0)
            {
                time -= Mathf.Max(config.refreshRate, Time.deltaTime);
                _skinnedMeshRenderers ??= GetComponentsInChildren<SkinnedMeshRenderer>();

                for (int i = 0; i < _skinnedMeshRenderers.Length; i++)
                {
                    GameObject t_go = new GameObject();

                    t_go.transform.SetPositionAndRotation(transform.position, transform.rotation);

                    MeshRenderer t_mr = t_go.AddComponent<MeshRenderer>();
                    MeshFilter   t_mf = t_go.AddComponent<MeshFilter>();

                    // config.material.SetFloat("_Progress", (t_totalTime - time) / t_totalTime);
                    Mesh mesh = new Mesh();
                    _skinnedMeshRenderers[i].BakeMesh(mesh);

                    t_mf.mesh     = mesh;
                    t_mr.material = config.material;

                    Destroy(t_go, config.destroyDelay);
                }
                yield return new WaitForSeconds(config.refreshRate);
            }
        }
    }
}