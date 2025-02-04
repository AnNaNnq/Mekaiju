using UnityEngine;
using Mekaiju.CameraSystem;


namespace Mekaiju.MechaControl
{

    public class MechaStateController : MonoBehaviour
    {

        public CameraFollowTPS CameraFollow;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(1)) // Clique droit pour passer en mode combat
            {
                CameraFollow.ChangeMechaState(CameraFollowTPS.MechaState.Combat);
            }
            else
            {
                CameraFollow.ChangeMechaState(CameraFollowTPS.MechaState.Idle);
            }
        }

    }
}


