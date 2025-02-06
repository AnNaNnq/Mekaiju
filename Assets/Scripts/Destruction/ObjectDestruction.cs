using System.Collections;
using UnityEngine;

namespace Mekaiju.Destruction
{
    public class ObjectDestruction : MonoBehaviour
    {
        public DestructionType destructionType;
        public GameObject destructionParticuleEffect;
        Material _mat;


        /// <summary>
        /// Start function
        /// </summary>
        private void Start()
        {
            _mat = GetComponent<Renderer>().material;
        }

        /// <summary>
        /// Function executed on collision with another object
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Kaiju"))
            {
                // Get the highest force
                float t_force = Mathf.Max(Mathf.Abs(collision.impulse.x), Mathf.Abs(collision.impulse.y), Mathf.Abs(collision.impulse.z));
                switch (destructionType)
                {
                    case DestructionType.None:
                        break;
                    case DestructionType.Walk: // Walk
                        if (t_force >= 2)
                        {
                            StartCoroutine(FadOutAnim());
                        }
                        break;
                    case DestructionType.Dash: // Dash
                        if (t_force >= 10)
                        {
                            StartCoroutine(FadOutAnim());
                        }
                        break;
                    default: break;
                }
                // Instantiate destruction particule effect
                GameObject t_part = Instantiate(destructionParticuleEffect, transform.position, Quaternion.identity);
                // Destroy the particule effect after 2 seconds
                Destroy(t_part, 2);
            }
        }

        /// <summary>
        /// FadOut object animation
        /// </summary>
        /// <returns></returns>
        IEnumerator FadOutAnim()
        {
            while (_mat.color.a > 0)
            {
                float alpha = _mat.color.a;
                _mat.color = new Color(_mat.color.r, _mat.color.g, _mat.color.b, alpha - 0.01f);
                yield return new WaitForSeconds(0.01f);
            }
            GetComponent<Renderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
        }

        /// <summary>
        /// Enum for the type of destruction
        /// </summary>
        public enum DestructionType
        {
            None,
            Walk,
            Dash
        }
    }
}

