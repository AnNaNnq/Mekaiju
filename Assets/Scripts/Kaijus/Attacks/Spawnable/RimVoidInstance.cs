using Mekaiju.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mekaiju.AI.Attack.Instance
{
    public class RimVoidInstance : MonoBehaviour
    {
        KaijuInstance _instance;
        RimeVoid _stat;
        private LineRenderer _line;
        public int pointCount = 10;
        private List<Vector3> _firePos = new List<Vector3>();

        private IDisposable _speedMod;

        bool _damagable = false;

        public void SetUp(KaijuInstance p_instance, RimeVoid p_stat)
        {
            _instance = p_instance;
            _stat = p_stat;

            _line = GetComponent<LineRenderer>();
            BoxCollider t_lineCollider = _line.GetComponent<BoxCollider>();

            // Ajouter un BoxCollider s'il n'existe pas encore
            if (t_lineCollider == null)
            {
                t_lineCollider = _line.gameObject.AddComponent<BoxCollider>();
            }

            Vector3 t_startPos = _instance.transform.position;
            Vector3 t_endPos = _instance.GetTargetPos();

            // Calculer la direction entre l'AI et la cible
            Vector3 t_directionToTarget = (t_endPos - t_startPos).normalized;

            // Calculer la position finale en fonction de la port�e max
            Vector3 t_finalPos = t_endPos + t_directionToTarget * (_stat.range - Vector3.Distance(t_startPos, t_endPos));

            // Distance avant et apr�s la cible
            float distanceToTarget = Vector3.Distance(t_startPos, t_endPos);
            float distanceAfterTarget = Vector3.Distance(t_endPos, t_finalPos);

            // Calculer la r�partition des points
            int pointsBefore = Mathf.RoundToInt((distanceToTarget / (distanceToTarget + distanceAfterTarget)) * pointCount);
            int pointsAfter = pointCount - pointsBefore; // Assurer le total

            _line.positionCount = pointCount;

            _firePos.Clear();

            // Tracer les points avant la cible
            for (int i = 0; i < pointsBefore; i++)
            {
                float t = (float)i / (pointsBefore - 1); // Interpolation entre l'AI et la cible
                Vector3 t_interpolatedPos = Vector3.Lerp(t_startPos, t_endPos, t);
                t_interpolatedPos.y = UtilsFunctions.GetGround(t_interpolatedPos); // Ajuster la hauteur au sol
                _line.SetPosition(i, t_interpolatedPos);
                _firePos.Add(t_interpolatedPos);
            }

            // Tracer les points apr�s la cible
            for (int i = 0; i < pointsAfter; i++)
            {
                float t = (float)i / (pointsAfter - 1); // Interpolation apr�s la cible
                Vector3 t_extendedPos = Vector3.Lerp(t_endPos, t_finalPos, t);
                t_extendedPos.y = UtilsFunctions.GetGround(t_extendedPos); // Ajuster la hauteur au sol
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
            GameObject t_fire = Instantiate(_stat.gameObjectRimVoidFire, p_point, Quaternion.identity);
            RimVoidFire t_rvf = t_fire.GetComponent<RimVoidFire>();
            t_rvf.UpdateLineVisual(_line, _stat);
        }

        IEnumerator lifeTime()
        {
            BoxCollider lineCollider = _line.GetComponent<BoxCollider>();

            // Initialiser le collider avec une taille minimale
            if (lineCollider == null)
            {
                lineCollider = _line.gameObject.AddComponent<BoxCollider>();
            }
            lineCollider.size = Vector3.zero; // D�but � z�ro

            Vector3 firstPoint = _firePos[0]; // Premier point du mur

            foreach (Vector3 t_point in _firePos)
            {
                SpawnFire(t_point);

                // Mettre � jour le collider avec la partie visible
                UpdateColliderProgressively(lineCollider, firstPoint, t_point);

                yield return new WaitForSeconds(0.01f);
            }

            yield return new WaitForSeconds(_stat.rimVoidDuration);
            lineCollider.enabled = false;

            _instance.motor.StartKaiju();

            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetMechaPartInstance(out var t_inst))
            {
                _speedMod = t_inst.mecha.AddEffect(_stat.rimVoidEffect);
                _damagable = true;
                StartCoroutine(Damage(t_inst));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetMechaPartInstance(out var t_inst))
            {
                t_inst.mecha.RemoveEffect(_speedMod);
                _damagable = false;
            }
        }

        IEnumerator Damage(MechaPartInstance p_mechaPart)
        {
            while (_damagable)
            {
                float t_damage = _instance.GetRealDamage(_stat.damage);
                p_mechaPart.TakeDamage(_instance, t_damage, Entity.DamageKind.Direct);
                _instance.AddDPS(t_damage);
                _instance.UpdateUI();
                yield return new WaitForSeconds(_stat.rimVoidHitCooldown);
            }
        }
    }
}
