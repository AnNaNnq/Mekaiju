using UnityEditor;
using UnityEngine;
using Mekaiju.Attributes;

namespace Mekaiju.Editor
{
    [CustomPropertyDrawer(typeof(FoldableAttribute))]
    public class FoldableDrawer : PropertyDrawer
    {
        private bool isFolded = true; // �tat pli� ou d�pli�

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FoldableAttribute foldable = (FoldableAttribute)attribute;

            // Dessiner le header pliable avec un fond
            Rect headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            // Style pour le header
            GUIStyle headerStyle = new GUIStyle(EditorStyles.foldout);
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.normal.textColor = Color.white;
            headerStyle.alignment = TextAnchor.MiddleLeft;

            // Afficher le Foldout
            isFolded = EditorGUI.Foldout(headerRect, isFolded, foldable.Header, true, headerStyle);

            if (!isFolded)
            {
                float yOffset = position.y + EditorGUIUtility.singleLineHeight + 2;

                // It�rer sur les propri�t�s enfants et afficher celles qui ne sont pas pli�es
                SerializedProperty iterator = property.Copy();
                iterator.NextVisible(true); // Assurer qu'on commence � partir de la premi�re propri�t� visible

                while (iterator.NextVisible(false))
                {
                    // Afficher chaque propri�t�
                    Rect propertyRect = new Rect(position.x, yOffset, position.width, EditorGUI.GetPropertyHeight(iterator, label, true));
                    EditorGUI.PropertyField(propertyRect, iterator, true);

                    yOffset += EditorGUI.GetPropertyHeight(iterator, label, true) + 2;
                }
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (isFolded)
            {
                return EditorGUIUtility.singleLineHeight; // Hauteur du seul header
            }

            // Hauteur totale avec toutes les propri�t�s enfant visibles
            float height = EditorGUIUtility.singleLineHeight;
            SerializedProperty iterator = property.Copy();
            iterator.NextVisible(true);

            while (iterator.NextVisible(false))
            {
                height += EditorGUI.GetPropertyHeight(iterator, label, true) + 2;
            }

            return height;
        }

    }
}
