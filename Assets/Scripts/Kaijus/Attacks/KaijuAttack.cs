using Mekaiju.Attributes;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

namespace Mekaiju.AI.Objet
{
    [CreateAssetMenu(fileName = "New Attack", menuName = "Kaiju/Attack")]
    public class KaijuAttack : ScriptableObject
    {
        /// <summary>
        /// 
        /// </summary>
        [field: SerializeReference, SubclassPicker]
        public Attack.Attack attack { get; private set; }

        private void OnEnable()
        {
            AssignDefaultAttack();
        }

        private void OnValidate()
        {
            AssignDefaultAttack();
        }

        private void AssignDefaultAttack()
        {
            if (attack == null)
            {
                // Récupérer le nom de cet asset
                string attackName = name;

                // Trouver une sous-classe de Attack qui porte ce nom
                Type attackType = GetAttackSubclassByName(attackName);

                if (attackType != null)
                {
                    // Créer une instance de la classe trouvée
                    attack = (Attack.Attack)Activator.CreateInstance(attackType);
                    Debug.Log($"Assigned default attack: {attackType.Name}");
                }
                else
                {
                    Debug.LogWarning($"No matching Attack subclass found for '{attackName}'. Please assign one manually.");
                }
            }
        }

        private Type GetAttackSubclassByName(string className)
        {
            return Assembly.GetAssembly(typeof(Attack.Attack))?
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Attack.Attack)))
                .FirstOrDefault(t => t.Name.Equals(className, StringComparison.OrdinalIgnoreCase));
        }
    }
}
