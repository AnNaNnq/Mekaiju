using Mekaiju.AI;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class KaijuAttackGraph : EditorWindow
{
    private KaijuAttackGraphView _graphView;
    private string _fileName = "New Attack";
    private static BasicAI _selectedPrefab;

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
        LoadBasicAIData();
    }

    public static void OpenKaijuGraphWindowWithPrefab(BasicAI prefab)
    {
        var window = GetWindow<KaijuAttackGraph>();
        window.titleContent = new GUIContent("Kaiju Attack Graph");
        _selectedPrefab = prefab;
    }


    private void GenerateMiniMap()
    {
        var miniMap = new MiniMap();
        var cords = _graphView.contentViewContainer.WorldToLocal(new Vector2(this.maxSize.x - 10, 30));
        miniMap.SetPosition(new Rect(cords.x, cords.y, 200, 140));
        _graphView.Add(miniMap);
    }

    private void ConstructGraphView()
    {
        _graphView = new KaijuAttackGraphView(this)
        {
            name = "Kaiju Attack",
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

    private void LoadBasicAIData()
    {
        if (_selectedPrefab == null)
        {
            Debug.LogWarning("No Prefab Selected!");
            return;
        }

        BasicAI aiScript = _selectedPrefab.GetComponentInChildren<BasicAI>();
        if (aiScript == null)
        {
            Debug.LogError("BasicAI script not found in the selected prefab!");
            return;
        }

        Debug.Log($"Loaded BasicAI Data: Phase = {aiScript.nbPhase}");
    }
}
