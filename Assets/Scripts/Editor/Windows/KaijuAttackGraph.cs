using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class KaijuAttackGraph : EditorWindow
{
    private KaijuAttackGraphView _graphView;

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

        var t_nodeCreateButton = new Button(() => { _graphView.CreateNode("Attaque Node"); });
        t_nodeCreateButton.text = "Create Node";

        t_toolbar.Add(t_nodeCreateButton);
        rootVisualElement.Add(t_toolbar);
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }
}
