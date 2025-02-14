using UnityEngine;

namespace Mekaiju.Utils
{
    public class UtilsFunctions
    {
        public static float GetGround(Vector3 pos)
        {
            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 1000, LayerMask.GetMask("Walkable")))
            {
                return hit.point.y;
            }
            if (Physics.Raycast(pos, Vector3.up, out hit, 1000, LayerMask.GetMask("Walkable")))
            {
                return hit.point.y;
            }

            return 0;
        }
    }
}
