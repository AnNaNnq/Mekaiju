using UnityEngine;

namespace Mekaiju.CameraSystem
{
    public class CameraFollowTPS : MonoBehaviour
    {
        public Transform Mecha; // Référence au mecha
        public Transform CameraPivot; // Pivot de la caméra (au niveau de la tête ou du torse)
        public float MaxMouseSensitivity = 100f; // Sensibilité de la souris
        public float MaxVerticalClamp = 80f; // Limites pour l'angle vertical de la caméra

        // Distances dynamiques en fonction des états
        public float IdleDistance = 7f;
        public float CombatDistance = 5f;
        public float TransitionSpeed = 5f; // Vitesse de transition entre les positions

        private float _targetDistance; // Distance actuelle visée par la caméra
        private float _verticalRotation = 0f; // Stocke la rotation verticale de la caméra

        // État actuel du mecha
        public enum MechaState { Idle, Combat }
        public MechaState CurrentState = MechaState.Idle;

        private void Start()
        {
            // Verrouille le curseur au centre de l'écran
            Cursor.lockState = CursorLockMode.Locked;

            // Initialisation de la distance cible
            _targetDistance = IdleDistance;
        }

        private void LateUpdate()
        {
            // Récupérer les mouvements de la souris
            float p_mouseX = Input.GetAxis("Mouse X") * MaxMouseSensitivity * Time.deltaTime;
            float p_mouseY = Input.GetAxis("Mouse Y") * MaxMouseSensitivity * Time.deltaTime;

            // Tourner le joueur avec la caméra horizontalement
            Mecha.Rotate(Vector3.up * p_mouseX);

            // Gérer la rotation verticale de la caméra
            _verticalRotation -= p_mouseY;
            _verticalRotation = Mathf.Clamp(_verticalRotation, -MaxVerticalClamp, MaxVerticalClamp);
            CameraPivot.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);

            // Ajuster la distance cible selon l'état actuel
            _AdjustDistanceByState();

            // Calculer la position souhaitée
            Vector3 t_desiredPosition = CameraPivot.position - CameraPivot.forward * _targetDistance;

            // Appliquer une transition fluide vers la nouvelle position
            transform.position = Vector3.Lerp(transform.position, t_desiredPosition, Time.deltaTime * TransitionSpeed);

            // Faire en sorte que la caméra regarde toujours le pivot
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

        // Méthode publique pour changer l'état du mecha
        public void ChangeMechaState(MechaState p_newState)
        {
            CurrentState = p_newState;
        }
    }
}
