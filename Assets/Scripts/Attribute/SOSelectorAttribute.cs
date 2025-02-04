using UnityEngine;

namespace Mekaiju.Attribute
{
    public class SOSelectorAttribute : PropertyAttribute
    {

    }
}

#if UNITY_EDITOR

namespace Mekaiju.Internal
{
    using UnityEditor;
    using System.Linq;
    using Mekaiju.Attribute;

    [CustomPropertyDrawer(typeof(SOSelectorAttribute))]
    public class ScriptableObjectDropdownDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                var type = fieldInfo.FieldType;
                var scriptableObjects = AssetDatabase.FindAssets("t:" + type.Name)
                    .Select(guid => AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid)))
                    .Where(obj => obj != null)
                    .ToArray();

                string[] options = scriptableObjects.Select(obj => obj.name).ToArray();
                int currentIndex = System.Array.IndexOf(scriptableObjects, property.objectReferenceValue);

                currentIndex = EditorGUI.Popup(position, label.text, currentIndex, options);

                property.objectReferenceValue = currentIndex >= 0 ? scriptableObjects[currentIndex] : null;
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use with ScriptableObject only");
            }

            EditorGUI.EndProperty();
        }
    }
}

#endif