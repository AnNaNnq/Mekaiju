using System.Collections;
using Mekaiju.AI;
using UnityEngine;

namespace Mekaiju
{

    /// <summary>
    /// 
    /// </summary>
    public class RailGunAbility : IAbilityBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _damage;

        /// <summary>
        /// Max bullet throw per minute
        /// </summary>
        [SerializeField]
        private int _rateOfFire;
        
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _consumption;

        /// <summary>
        /// Bullet speed in m/s
        /// </summary>
        [SerializeField]
        private float _travelTime;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _projectile;

        /// <summary>
        /// 
        /// </summary>
        private float _lastTriggerTime;

        /// <summary>
        /// 
        /// </summary>
        private float _minTimeBetweenFire => 1f / (_rateOfFire / 60f);

        /// <summary>
        /// 
        /// </summary>
        public override void Initialize()
        {
            _lastTriggerTime = 0f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_self"></param>
        /// <param name="p_target"></param>
        /// <returns></returns>
        public override IEnumerator Execute(MechaInstance p_self, BasicAI p_target)
        {
            var t_now     = Time.time; 
            var t_elapsed = t_now - _lastTriggerTime;
            if (t_elapsed >= _minTimeBetweenFire)
            {
                if (p_self.CanExecuteAbility(_consumption))
                {   
                    _lastTriggerTime = t_now;
                    // TODO: Launch animation

                    p_self.ConsumeStamina(_consumption);

                    var t_targetPosition = p_target.transform.position;

                    // Compute travel time
                    var t_dist = Vector3.Distance(p_self.transform.position, t_targetPosition);
                    var t_time = t_dist / _travelTime;

                    bool t_hasCollide = false;

                    var t_go = GameObject.Instantiate(_projectile);
                    var t_wb = t_go.GetComponent<WeaponBullet>();

                    t_wb.transform.position = p_self.transform.position + new Vector3(0, 2f, 2f);
                    t_wb.OnCollide.AddListener(
                        collision => {
                            Debug.Log(collision.gameObject.name);
                            if (collision.gameObject.name == "Kaiju")
                            {
                                t_hasCollide = true;
                                GameObject.Destroy(t_wb);
                            }
                        }
                    );
                    t_wb.Launch(p_self.transform.forward.normalized * _travelTime);
                    

                    // Wait for bullet travel
                    float t_timout = t_time;
                    yield return new WaitUntil(() => t_hasCollide || (t_timout -= Time.deltaTime) <= 0);

                    // Check new position of BasicAI ?
                    if (t_hasCollide)
                    {
                        // Apply damage
                        Debug.Log("RailGun projectile reach its target!");
                    }
                    else
                    {
                        GameObject.Destroy(t_go);
                    }
                }
            }
        }
    }

}
