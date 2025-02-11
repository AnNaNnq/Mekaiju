using Mekaiju.Attribute;
using System;
using UnityEditor;
using UnityEngine;

namespace Mekaiju.Attribute
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class OpenPrefabButtonAttribute : PropertyAttribute { }
}

#if UNITY_EDITOR
namespace Mekaiju.Internal
{
    [CustomPropertyDrawer(typeof(OpenPrefabButtonAttribute))]
    public class OpenPrefabButtonDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Calcul des positions
            Rect objectFieldRect = new Rect(position.x, position.y, position.width - 30, position.height);
            Rect buttonRect = new Rect(position.x + position.width - 25, position.y, 25, position.height);

            // Affichage du champ GameObject
            EditorGUI.PropertyField(objectFieldRect, property, label);

            // Bouton d'ouverture
            if (GUI.Button(buttonRect, "🔍"))
            {
                GameObject prefab = property.objectReferenceValue as GameObject;
                if (prefab != null)
                {
                    string assetPath = AssetDatabase.GetAssetPath(prefab);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        AssetDatabase.OpenAsset(prefab);
                    }
                    else
                    {
                        Debug.LogWarning("L'objet sélectionné n'est pas une prefab valide.");
                    }
                }
                else
                {
                    Debug.LogWarning("Aucune prefab assignée.");
                }
            }

            EditorGUI.EndProperty();
        }
    }
}
#endif
