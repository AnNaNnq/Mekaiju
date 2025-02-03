using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class destructible : MonoBehaviour
{
    public GameObject newMeshPrefab; // Prefab de la structure endommag�e

    public void getDestruct()
    {
        ChangeMesh(this.gameObject, newMeshPrefab);
    }

    public void ChangeMesh(GameObject target, GameObject prefab)
    {
        if (target == null || prefab == null)
        {
            Debug.LogError("Target or Prefab is null!");
            return;
        }

        MeshFilter targetMeshFilter = target.GetComponent<MeshFilter>();
        MeshRenderer targetMeshRenderer = target.GetComponent<MeshRenderer>();

        MeshFilter prefabMeshFilter = prefab.GetComponent<MeshFilter>();
        MeshRenderer prefabMeshRenderer = prefab.GetComponent<MeshRenderer>();

        if (targetMeshFilter == null || prefabMeshFilter == null)
        {
            Debug.LogError("MeshFilter missing on target or prefab!");
            return;
        }

        // Remplace le mesh
        targetMeshFilter.mesh = prefabMeshFilter.sharedMesh;

        // Remplace le renderer si n�cessaire
        if (targetMeshRenderer != null && prefabMeshRenderer != null)
        {
            targetMeshRenderer.sharedMaterials = prefabMeshRenderer.sharedMaterials;
        }
    }
}
