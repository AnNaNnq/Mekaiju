using System.Collections;
using UnityEngine;

namespace Mekaiju.AI
{
    public interface ICharge
    {
        public void Jump(Rigidbody p_rb, float p_verticalBoost)
        {
            p_rb.AddForce(Vector3.up * p_verticalBoost, ForceMode.Impulse);
        }

        public IEnumerator Charge(KaijuInstance p_kaiju, float p_chargeSpeed, float p_chargeDuration, float p_verticalBoost, float p_chargePrepTime = 0)
        {
            Rigidbody t_rb = p_kaiju.GetComponent<Rigidbody>();
            Jump(t_rb, p_verticalBoost);

            yield return Charge(p_kaiju, p_chargeSpeed, p_chargeDuration, p_chargePrepTime);
        }

        public IEnumerator Charge(KaijuInstance p_kaiju, float p_chargeSpeed, float p_chargeDuration, float p_chargePrepTime)
        {
            float t_time = 0;
            Vector3 t_targetPosition = p_kaiju.GetTargetPos();

            while (t_time < p_chargePrepTime)
            {
                p_kaiju.motor.StopKaiju();
                p_kaiju.motor.LookTarget();
                t_targetPosition = p_kaiju.GetTargetPos();
                yield return new WaitForSeconds(0.01f);
                t_time += 0.01f;
            }

            yield return Charge(p_kaiju, p_chargeSpeed, p_chargeDuration);
        }

        public IEnumerator Charge(KaijuInstance p_kaiju, float p_chargeSpeed, float p_chargeDuration)
        {
            Rigidbody t_rb = p_kaiju.GetComponent<Rigidbody>();
            Vector3 t_targetPosition = p_kaiju.GetTargetPos();

            Vector3 t_startPos = p_kaiju.transform.position;
            Vector3 t_direction = (t_targetPosition - t_startPos).normalized;

            float t_elapsed = 0f;
            while (t_elapsed < p_chargeDuration)
            {
                t_rb.AddForce(t_direction * (p_chargeSpeed / p_chargeDuration), ForceMode.Acceleration);
                t_elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}
