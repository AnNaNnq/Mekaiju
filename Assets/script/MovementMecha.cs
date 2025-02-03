using UnityEngine;

public class MovementMecha : MonoBehaviour
{
    public float moveSpeed = 5f; // Vitesse de d�placement
    public float gravity = -9.81f; // Gravit�
    public float jumpHeight = 2f; // Hauteur de saut

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    public Transform groundCheck; // Un Empty GameObject positionn� sous le joueur
    public float groundDistance = 0.4f; // Rayon pour d�tecter le sol
    public LayerMask groundMask; // Mask pour identifier les layers du sol

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // V�rifier si le joueur est au sol
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Reste l�g�rement au sol pour �viter les probl�mes de collision
        }

        // R�cup�rer les inputs (horizontal et vertical)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculer la direction de d�placement
        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        // Appliquer le d�placement
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Saut
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Appliquer la gravit�
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}