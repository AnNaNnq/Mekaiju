using UnityEngine;

public class ImpactHandler : MonoBehaviour
{
    public GameObject decalPrefab; // Assign a Decal Projector Prefab
    public float decalLifetime = 10f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                SpawnDecal(contact.point, contact.normal);
            }
        }
    }

    void SpawnDecal(Vector3 position, Vector3 normal)
    {
        // Instantiate the Decal
        GameObject decal = Instantiate(decalPrefab, position, Quaternion.LookRotation(-normal));

        // Ensure it is above the surface to avoid overlap
        decal.transform.position += normal * 0.01f;

        // Destroy the decal after 10 seconds
        Destroy(decal, decalLifetime);
    }
}
