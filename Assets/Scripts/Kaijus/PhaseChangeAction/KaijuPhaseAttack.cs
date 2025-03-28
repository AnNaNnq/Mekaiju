using Mekaiju.Attributes;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Mekaiju.AI.Objet
{
    [CreateAssetMenu(fileName = "New Phase Attack", menuName = "Kaiju/Phase Attack")]
    public class KaijuPhaseAttack : ScriptableObject
    {
        [field: SerializeReference, SubclassPicker]
        public PhaseAttack.PhaseAttack attack { get; private set; }

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
                Type attackType = GetAttackSubclassByName(attackName + "PhaseChange");

                if (attackType != null)
                {
                    // Créer une instance de la classe trouvée
                    attack = (PhaseAttack.PhaseAttack)Activator.CreateInstance(attackType);
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
            return Assembly.GetAssembly(typeof(PhaseAttack.PhaseAttack))?
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(PhaseAttack.PhaseAttack)))
                .FirstOrDefault(t => t.Name.Equals(className, StringComparison.OrdinalIgnoreCase));
        }
    }
}
