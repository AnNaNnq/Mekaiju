using Mekaiju;
using Mekaiju.AI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RimVoid : MonoBehaviour
{
    private TeneborokAI _ai;
    private LineRenderer _line;
    public int pointCount = 10;
    private List<Vector3> _firePos = new List<Vector3>();

    bool _damagable = false;

    public void SetUp(TeneborokAI p_teneborokAI)
    {
        _ai = p_teneborokAI;
        _line = GetComponent<LineRenderer>();
        BoxCollider t_lineCollider = _line.GetComponent<BoxCollider>();

        // Ajouter un BoxCollider s'il n'existe pas encore
        if (t_lineCollider == null)
        {
            t_lineCollider = _line.gameObject.AddComponent<BoxCollider>();
        }

        Vector3 t_startPos = _ai.transform.position;
        Vector3 t_endPos = _ai.GetTargetPos();

        // Calculer la direction entre l'AI et la cible
        Vector3 t_directionToTarget = (t_endPos - t_startPos).normalized;

        // Calculer la position finale en fonction de la portée max
        Vector3 t_finalPos = t_endPos + t_directionToTarget * (_ai.rimRange - Vector3.Distance(t_startPos, t_endPos));

        // Distance avant et après la cible
        float distanceToTarget = Vector3.Distance(t_startPos, t_endPos);
        float distanceAfterTarget = Vector3.Distance(t_endPos, t_finalPos);

        // Calculer la répartition des points
        int pointsBefore = Mathf.RoundToInt((distanceToTarget / (distanceToTarget + distanceAfterTarget)) * pointCount);
        int pointsAfter = pointCount - pointsBefore; // Assurer le total

        _line.positionCount = pointCount;

        _firePos.Clear();

        // Tracer les points avant la cible
        for (int i = 0; i < pointsBefore; i++)
        {
            float t = (float)i / (pointsBefore - 1); // Interpolation entre l'AI et la cible
            Vector3 t_interpolatedPos = Vector3.Lerp(t_startPos, t_endPos, t);
            t_interpolatedPos.y = GetGround(t_interpolatedPos); // Ajuster la hauteur au sol
            _line.SetPosition(i, t_interpolatedPos);
            _firePos.Add(t_interpolatedPos);
        }

        // Tracer les points après la cible
        for (int i = 0; i < pointsAfter; i++)
        {
            float t = (float)i / (pointsAfter - 1); // Interpolation après la cible
            Vector3 t_extendedPos = Vector3.Lerp(t_endPos, t_finalPos, t);
            t_extendedPos.y = GetGround(t_extendedPos); // Ajuster la hauteur au sol
            _line.SetPosition(pointsBefore + i, t_extendedPos);
            _firePos.Add(t_extendedPos);
        }

        StartCoroutine(lifeTime());
    }

    void UpdateColliderProgressively(BoxCollider collider, Vector3 start, Vector3 currentEnd)
    {
        Vector3 midPoint = (start + currentEnd) / 2f; // Position centrale du collider
        float length = Vector3.Distance(start, currentEnd); // Longueur actuelle de la ligne
        float thickness = 0.2f; // Ajuste selon besoin

        collider.center = collider.transform.InverseTransformPoint(midPoint);
        collider.size = new Vector3(thickness, 20, length);
        collider.center = new Vector3(0, 0, collider.center.z);

        // Orienter le collider pour suivre la direction actuelle de la ligne
        collider.transform.rotation = Quaternion.LookRotation(currentEnd - start);
    }



    public void SpawnFire(Vector3 p_point)
    {
        GameObject t_fire = Instantiate(_ai.gameObjectRimFire, p_point, Quaternion.identity);
        RimVoidFire t_rvf = t_fire.GetComponent<RimVoidFire>();
        t_rvf.UpdateLineVisual(_line, _ai);
    }

    IEnumerator lifeTime()
    {
        BoxCollider lineCollider = _line.GetComponent<BoxCollider>();

        // Initialiser le collider avec une taille minimale
        if (lineCollider == null)
        {
            lineCollider = _line.gameObject.AddComponent<BoxCollider>();
        }
        lineCollider.size = Vector3.zero; // Début à zéro

        Vector3 firstPoint = _firePos[0]; // Premier point du mur

        foreach (Vector3 t_point in _firePos)
        {
            SpawnFire(t_point);

            // Mettre à jour le collider avec la partie visible
            UpdateColliderProgressively(lineCollider, firstPoint, t_point);

            yield return new WaitForSeconds(0.01f);
        }

        _ai.AttackCooldown();
        _ai.SetLastAttack(TeneborokAttack.RimVoid);

        yield return new WaitForSeconds(_ai.rimDuration);
        lineCollider.enabled = false;

        yield return new WaitForSeconds(1f);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MechaInstance t_mecha = other.GetComponent<MechaInstance>();
            //Temporaire à changer quant le systeme d'effet sera mis à jour
            t_mecha.Context.SpeedModifier /= 2;
            _damagable = true;
            StartCoroutine(Damage(t_mecha));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MechaInstance t_mecha = other.GetComponent<MechaInstance>();
            //Temporaire à changer quant le systeme d'effet sera mis à jour
            t_mecha.Context.SpeedModifier *= 2;
            _damagable = false;
        }
    }

    IEnumerator Damage(MechaInstance p_mecha)
    {
        while (_damagable)
        {
            p_mecha.TakeDamage(_ai.rimDamage);
            _ai.AddDps(_ai.rimDamage);
            yield return new WaitForSeconds(_ai.rimHitCooldown);
        }
    }
}
