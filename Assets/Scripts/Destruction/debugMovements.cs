using UnityEngine;


namespace Mekaiju.DestructionStructuresDebug { 
    public class debugMovements : MonoBehaviour
    {
        public float speed = 5f; // Vitesse de déplacement

        void Update()
        {
            float move = 0f;

            if (Input.GetKey(KeyCode.A))
            {
                move = -speed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                move = speed * Time.deltaTime;
            }

            transform.Translate(0, 0, move);
        }
    }
}