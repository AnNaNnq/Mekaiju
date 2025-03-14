using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack.Instance
{
    public class RimVoidFire : MonoBehaviour
    {
        public int maxSizeY = 10;

        RimeVoid _stat;

        public void UpdateLineVisual(LineRenderer p_line, RimeVoid p_stat)
        {
            _stat = p_stat;
            int pointCount = p_line.positionCount;
            if (pointCount < 2) return; // Il faut au moins deux points pour tracer une ligne

            // R�cup�rer les extr�mit�s du LineRenderer
            Vector3 startPos = p_line.GetPosition(0);
            Vector3 endPos = p_line.GetPosition(pointCount - 1);

            // Calculer l'orientation
            Vector3 direction = (endPos - startPos).normalized;
            gameObject.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            StartCoroutine(GrowthWall());
            StartCoroutine(DestroyWall());
        }

        IEnumerator GrowthWall()
        {
            while(gameObject.transform.localScale.y <= maxSizeY)
            {
                yield return new WaitForSeconds(0.01f);
                Vector3 t_scale = gameObject.transform.localScale;
                gameObject.transform.localScale = new Vector3(t_scale.x, t_scale.y + 0.5f, t_scale.z);
            }
        }

        IEnumerator DestroyWall()
        {
            yield return new WaitForSeconds(_stat.rimVoidDuration);
            while (gameObject.transform.localScale.y >= 0)
            {
                yield return new WaitForSeconds(0.01f);
                Vector3 t_scale = gameObject.transform.localScale;
                gameObject.transform.localScale = new Vector3(t_scale.x, t_scale.y - 0.5f, t_scale.z);
            }
            Destroy(gameObject);
        }
    }
}
