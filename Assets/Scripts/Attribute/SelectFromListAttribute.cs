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
                    SerializedProperty nameProperty = element.FindPropertyRelative("nom"); // R�cup�re "nom"
                    options.Add(nameProperty != null ? nameProperty.stringValue : "Sans Nom");
                }

                // R�cup�re l'index s�lectionn� � partir de la propri�t� de type int
                int selectedIndex = property.intValue; // On utilise ici `intValue` pour r�cup�rer l'index
                int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, options.ToArray());

                // V�rifie si la s�lection a chang�
                if (newIndex != selectedIndex)
                {
                    // Met � jour la valeur de la variable avec le nouvel index
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
