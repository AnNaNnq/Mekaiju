using UnityEngine;

namespace Mekaiju.CameraSystem
{
    public class CameraFollowTPS : MonoBehaviour
    {
        public Transform Mecha; // R�f�rence au mecha
        public Transform CameraPivot; // Pivot de la cam�ra (au niveau de la t�te ou du torse)
        public float MaxMouseSensitivity = 100f; // Sensibilit� de la souris
        public float MaxVerticalClamp = 80f; // Limites pour l'angle vertical de la cam�ra

        // Distances dynamiques en fonction des �tats
        public float IdleDistance = 7f;
        public float CombatDistance = 5f;
        public float TransitionSpeed = 5f; // Vitesse de transition entre les positions

        private float _targetDistance; // Distance actuelle vis�e par la cam�ra
        private float _verticalRotation = 0f; // Stocke la rotation verticale de la cam�ra

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
            // R�cup�rer les mouvements de la souris
            float p_mouseX = Input.GetAxis("Mouse X") * MaxMouseSensitivity * Time.deltaTime;
            float p_mouseY = Input.GetAxis("Mouse Y") * MaxMouseSensitivity * Time.deltaTime;

            // Tourner le joueur avec la cam�ra horizontalement
            Mecha.Rotate(Vector3.up * p_mouseX);

            // G�rer la rotation verticale de la cam�ra
            _verticalRotation -= p_mouseY;
            _verticalRotation = Mathf.Clamp(_verticalRotation, -MaxVerticalClamp, MaxVerticalClamp);
            CameraPivot.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);

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
