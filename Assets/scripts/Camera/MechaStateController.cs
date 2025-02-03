using UnityEngine;

public class MechaStateController : MonoBehaviour
{
    
    public CameraFollowTPS cameraFollow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //cameraFollow.ChangeMechaState(CameraFollowTPS.MechaState.Sprint);
        }
        else if (Input.GetMouseButton(1)) // Clique droit pour passer en mode combat
        {
            cameraFollow.ChangeMechaState(CameraFollowTPS.MechaState.Combat);
        }
        else
        {
            cameraFollow.ChangeMechaState(CameraFollowTPS.MechaState.Idle);
        }
    }
    

}


