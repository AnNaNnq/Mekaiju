using UnityEngine;

namespace Mekaiju.Attribute
{
    public class FocusObjectAttribute : PropertyAttribute
    {
        
    }
}

#if UNITY_EDITOR

namespace Mekaiju.Internal
{
    using Mekaiju.Attribute;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(FocusObjectAttribute))]
    public class FocusTransformDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference &&
                property.objectReferenceValue is Transform transform)
            {
                Rect fieldRect = new Rect(position.x, position.y, position.width - 60, position.height);
                Rect buttonRect = new Rect(position.x + position.width - 55, position.y, 55, position.height);

                EditorGUI.PropertyField(fieldRect, property, label);

                if (GUI.Button(buttonRect, "Focus"))
                {
                    FocusOnTransform(transform);
                }
            }else if(property.propertyType == SerializedPropertyType.ObjectReference &&
                property.objectReferenceValue is GameObject obj)
            {
                Rect fieldRect = new Rect(position.x, position.y, position.width - 60, position.height);
                Rect buttonRect = new Rect(position.x + position.width - 55, position.y, 55, position.height);

                EditorGUI.PropertyField(fieldRect, property, label);

                if (GUI.Button(buttonRect, "Focus"))
                {
                    FocusOnTransform(obj.transform);
                }
            }
            else
            {
                Rect fieldRect = new Rect(position.x, position.y, position.width, position.height);
                EditorGUI.PropertyField(fieldRect, property, label);
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
