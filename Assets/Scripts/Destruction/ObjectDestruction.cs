using System.Collections;
using UnityEngine;

namespace Mekaiju.Destruction
{
    public class ObjectDestruction : MonoBehaviour
    {
        public DestructionType destructionType;
        public GameObject destructionParticuleEffect;
        Material _mat;

        private void Start()
        {
            _mat = GetComponent<Renderer>().material;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Kaiju"))
            {
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

                GameObject t_part = Instantiate(destructionParticuleEffect, transform.position, Quaternion.identity);
                Destroy(t_part, 2);
            }
        }

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

        public enum DestructionType
        {
            None,
            Walk,
            Dash
        }
    }
}

