using UnityEngine;
using static UnityEngine.GraphicsBuffer;


namespace Mekaiju.DestructionStructures { 
    public class Destructible : MonoBehaviour
    {
        public GameObject newMeshPrefab; // Prefab de la structure endommagée

        public void getDestruct()
        {
            _ChangeMesh(this.gameObject, newMeshPrefab);
        }

        private void _ChangeMesh(GameObject p_target, GameObject p_prefab)
        {
            if (p_target == null || p_prefab == null)
            {
                // évite les erreurs s'il n'y a pas de Prefab 
                Debug.LogError("Target or Prefab is null!");
                return;
            }

            MeshFilter t_targetMeshFilter = p_target.GetComponent<MeshFilter>();
            MeshRenderer t_targetMeshRenderer = p_target.GetComponent<MeshRenderer>();

            MeshFilter t_prefabMeshFilter = p_prefab.GetComponent<MeshFilter>();
            MeshRenderer t_prefabMeshRenderer = p_prefab.GetComponent<MeshRenderer>();

            if (t_targetMeshFilter == null || t_prefabMeshFilter == null)
            {
                Debug.LogError("MeshFilter missing on target or prefab!");
                return;
            }

            // Remplace le mesh
            t_targetMeshFilter.mesh = t_prefabMeshFilter.sharedMesh;

            // Remplace le renderer si nécessaire
            if (t_targetMeshRenderer != null && t_prefabMeshRenderer != null)
            {
                t_targetMeshRenderer.sharedMaterials = t_prefabMeshRenderer.sharedMaterials;
            }
        }
    }
}