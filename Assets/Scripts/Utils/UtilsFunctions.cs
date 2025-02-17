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
                        minY = hit.point.y; // Prend le point le plus bas
                }
                return minY;
            }

            return pos.y; // Retourne la hauteur actuelle si aucun hit
        }
    }
}
