using UnityEngine;

namespace Mekaiju.AI.Body
{
    [System.Serializable]
    public class BodyPart
    {
        public string nom;
        public GameObject[] part;
        public float maxHealth;
        public float currentHealth;
        public bool isDestroyed;
    }
}
