using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mekaiju.Editor
{
    
    [CustomPropertyDrawer(typeof(Utils.EnumArray), true)]
    public class EnumArrayDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty p_property, GUIContent p_label)
        {
            if (!fieldInfo.FieldType.IsGenericType || fieldInfo.FieldType.GetGenericArguments().Length < 2)
            {
                return 0f;
            }

            var t_elemType = fieldInfo.FieldType.GetGenericArguments()[1];            
            if (t_elemType.IsSerializable || typeof(UnityEngine.Object).IsAssignableFrom(t_elemType))
            {
                if (p_property.isExpanded)
                {
                    Type t_kType = fieldInfo.FieldType.GetGenericArguments()[0];
                    int  t_size  = Enum.GetValues(t_kType).Length;

                    SerializedProperty array;
                    if (t_elemType.IsAbstract || t_elemType.IsInterface)
                        array = p_property.FindPropertyRelative("_refArray");
                    else
                        array = p_property.FindPropertyRelative("_valArray");

                    float t_totalHeight = EditorGUIUtility.singleLineHeight;

                    for (int i = 0; i < t_size; i++)
                    {
                        SerializedProperty t_elem = array.GetArrayElementAtIndex(i);
                        t_totalHeight += EditorGUI.GetPropertyHeight(t_elem, true) + EditorGUIUtility.standardVerticalSpacing;
                    }

                    return t_totalHeight;
                }

                return EditorGUIUtility.singleLineHeight;
            }

            return 0f;
        }


        public override void OnGUI(Rect p_position, SerializedProperty p_property, GUIContent p_label)
        {
            if (!fieldInfo.FieldType.IsGenericType || fieldInfo.FieldType.GetGenericArguments().Length < 2)
            {
                Debug.LogWarning("Seems to be a nested EnumArray (public class Foo: EnumArray<A, B> {})!");
                return;
            }

            EditorGUI.BeginProperty(p_position, p_label, p_property);

            var t_elemType = fieldInfo.FieldType.GetGenericArguments()[1];
            if (t_elemType.IsSerializable || typeof(UnityEngine.Object).IsAssignableFrom(t_elemType))
            {
                Rect t_foldRect = new(p_position.x, p_position.y, p_position.width, EditorGUIUtility.singleLineHeight);
                p_property.isExpanded = EditorGUI.Foldout(t_foldRect, p_property.isExpanded, p_label, true);

                if (p_property.isExpanded)
                {
                    Type     t_kType = fieldInfo.FieldType.GetGenericArguments()[0];
                    string[] t_names = Enum.GetNames(t_kType);

                    SerializedProperty array;
                    if (t_elemType.IsAbstract || t_elemType.IsInterface)
                        array = p_property.FindPropertyRelative("_refArray");
                    else
                        array = p_property.FindPropertyRelative("_valArray");

                    float t_yOffset = p_position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    for (int i = 0; i < t_names.Length; i++)
                    {
                        SerializedProperty t_elem  = array.GetArrayElementAtIndex(i);
                        GUIContent         t_label = new(t_names[i]);

                        EditorGUI.indentLevel++;

                        Rect t_elemRect = new(p_position.x, t_yOffset, p_position.width, EditorGUI.GetPropertyHeight(t_elem, true));
                        EditorGUI.PropertyField(t_elemRect, t_elem, t_label, true);

                        EditorGUI.indentLevel--;

                        t_yOffset += t_elemRect.height + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
            }

            EditorGUI.EndProperty();
        }
    }

}