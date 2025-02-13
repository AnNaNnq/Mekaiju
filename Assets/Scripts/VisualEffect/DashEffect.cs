using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class MeshTrailTut : MonoBehaviour
{
    [Header("Mesh Related")]
    public float meshRefreshRate = 0.075f;
    public float meshDestroyDelay = 1f;
    public Transform positionToSpawn;

    [Header("Shader Related")]
    public Material mat;

    public VisualEffectAsset asset;

    private bool isTrailActive;
    private SkinnedMeshRenderer[] skinnedMeshRenderers;

    void Awake()
    {
        positionToSpawn = transform;
    }

    public void Trigger(float time)
    {
        StartCoroutine(ActivateTrail(time));
    }

    private IEnumerator ActivateTrail(float timeActive)
    {
        float totalTime = timeActive;
        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate;

            if (skinnedMeshRenderers == null)
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                GameObject gObj = new GameObject();

                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                MeshFilter mf = gObj.AddComponent<MeshFilter>();

                mat.SetFloat("_Progress", (totalTime - timeActive) / totalTime);
                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);

                mf.mesh = mesh;
                mr.material = mat;

                Destroy(gObj, meshDestroyDelay);
            }
            yield return new WaitForSeconds(meshRefreshRate);
        }
        isTrailActive = false;
    }
}