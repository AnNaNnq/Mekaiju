using UnityEngine;

namespace Mekaiju
{

    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        private static GameManager _instance;
        public  static GameManager instance 
        { 
            get
            {
                if (!_instance)
                {
                    Debug.LogError("Missing GameManager.");
                }

                return _instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public PlayerData playerData { get; private set; }

        private void Awake()
        {
            // Singleton pattern to ensure only one instance of GameManager exists.
            if (_instance == null)
            {
                _instance = this;
                playerData = new();
                
                playerData.LoadMechaConfig();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

    }

}