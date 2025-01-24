using System;
using UnityEditor;
using UnityEngine;

namespace Mekaiju.Editor
{
    
    [CustomPropertyDrawer(typeof(Utils.EnumArray), true)]
    public class EnumArrayDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                Type kType = fieldInfo.FieldType.GetGenericArguments()[0];
                int  size  = Enum.GetValues(kType).Length;

                SerializedProperty array = property.FindPropertyRelative("_array");

                float totalHeight = EditorGUIUtility.singleLineHeight;

                for (int i = 0; i < size; i++)
                {
                    SerializedProperty elem = array.GetArrayElementAtIndex(i);
                    totalHeight += EditorGUI.GetPropertyHeight(elem, true) + EditorGUIUtility.standardVerticalSpacing;
                }

                return totalHeight;
            }

            return EditorGUIUtility.singleLineHeight;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect fRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(fRect, property.isExpanded, label, true);

            if (property.isExpanded)
            {
                Type kType     = fieldInfo.FieldType.GetGenericArguments()[0];
                string[] names = Enum.GetNames(kType);

                SerializedProperty array = property.FindPropertyRelative("_array");

                float yOffset = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                for (int i = 0; i < names.Length; i++)
                {
                    SerializedProperty elem = array.GetArrayElementAtIndex(i);

                    GUIContent cLabel = new(names[i]);

                    EditorGUI.indentLevel++;

                    Rect eRect = new(position.x, yOffset, position.width, EditorGUI.GetPropertyHeight(elem, true));
                    EditorGUI.PropertyField(eRect, elem, cLabel, true);

                    EditorGUI.indentLevel--;

                    yOffset += eRect.height + EditorGUIUtility.standardVerticalSpacing;
                }
            }

            EditorGUI.EndProperty();
        }
    }

}