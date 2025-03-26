using MyBox;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshReloader : MonoBehaviour
{
    public GameObject[] destroyables;
    public NavMeshSurface surfaceOne, surfaceTow;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            DestroyObject();
        }
    }

    public void DestroyObject()
    {
        destroyables.ForEach(x => Destroy(x));
        surfaceOne.enabled = false;
        surfaceTow.enabled = true;
    }
}
