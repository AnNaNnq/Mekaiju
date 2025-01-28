using UnityEditor.EditorTools;
using UnityEngine;

namespace Mekaiju.Attribute
{
    public class FocusTransformAttribute : PropertyAttribute
    {
        // Cet attribut n'a pas besoin de propriétés pour le moment.
    }
}

#if UNITY_EDITOR

namespace Mekaiju.Internal
{
    using Mekaiju.Attribute;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(FocusTransformAttribute))]
    public class FocusTransformDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Vérifie si le champ est un Transform
            if (property.propertyType == SerializedPropertyType.ObjectReference &&
                property.objectReferenceValue is Transform transform)
            {
                // Divise l'espace pour le champ et le bouton
                Rect fieldRect = new Rect(position.x, position.y, position.width - 60, position.height);
                Rect buttonRect = new Rect(position.x + position.width - 55, position.y, 55, position.height);

                // Dessine le champ pour la Transform
                EditorGUI.PropertyField(fieldRect, property, label);

                // Ajoute le bouton à côté
                if (GUI.Button(buttonRect, "Focus"))
                {
                    // Centre la vue de la scène sur la Transform
                    FocusOnTransform(transform);
                }
            }
            else
            {
                // Affiche un message si la propriété n'est pas une Transform
                EditorGUI.LabelField(position, label.text, "Use with Transform only");
            }
        }

        private void FocusOnTransform(Transform transform)
        {
            // Sélectionne l'objet dans la hiérarchie
            Selection.activeTransform = transform;

            // Centre la vue de la scène sur l'objet
            SceneView.lastActiveSceneView.FrameSelected();
        }
    }
}

#endif
