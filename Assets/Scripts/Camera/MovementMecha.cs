using UnityEngine;


namespace Mekaiju.MechaControl
{
    public class MovementMecha : MonoBehaviour
    {
        public float MoveSpeed = 5f; // Vitesse de d�placement
        public float Gravity = -9.81f; // Gravit�
        public float JumpHeight = 2f; // Hauteur de saut

        private CharacterController _controller;
        private Vector3 _velocity;
        private bool _isGrounded;

        public Transform GroundCheck; // Un Empty GameObject positionn� sous le joueur
        public float GroundDistance = 0.4f; // Rayon pour d�tecter le sol
        public LayerMask GroundMask; // Mask pour identifier les layers du sol

        void Start()
        {
            _controller = GetComponent<CharacterController>();
        }

        void Update()
        {
            // V�rifier si le joueur est au sol
            _isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);

            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f; // Reste l�g�rement au sol pour �viter les probl�mes de collision
            }

            // R�cup�rer les inputs (horizontal et vertical)
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Calculer la direction de d�placement
            Vector3 move = transform.right * horizontal + transform.forward * vertical;

            // Appliquer le d�placement
            _controller.Move(move * MoveSpeed * Time.deltaTime);

            // Saut
            if (Input.GetButtonDown("Jump") && _isGrounded)
            {
                _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            }

            // Appliquer la gravit�
            _velocity.y += Gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }
    }
}