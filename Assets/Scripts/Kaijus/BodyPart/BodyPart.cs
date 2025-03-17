using MyBox;
using UnityEngine;

namespace Mekaiju.AI.Body
{
    [System.Serializable]
    public class BodyPart
    {
        public string nom;
        public GameObject[] part;
        public float maxHealth;
        [ReadOnly]
        public float currentHealth;
        [ReadOnly]
        public bool isDestroyed;
    }
}
