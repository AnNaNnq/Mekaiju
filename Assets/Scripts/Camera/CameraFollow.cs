using System;
using UnityEngine;

namespace Mekaiju.CameraSystem
{
    public class CameraFollowTPS : MonoBehaviour
    {
        public Transform Mecha; // R�f�rence au mecha
        public Transform CameraPivot; // Pivot de la cam�ra (au niveau de la t�te ou du torse)

        // Distances dynamiques en fonction des �tats
        public float IdleDistance = 7f;
        public float CombatDistance = 5f;
        public float TransitionSpeed = 5f; // Vitesse de transition entre les positions

        private float _targetDistance; // Distance actuelle vis�e par la cam�ra

        // �tat actuel du mecha
        public enum MechaState { Idle, Combat }
        public MechaState CurrentState = MechaState.Idle;

        private void Start()
        {
            // Verrouille le curseur au centre de l'�cran
            Cursor.lockState = CursorLockMode.Locked;

            // Initialisation de la distance cible
            _targetDistance = IdleDistance;
        }

        private void LateUpdate()
        {
            // Ajuster la distance cible selon l'�tat actuel
            _AdjustDistanceByState();

            // Calculer la position souhait�e
            Vector3 t_desiredPosition = CameraPivot.position - CameraPivot.forward * _targetDistance;

            // Appliquer une transition fluide vers la nouvelle position
            transform.position = Vector3.Lerp(transform.position, t_desiredPosition, Time.deltaTime * TransitionSpeed);

            // Faire en sorte que la cam�ra regarde toujours le pivot
            transform.LookAt(CameraPivot);
        }

        private void _AdjustDistanceByState()
        {
            float t_targetDistance = CurrentState switch
            {
                MechaState.Idle => IdleDistance,
                MechaState.Combat => CombatDistance,
                _ => IdleDistance
            };

            // Transition douce entre la distance actuelle et la cible
            _targetDistance = Mathf.Lerp(_targetDistance, t_targetDistance, Time.deltaTime * TransitionSpeed);
        }

        // M�thode publique pour changer l'�tat du mecha
        public void ChangeMechaState(MechaState p_newState)
        {
            CurrentState = p_newState;
        }
    }
}