using Mekaiju.AI;
using Mekaiju.AI.Body;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mekaiju.LockOnTargetSystem
{
    public class LockOnTargetSystem : MonoBehaviour
    {

        [Header("Références")]
        [SerializeField] private LayerMask _targetLayerMask; // Masque de couche pour les cibles valides

        [Header("Paramètres de Lock-On")]
        public float lockOnRange = 100f; // Portée du Lock-On
        public float lockOnRotationSpeed = 10f; // Vitesse de rotation vers la cible
        public float lockOnDistance = 3f; // Distance de rotation souhaitée
        public float verticalAngle = 0f; // Angle vertical fixe

        [Header("Paramètres de Debug")]
        [SerializeField] private Transform _lockedTarget = null; // Cible verrouillée

        private List<Transform> _potentialTargets = new List<Transform>(); // Liste des cibles
        private int _targetIndex = 0; // Index pour changer de cible
        private bool _isLockedOn = false; // État du Lock-On

        private Transform _cameraPivot;
        private void Start()
        {
            _cameraPivot = transform.Find("CameraPivot");
        }

        private void Update()
        {
            _DetectTargets();
            //Debug.Log(GetTargetBodyPartObject());
        }

        // Détecte les cibles proches dans la portée du Lock-On
        private void _DetectTargets()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, lockOnRange, _targetLayerMask);
            _potentialTargets.Clear();

            foreach (Collider hit in hits)
            {
                _potentialTargets.Add(hit.transform);
            }

            // Trier les cibles par distance
            _potentialTargets.Sort((a, b) =>
                Vector3.Distance(transform.position, a.position)
                .CompareTo(Vector3.Distance(transform.position, b.position)));
        }

        // Active ou désactive le Lock-On
        public void ToggleLockOn(bool p_isLockedOn)
        {
            if (p_isLockedOn && _potentialTargets.Count > 0)
            {
                _isLockedOn = p_isLockedOn;
                _targetIndex = 0;
                _lockedTarget = _potentialTargets[_targetIndex];
                Debug.Log("Lock-On activé sur : " + _lockedTarget.name);
                StartCoroutine(_SmoothFollowTarget());
            }
            else
            {
                _isLockedOn = p_isLockedOn;
                _lockedTarget = null;
                Debug.Log("Lock-On désactivé");
                StopAllCoroutines();
            }
        }

        // Change la cible verrouillée actuelle
        public void ChangeTarget(int direction)
        {
            if (_potentialTargets.Count == 0) return;

            _targetIndex += direction;
            if (_targetIndex < 0) _targetIndex = _potentialTargets.Count - 1;
            if (_targetIndex >= _potentialTargets.Count) _targetIndex = 0;

            _lockedTarget = _potentialTargets[_targetIndex];
            Debug.Log("Nouvelle cible verrouillée : " + _lockedTarget.name);
        }

        // Suit la cible verrouillée de manière fluide
        private IEnumerator _SmoothFollowTarget()
        {
            while (_isLockedOn && _lockedTarget != null)
            {
                // Calculer la position désirée
                Vector3 directionToTarget = (_lockedTarget.position - _cameraPivot.position).normalized;

                // Calculer et appliquer la rotation
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

                Quaternion yRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, yRotation, Time.deltaTime * lockOnRotationSpeed);

                _cameraPivot.localRotation = Quaternion.Euler(targetRotation.eulerAngles.x, 0, 0);

                Quaternion xzRotation = Quaternion.Euler(targetRotation.eulerAngles.x, 0, targetRotation.eulerAngles.z);
                _cameraPivot.localRotation = Quaternion.Slerp(_cameraPivot.localRotation, xzRotation, Time.deltaTime * lockOnRotationSpeed);

                yield return null;
            }
        }

        // Dessine des gizmos pour visualiser les cibles dans l'éditeur
        private void OnDrawGizmos()
        {
            if (_lockedTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(_lockedTarget.position, new Vector3(1f, 1f, 1f));
            }

            foreach (Transform target in _potentialTargets)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(target.position, new Vector3(2f, 2f, 2f));
            }

        }

        public BodyPartObject GetTargetBodyPartObject()
        {
            return _lockedTarget.gameObject.GetComponent<BodyPartObject>();
        }
    }
}