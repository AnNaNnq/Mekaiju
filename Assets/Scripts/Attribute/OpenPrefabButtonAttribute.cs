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
            float buttonWidth = 25;
            Rect objectFieldRect = new Rect(position.x, position.y, position.width - (buttonWidth * 2 + 5), position.height);
            Rect openButtonRect = new Rect(position.x + position.width - (buttonWidth * 2 + 5), position.y, buttonWidth, position.height);
            Rect showInProjectButtonRect = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, position.height);

            // Affichage du champ GameObject
            EditorGUI.PropertyField(objectFieldRect, property, label);

            // Bouton d'ouverture de la prefab
            if (GUI.Button(openButtonRect, "🔍"))
            {
                OpenPrefab(property);
            }

            // Bouton pour afficher dans le projet
            if (GUI.Button(showInProjectButtonRect, "📂"))
            {
                ShowPrefabInProject(property);
            }

            EditorGUI.EndProperty();
        }

        private void OpenPrefab(SerializedProperty property)
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

        private void ShowPrefabInProject(SerializedProperty property)
        {
            UnityEngine.Object prefab = property.objectReferenceValue;
            if (prefab != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(prefab);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(assetPath);
                    EditorGUIUtility.PingObject(obj);
                    Selection.activeObject = obj;
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
    }
}
#endif