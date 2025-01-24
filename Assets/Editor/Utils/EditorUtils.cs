using UnityEditor;
using UnityEngine;

public class EditorUtils
{
    SerializedObject _obj;

    public EditorUtils(SerializedObject p_obj)
    {
        this._obj = p_obj;
    }

    public bool AddClosableLabel(string p_label, bool p_foldout = true)
    {
        bool b = EditorGUILayout.Foldout(p_foldout, p_label);
        EditorGUI.indentLevel++;
        return b;

        
    }

    public void AddLabel(string p_label)
    {
        EditorGUILayout.LabelField(p_label);
        EditorGUI.indentLevel++;
    }

    public void AddProperty(string[] p_labels, bool p_interactable = true, string p_msg = "", bool p_show = true)
    {
        if (!p_show) return;

        if(!p_interactable) GUI.enabled = false;

        foreach (string t_label in p_labels) {
            EditorGUILayout.PropertyField(_obj.FindProperty(t_label));
        }

        if (!p_interactable) GUI.enabled = true;
        if(p_msg != "") EditorGUILayout.HelpBox(p_msg, MessageType.Info);
    }

    public void DrawHorizontalLine()
    {
        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
}
