using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class KaijuAttackGraph : EditorWindow
{
    private KaijuAttackGraphView _graphView;
    private string _fileName = "New Attack";

    [MenuItem("Graph/Attack Graph")]
    public static void OpenKaijuGraphWindow()
    {
        var window = GetWindow<KaijuAttackGraph>();
        window.titleContent = new GUIContent("Kaiju Attack Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolBar();
    }

    private void ConstructGraphView()
    {
        _graphView = new KaijuAttackGraphView
        {
            name = "Kaiju Attack"
        };

        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void GenerateToolBar()
    {
        var t_toolbar = new Toolbar();

        var t_fileNameTextField = new TextField("File Name");
        t_fileNameTextField.SetValueWithoutNotify(_fileName);
        t_fileNameTextField.MarkDirtyRepaint();
        t_fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
        t_toolbar.Add(t_fileNameTextField);

        var t_nodeSaveButton = new Button(() => { SaveData(); });
        t_nodeSaveButton.text = "Save";

        var t_nodeLoadButton = new Button(() => { LoadData(); });
        t_nodeLoadButton.text = "Load";

        var t_nodeCreateButton = new Button(() => { _graphView.CreateNode("Attaque Node"); });
        t_nodeCreateButton.text = "Create Node";

        t_toolbar.Add(t_nodeCreateButton);
        t_toolbar.Add(t_nodeSaveButton);
        t_toolbar.Add(t_nodeLoadButton);
        rootVisualElement.Add(t_toolbar);
    }

    private void LoadData()
    {
        throw new NotImplementedException();
    }

    private void SaveData()
    {
        throw new NotImplementedException();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }
}
