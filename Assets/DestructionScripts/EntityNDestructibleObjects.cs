using UnityEngine;

public class EntityNDestructibleObjects : MonoBehaviour
{

    public GameObject smokePrefab; // Prefab de particules pour la destruction
    public GameObject impactPrefab; // Prefab de l'impact sur les murs

    private void OnTriggerEnter(Collider other)
    {
        
        // Gestion de la destruction des objets destructibles
        if (other.tag == "destructible")
        {
            float maHauteur = GetComponent<Collider>().bounds.size.y;
            float autreHauteur = other.bounds.size.y;

            if ((this.tag == "mechaDash") && ((maHauteur / 5) >= autreHauteur))
            {
                Destruct(other.gameObject);
            }
            else if (maHauteur / 10 >= autreHauteur)
            {
                Destruct(other.gameObject);
            }
        }
        else if (this.tag == "mechaAttack")
        {
            
            CreateImpact(other);
        }
    }

    public void Destruct(GameObject target)
    {
        if (smokePrefab != null)
        {
            Instantiate(smokePrefab, target.transform.position, Quaternion.identity);
            print("effet de particules");
        }
        Destroy(target);
    }

    private void CreateImpact(Collider wall)
    {
        Vector3 impactPosition = wall.ClosestPoint(transform.position);

        // Instancier l'impact visuel sur la surface du mur
        if (impactPrefab != null)
        {
            GameObject impact = Instantiate(impactPrefab, impactPosition, Quaternion.identity);
            impact.transform.up = wall.transform.forward; // Aligner l'impact sur le mur
        }
        
        // Instancier la fumée et la diriger vers l'objet attaquant
        if (smokePrefab != null)
        {
            
            GameObject smoke = Instantiate(smokePrefab, impactPosition, Quaternion.identity);
            print("effet de particules");
            Vector3 direction = (transform.position - impactPosition).normalized;
            ParticleSystem ps = smoke.GetComponent<ParticleSystem>();

            if (ps != null)
            {
                var main = ps.main;
                main.startSpeed = 5f;

                // Convertir la direction en angle et l'appliquer en radians
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                main.startRotation = angle * Mathf.Deg2Rad;
            }
        }
    }
}