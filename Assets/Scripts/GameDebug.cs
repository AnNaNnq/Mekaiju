using Mekaiju;
using UnityEngine;

public class GameDebug : MonoBehaviour
{
    public void ReloadMechaConfig()
    {
        GameManager.instance.playerData.LoadMechaConfig();
    }
}
