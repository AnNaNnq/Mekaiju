using System.Collections.Generic;
using UnityEngine;

public class AimSystem : MonoBehaviour
{
    [Header("Références")]
    public Camera playerCamera; // Caméra du joueur
    public float aimRange = 100f; // Portée du rayon
    public LayerMask aimLayer; // Masque de détection des cibles

    [Header("Lock-On")]
    public float lockOnRange = 15f; // Portée du lock-on
    public float lockOnRotationSpeed = 5f; // Vitesse de rotation vers la cible

    [Header("Debug (Inspector)")]
    [SerializeField] private Transform target; // Cible actuelle
    [SerializeField] private Transform lockedTarget; // Cible verrouillée
    private List<Transform> potentialTargets = new List<Transform>(); // Liste des cibles
    private int targetIndex = 0; // Index pour changer de cible
    private bool isLockedOn = false; // Si le Lock-On est actif

    void Update()
    {
        AimAtTarget();

        // Verrouiller/Déverrouiller le Lock-On (Touche L)
        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleLockOn();
        }

        // Changer la cible verrouillée (Touche J / K)
        if (isLockedOn && potentialTargets.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.J)) ChangeTarget(-1);
            if (Input.GetKeyDown(KeyCode.K)) ChangeTarget(1);
        }

        // Suivre la cible verrouillée
        if (isLockedOn && lockedTarget != null)
        {
            FollowTarget();
        }
    }

    // 🔵 Détection de la cible sous le curseur
    void AimAtTarget()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, aimRange, aimLayer))
        {
            target = hit.transform; // Assigne la cible visée
            if (!potentialTargets.Contains(target))
            {
                potentialTargets.Add(target);
            }
        }
        else
        {
            target = null;
        }
    }

    // 🔴 Active/Désactive le Lock-On
    void ToggleLockOn()
    {
        if (!isLockedOn && potentialTargets.Count > 0)
        {
            isLockedOn = true;
            targetIndex = 0;
            lockedTarget = potentialTargets[targetIndex];
            Debug.Log("Lock-On activé sur : " + lockedTarget.name);
        }
        else
        {
            isLockedOn = false;
            lockedTarget = null;
            Debug.Log("Lock-On désactivé");
        }
    }

    // 🔄 Change la cible verrouillée
    void ChangeTarget(int direction)
    {
        if (potentialTargets.Count == 0) return;

        targetIndex += direction;
        if (targetIndex < 0) targetIndex = potentialTargets.Count - 1;
        if (targetIndex >= potentialTargets.Count) targetIndex = 0;

        lockedTarget = potentialTargets[targetIndex];
        Debug.Log("Nouvelle cible verrouillée : " + lockedTarget.name);
    }

    // 🎯 Suivre la cible verrouillée
    void FollowTarget()
    {
        if (lockedTarget != null)
        {
            Vector3 direction = lockedTarget.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lockOnRotationSpeed);
        }
    }

    // 🎨 Gizmos pour voir les cibles en mode éditeur
    void OnDrawGizmos()
    {
        if (lockedTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(lockedTarget.position, 1f);
        }

        foreach (Transform t in potentialTargets)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(t.position, 1f);
        }
    }
}
