using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Mekaiju.Editor
{
    [CustomPropertyDrawer(typeof(Attributes.SubclassPickerAttribute))]
    public class SubclassPickerPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty p_property, GUIContent p_label)
        {
            return EditorGUI.GetPropertyHeight(p_property);
        }

        IEnumerable GetClasses(Type p_baseType)
        {
            return Assembly.GetAssembly(p_baseType).GetTypes().Where(t => t.IsClass && !t.IsAbstract && p_baseType.IsAssignableFrom(t));
        }

        public override void OnGUI(Rect p_position, SerializedProperty p_property, GUIContent p_label)
        {
            Type t_type = fieldInfo.FieldType;

            if (t_type.IsGenericType && t_type.GetGenericTypeDefinition() == typeof(List<>))
            {
                t_type = t_type.GetGenericArguments()[0];
            }

            if (t_type.IsArray && t_type.FullName.EndsWith("[]")) 
            { 
                t_type = Type.GetType(t_type.FullName[..^2]); 
            } 
            string t_typeName = p_property.managedReferenceValue?.GetType().Name ?? "None";

            Rect t_dropdownRect   = p_position;
            t_dropdownRect.x     += EditorGUIUtility.labelWidth + 2;
            t_dropdownRect.width -= EditorGUIUtility.labelWidth + 2;
            t_dropdownRect.height = EditorGUIUtility.singleLineHeight;
            if (EditorGUI.DropdownButton(t_dropdownRect, new(t_typeName), FocusType.Keyboard))
            {
                GenericMenu menu = new();

                // null
                menu.AddItem(new GUIContent("None"), p_property.managedReferenceValue == null, () =>
                {
                    p_property.managedReferenceValue = null;
                    p_property.serializedObject.ApplyModifiedProperties();
                });

                // inherited types
                foreach (Type type in GetClasses(t_type))
                {
                    menu.AddItem(new GUIContent(type.Name), t_typeName == type.Name, () =>
                    {
                        p_property.managedReferenceValue = type.GetConstructor(Type.EmptyTypes).Invoke(null);
                        p_property.serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.ShowAsContext();
            }
            EditorGUI.PropertyField(p_position, p_property, p_label, true);
        }
    }
}