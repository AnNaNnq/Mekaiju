using System.Collections;
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
        DetectTargets();

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
    }

    //Détection des cibles proches
    void DetectTargets()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, lockOnRange, aimLayer);
        potentialTargets.Clear();

        foreach (Collider hit in hits)
        {
            potentialTargets.Add(hit.transform);
        }

        // Trier les cibles par distance
        potentialTargets.Sort((a, b) =>
            Vector3.Distance(transform.position, a.position)
            .CompareTo(Vector3.Distance(transform.position, b.position))
        );
    }

    //Active/Désactive le Lock-On
    void ToggleLockOn()
    {
        if (!isLockedOn && potentialTargets.Count > 0)
        {
            isLockedOn = true;
            targetIndex = 0;
            lockedTarget = potentialTargets[targetIndex];
            Debug.Log("Lock-On activé sur : " + lockedTarget.name);
            StartCoroutine(SmoothFollowTarget());
        }
        else
        {
            isLockedOn = false;
            lockedTarget = null;
            Debug.Log("Lock-On désactivé");
            StopAllCoroutines();
        }
    }

    //Change la cible verrouillée
    void ChangeTarget(int direction)
    {
        if (potentialTargets.Count == 0) return;

        targetIndex += direction;
        if (targetIndex < 0) targetIndex = potentialTargets.Count - 1;
        if (targetIndex >= potentialTargets.Count) targetIndex = 0;

        lockedTarget = potentialTargets[targetIndex];
        Debug.Log("Nouvelle cible verrouillée : " + lockedTarget.name);
    }

    //Coroutine pour suivre la cible verrouillée
    IEnumerator SmoothFollowTarget()
    {
        while (isLockedOn && lockedTarget != null)
        {
            Vector3 direction = lockedTarget.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lockOnRotationSpeed);
            yield return null;
        }
    }

    //Gizmos pour voir les cibles en mode éditeur
    void OnDrawGizmos()
    {
#if UNITY_EDITOR
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
#endif
    }
}
