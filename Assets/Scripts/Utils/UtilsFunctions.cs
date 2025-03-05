using System.Collections;
using UnityEngine;

namespace Mekaiju.Utils
{
    public class UtilsFunctions
    {
        public static float GetGround(Vector3 pos)
        {
            RaycastHit[] hits = Physics.RaycastAll(pos + Vector3.up * 10, Vector3.down, 1000, LayerMask.GetMask("Walkable"));

            if (hits.Length > 0)
            {
                float minY = float.MaxValue;
                foreach (var hit in hits)
                {
                    if (hit.point.y < minY)
                    {
                        minY = hit.point.y; // Prend le point le plus bas
                    }
                }
                return minY;
            }

            return pos.y; // Retourne la hauteur actuelle si aucun hit
        }

        /// <summary>
        /// Countdown function
        /// </summary>
        /// <returns></returns>
        public static IEnumerator CooldownRoutine(float cooldown, System.Action onCooldownEnd)
        {
            yield return new WaitForSeconds(cooldown);
            onCooldownEnd?.Invoke();
        }

        public static Vector3 GetRandomPointInCircle(float radius, Transform transform)
        {
            float angle = Random.Range(0f, Mathf.PI * 2); // Angle aléatoire en radians
            float distance = Mathf.Sqrt(Random.Range(0f, 1f)) * radius; // Rayon aléatoire (évite une distribution biaisée)

            float x = transform.position.x + Mathf.Cos(angle) * distance;
            float z = transform.position.z + Mathf.Sin(angle) * distance;
            float y = transform.position.y + 20; // Hauteur fixée

            return new Vector3(x, y, z);
        }
    }
}
