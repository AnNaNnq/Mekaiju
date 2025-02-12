using Mekaiju.AI;
using System.Collections;
using UnityEngine;

public class RimVoid : MonoBehaviour
{
    private TeneborokAI _ai;
    private LineRenderer _line;
    public int pointCount = 10;

    public void SetUp(TeneborokAI p_teneborokAI)
    {
        _ai = p_teneborokAI;
        _line = GetComponent<LineRenderer>();

        Vector3 t_startPos = _ai.transform.position;
        Vector3 t_endPos = _ai.GetTargetPos();

        // Calculer la direction entre l'AI et la cible
        Vector3 directionToTarget = (t_endPos - t_startPos).normalized;

        // Calculer la position finale de la ligne en fonction de la portée max
        Vector3 finalPos = t_endPos + directionToTarget * (_ai.rimRange - Vector3.Distance(t_startPos, t_endPos));

        // Calculer le nombre total de points pour la ligne
        int totalPoints = pointCount + 5; // Ajouter quelques points supplémentaires pour la partie derrière

        _line.positionCount = totalPoints;

        // Tracer les points depuis l'AI jusqu'à la cible, puis vers la portée max
        for (int i = 0; i < pointCount; i++)
        {
            float t = (float)i / (pointCount - 1); // Interpolation entre l'AI et la cible
            Vector3 interpolatedPos = Vector3.Lerp(t_startPos, t_endPos, t);
            interpolatedPos.y = GetGround(interpolatedPos); // Ajuster la hauteur au sol
            _line.SetPosition(i, interpolatedPos);
        }

        // Tracer les points au-delà de la cible jusqu'à la portée max
        for (int i = pointCount; i < totalPoints; i++)
        {
            float t = (float)(i - pointCount) / (totalPoints - pointCount - 1); // Interpolation après la cible
            Vector3 extendedPos = Vector3.Lerp(t_endPos, finalPos, t);
            extendedPos.y = GetGround(extendedPos); // Ajuster la hauteur au sol
            _line.SetPosition(i, extendedPos);
        }

        // Instancier le feu visuel
        GameObject _fire = Instantiate(_ai.gameObjectRimFire, transform.position, Quaternion.identity);
        RimVoidFire t_rvf = _fire.GetComponent<RimVoidFire>();
        t_rvf.UpdateLineVisual(_line, _ai);
        StartCoroutine(lifeTime());

    }

    IEnumerator lifeTime()
    {
        yield return new WaitForSeconds(_ai.rimDuration);
        _ai.SetLastAttack(TeneborokAttack.RimVoid);
        Destroy(gameObject);
    }

    public float GetGround(Vector3 pos)
    {
        if(Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 1000, LayerMask.GetMask("Walkable")))
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
