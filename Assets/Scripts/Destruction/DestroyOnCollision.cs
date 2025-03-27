using UnityEngine;

public class SpawnOnDestroyWithEffects : MonoBehaviour
{
    [Header("Objets")]
    public GameObject objectToSpawn; // L'objet qui apparaîtra après destruction
    public GameObject particleEffect; // L'effet de particules à jouer après la destruction

    [Header("Effets")]
    public AudioClip soundEffect; // Son à jouer lors de la destruction
    public float particleDuration = 2f; // Durée avant de détruire l'effet de particules

    // Méthode appelée lors de la collision avec un autre objet (si "Is Trigger" est activé)
    private void OnTriggerEnter(Collider other)
    {
        // Vérifie que l'objet en collision est le joueur
        if (other.CompareTag("Player"))
        {
            // 1️⃣ Apparition d'un nouvel objet à la place de l'objet détruit
            if (objectToSpawn != null)
            {
                // Crée une copie de l'objet à la position de l'objet détruit
                Instantiate(objectToSpawn, transform.position, transform.rotation);
            }

            // 2️⃣ Jouer un son lors de la destruction
            if (soundEffect != null)
            {
                AudioSource.PlayClipAtPoint(soundEffect, transform.position);
            }

            // 3️⃣ Créer un effet de particules à la position de l'objet
            if (particleEffect != null)
            {
                GameObject fx = Instantiate(particleEffect, transform.position, Quaternion.identity);
                // Détruire l'effet de particules après la durée spécifiée
                Destroy(fx, particleDuration);
            }

            // 4️⃣ Détruire l'objet actuel après avoir exécuté toutes les actions
            Destroy(gameObject);
        }
    }
}