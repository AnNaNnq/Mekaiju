using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 6f; // Speed of the player
    Vector3 movement; // Vector to store the direction of the player
    Animator anim; // Reference to the animator component
    Rigidbody playerRigidbody; // Reference to the player's rigidbody


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the reference to the animator component
        anim = GetComponent<Animator>();
        // Get the reference to the player's rigidbody
        playerRigidbody = GetComponent<Rigidbody>();


    }

    // Update is called once per frame
    void Update()
    {
        // Get the input from the player
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Call the function to move the player
        Move(h, v);
        // Call the function to turn the player
        Turning();
        // Call the function to animate the player
        Animating(h, v);
    }

    void Move(float h, float v) {
        // Set the movement vector based on the axis input
        movement.Set(h, 0f, v);
        // Normalize the movement vector and make it proportional to the speed per second
        movement = movement.normalized * speed * Time.deltaTime;
        // Move the player to it's current position plus the movement
        playerRigidbody.MovePosition(transform.position + movement);
    }

    void Turning() {
        // Create a ray from the mouse cursor on screen in the direction of the camera
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Create a RaycastHit variable to store information about what was hit by the ray
        RaycastHit floorHit;
        // Perform the raycast and if it hits something on the floor layer
        if (Physics.Raycast(camRay, out floorHit, 100f))
        {
            // Create a vector from the player to the point on the floor the raycast from the mouse hit
            Vector3 playerToMouse = floorHit.point - transform.position;
            // Ensure the vector is entirely along the floor plane
            playerToMouse.y = 0f;
            // Create a quaternion (rotation) based on looking down the vector from the player to the mouse
            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
            // Set the player's rotation to this new rotation
            playerRigidbody.MoveRotation(newRotation);
        }
    }

    void Animating(float h, float v)
    {
        // Create a boolean that is true if either of the input axes is non-zero
        bool walking = h != 0f || v != 0f;
        // Tell the animator whether or not the player is walking
        anim.SetBool("IsWalking", walking);
    }


}
