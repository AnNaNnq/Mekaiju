using System;
using UnityEngine;

namespace Mekaiju.Attribute
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class SelectFromListAttribute : PropertyAttribute
    {
        public string ListFieldName { get; }

        public SelectFromListAttribute(string listFieldName)
        {
            ListFieldName = listFieldName;
        }
    }
}

#if UNITY_EDITOR
namespace Mekaiju.Internal
{
    using Mekaiju.Attribute;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(SelectFromListAttribute))]
    public class SelectFromListDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SelectFromListAttribute selectFromList = (SelectFromListAttribute)attribute;
            SerializedObject serializedObject = property.serializedObject;
            SerializedProperty listProperty = serializedObject.FindProperty(selectFromList.ListFieldName);

            if (listProperty != null && listProperty.isArray)
            {
                List<string> options = new List<string>();
                for (int i = 0; i < listProperty.arraySize; i++)
                {
                    SerializedProperty element = listProperty.GetArrayElementAtIndex(i);
                    SerializedProperty nameProperty = element.FindPropertyRelative("nom"); // Récupère "nom"
                    options.Add(nameProperty != null ? nameProperty.stringValue : "Sans Nom");
                }

                // Récupère l'index sélectionné à partir de la propriété de type int
                int selectedIndex = property.intValue; // On utilise ici `intValue` pour récupérer l'index
                int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, options.ToArray());

                // Vérifie si la sélection a changé
                if (newIndex != selectedIndex)
                {
                    // Met à jour la valeur de la variable avec le nouvel index
                    property.intValue = newIndex;
                    property.serializedObject.ApplyModifiedProperties(); // Applique les modifications
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Liste introuvable");
            }
        }
    }
}
#endif
