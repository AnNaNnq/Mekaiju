using UnityEngine;

namespace Mekaiju.DestructionStructures {
    public class EntityNDestructibleObjects : MonoBehaviour
    {
        // Ce script doit �tre attach� aux hitbox du mecha concern�es par la destruction
        // (Donc les jambes/pieds/bras/poings du mecha et ses attaques)
    
        public GameObject smokePrefab; // Prefab de particules pour la destruction
        public GameObject impactPrefab; // Prefab de l'impact sur les murs

        private void OnTriggerEnter(Collider p_other)
        {
        
            // Gestion de la destruction des objets destructibles
            if (p_other.tag == "destructible")
            {
                float maHauteur = GetComponent<Collider>().bounds.size.y;
                float autreHauteur = p_other.bounds.size.y;

                if ((this.tag == "mechaDash") && ((maHauteur / 5) >= autreHauteur))
                {
                    _DestructStructures(p_other.gameObject);
                }
                else if (maHauteur / 10 >= autreHauteur)
                {
                    _DestructStructures(p_other.gameObject);
                }
            }
            else if (this.tag == "mechaAttack")
            {

                _CreateImpact(p_other);
            }
        }

        private void _DestructStructures(GameObject p_target)
        {
            if (smokePrefab != null)
            {
                // �vite les erreurs si aucun smokePrefab n'est s�l�ctionn�
                // G�re l'apparition de l'effet de particules
                Instantiate(smokePrefab, p_target.transform.position, Quaternion.identity);
                print("effet de particules");
            }

            if (p_target.TryGetComponent<Destructible>(out Destructible myScript))
            {
                // v�rifie si le collider poss�de le script Destructible et la m�thode getDestruct
                if (myScript.GetType().GetMethod("getDestruct") != null)
                {
                    Debug.Log("M�thode trouv�e !");
                    myScript.getDestruct();
                }
            }
            else
            {
                // si l'objet a le tag destructible mais pas de script attach� alors il sera simplement d�truit
                Destroy(p_target);
            }
        }

        private void _CreateImpact(Collider p_wall)
        {
            // Fonctionne Bof donc � peaufiner
            Vector3 impactPosition = p_wall.ClosestPoint(transform.position);

            // Instancier l'impact visuel sur la surface du mur
            if (impactPrefab != null)
            {
                GameObject impact = Instantiate(impactPrefab, impactPosition, Quaternion.identity);
                impact.transform.up = p_wall.transform.forward; // Aligner l'impact sur le mur
            }
        
            // Instancier la fum�e et la diriger vers l'objet attaquant
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
}