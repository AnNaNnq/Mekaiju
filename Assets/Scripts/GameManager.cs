using UnityEngine;

namespace Mekaiju
{

    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        public static GameManager Instance { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public PlayerData playerData { get; private set; }

        private void Awake()
        {
            // Singleton pattern to ensure only one instance of GameManager exists.
            if (Instance == null)
            {
                Instance = this;
                playerData = new();


                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
        
    }

}