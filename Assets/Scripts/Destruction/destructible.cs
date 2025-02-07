using UnityEditor.UI;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


namespace Mekaiju.DestructionStructures { 
    public class Destructible : MonoBehaviour
    {
        public GameObject newMeshPrefab; // Destroyed structure's Prefab
        public GameObject SmokePrefab; // Smoke Particles' Prefab
        public GameObject ImpactPrefab; // Impact Prefab
        public float sizeRatioDefault = 1/8; // A ratio between this object's size and the player's size
        public float sizeRatioHV = 1/4; // A ratio between this object's size and the player's size if the player moves at high Velocity
        public float Mass = 500; // Mass needed to break this object

        private GameObject self;
        private bool isDestroyed = false; // Structure's State

        private void Start()
        {
            self = transform.gameObject; // Define the reference to this object
        }

        private void OnTriggerEnter(Collider p_other)
        {

            // Check if a player hitbox is colliding with this object
            if (p_other.CompareTag("Player") && !isDestroyed)
            {
                
                // Checking this object size in order to know if it's 
                float t_OtherHeight = GetComponent<Collider>().bounds.size.y;
                float t_PlayerHeight = p_other.bounds.size.y;

                // Checking Player's Velocity to detect High Velocity movements
                if (p_other.attachedRigidbody.mass > 300 && ((sizeRatioHV * t_PlayerHeight) >= t_OtherHeight))
                {

                    _getDestroyed(newMeshPrefab);
                }
                else if ((sizeRatioDefault*t_PlayerHeight) >= t_OtherHeight)
                {
                    print("ici");
                    _getDestroyed(newMeshPrefab);
                }
            }
            else if (p_other.CompareTag("Player"))
            {
                // _CreateImpact(p_other);
            }
        }

        private void _getDestroyed(GameObject p_prefab)
        {
            // Refreshing the Structure's State
            isDestroyed = true;

            // Check if the prefabs have been defined to avoid errors
            if ( p_prefab == null)
            {
                Debug.LogError("Target or Prefab is null!");
                return;
            }
            if (SmokePrefab != null)
            {
                // Avoid compiling errors if no smokePrefab is attached to this Script
                // Manage Smoke Particles Prefab Spawn
                Instantiate(SmokePrefab, self.transform.position, Quaternion.identity);
            }

            MeshFilter t_targetMeshFilter = self.GetComponent<MeshFilter>();
            MeshRenderer t_targetMeshRenderer = self.GetComponent<MeshRenderer>();

            MeshFilter t_prefabMeshFilter = p_prefab.GetComponent<MeshFilter>();
            MeshRenderer t_prefabMeshRenderer = p_prefab.GetComponent<MeshRenderer>();

            // Replacing the current mesh
            t_targetMeshFilter.mesh = t_prefabMeshFilter.sharedMesh;

            // Replace the renderer if needed
            if (t_targetMeshRenderer != null && t_prefabMeshRenderer != null)
            {
                t_targetMeshRenderer.sharedMaterials = t_prefabMeshRenderer.sharedMaterials;
            }
        }


        // WIP
        //private void _CreateImpact(Collider p_wall)
        //{
        //    Vector3 t_impactPosition = p_wall.ClosestPoint(transform.position);

        //    // Instantiate Impact Particles
        //    if (ImpactPrefab != null)
        //    {
        //        GameObject t_impact = Instantiate(ImpactPrefab, t_impactPosition, Quaternion.identity);
        //        t_impact.transform.up = p_wall.transform.forward; // Aligner l'impact sur le mur
        //    }

        //    // Instantiate Smoke Particles
        //    if (SmokePrefab != null)
        //    {

        //        GameObject t_smoke = Instantiate(SmokePrefab, t_impactPosition, Quaternion.identity);
        //        print("effet de particules");
        //        Vector3 t_direction = (transform.position - t_impactPosition).normalized;
        //        ParticleSystem t_ps = t_smoke.GetComponent<ParticleSystem>();

        //        if (t_ps != null)
        //        {
        //            var t_main = t_ps.main;
        //            t_main.startSpeed = 5f;

        //            // Give the correct angle to the Impact Particle System
        //            float angle = Mathf.Atan2(t_direction.x, t_direction.z) * Mathf.Rad2Deg;
        //            t_main.startRotation = angle * Mathf.Deg2Rad;
        //        }
        //    }
        //}
    }
}