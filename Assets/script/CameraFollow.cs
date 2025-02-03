using UnityEngine;

public class CameraFollowTPS : MonoBehaviour
{
    public Transform mecha; // R�f�rence au mecha
    public Transform cameraPivot; // Pivot de la cam�ra (au niveau de la t�te ou du torse)
    public float mouseSensitivity = 100f; // Sensibilit� de la souris
    public float verticalClamp = 80f; // Limites pour l'angle vertical de la cam�ra

    // Distances dynamiques en fonction des �tats
    public float idleDistance = 7f;
    public float combatDistance = 5f;

    public float transitionSpeed = 5f; // Vitesse de transition entre les positions
    private float targetDistance; // Distance actuelle vis�e par la cam�ra

    private float verticalRotation = 0f; // Stocke la rotation verticale de la cam�ra

    // �tat actuel du mecha
    public enum MechaState { Idle, Combat }
    public MechaState currentState = MechaState.Idle;

    void Start()
    {
        // Verrouille le curseur au centre de l'�cran
        Cursor.lockState = CursorLockMode.Locked;

        // Initialisation de la distance cible
        targetDistance = idleDistance;
    }

    void LateUpdate()
    {
        // R�cup�rer les mouvements de la souris
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Tourner le joueur avec la cam�ra horizontalement
        mecha.Rotate(Vector3.up * mouseX);

        // G�rer la rotation verticale de la cam�ra
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalClamp, verticalClamp);
        cameraPivot.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // Ajuster la distance cible selon l'�tat actuel
        AdjustDistanceByState();

        // Calculer la position souhait�e
        Vector3 desiredPosition = cameraPivot.position - cameraPivot.forward * targetDistance;

        // Appliquer une transition fluide vers la nouvelle position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * transitionSpeed);

        // Faire en sorte que la cam�ra regarde toujours le pivot
        transform.LookAt(cameraPivot);
    }

    void AdjustDistanceByState()
    {
        float targetDistance = currentState switch
        {
            MechaState.Idle => idleDistance,
            MechaState.Combat => combatDistance,
            _ => idleDistance
        };

        // Transition douce entre la distance actuelle et la cible
        targetDistance = Mathf.Lerp(targetDistance, targetDistance, Time.deltaTime * transitionSpeed);
    }

    // M�thode publique pour changer l'�tat du mecha
    public void ChangeMechaState(MechaState newState)
    {
        currentState = newState;
    }
}
