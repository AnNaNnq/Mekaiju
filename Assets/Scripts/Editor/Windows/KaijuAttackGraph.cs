using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;

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
        GenerateMiniMap();
    }

    private void GenerateMiniMap()
    {
        var miniMap = new MiniMap();
        miniMap.SetPosition(new Rect(10, 30, 200, 140));
        _graphView.Add(miniMap);
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

        var t_nodeSaveButton = new Button(() => { RequestDataOperation(true); });
        t_nodeSaveButton.text = "Save";

        var t_nodeLoadButton = new Button(() => { RequestDataOperation(false); });
        t_nodeLoadButton.text = "Load";

        var t_nodeCreateButton = new Button(() => { _graphView.CreateNode("Attaque Node"); });
        t_nodeCreateButton.text = "Create Node";

        t_toolbar.Add(t_nodeCreateButton);
        t_toolbar.Add(t_nodeSaveButton);
        t_toolbar.Add(t_nodeLoadButton);
        rootVisualElement.Add(t_toolbar);
    }

    private void RequestDataOperation(bool p_save)
    {
        if (string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name!", "Please enter valid file name", "ok");
            return;
        }

        var t_saveUtility = GraphSaveUtility.GetInstance(_graphView);
        if (p_save)
        {
            t_saveUtility.SaveGraph(_fileName);
        }
        else
        {
            t_saveUtility.LoadGraph(_fileName);
        }
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }
}
