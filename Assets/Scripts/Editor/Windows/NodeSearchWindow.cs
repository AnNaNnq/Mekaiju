using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private KaijuAttackGraphView _graphView;
    private EditorWindow _window;
    private Texture2D _indentationIcon;

    public void Init(EditorWindow window, KaijuAttackGraphView graphView)
    {
        _graphView = graphView;
        _window = window;

        _indentationIcon = new Texture2D(1, 1);
        _indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
        _indentationIcon.Apply();
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Add Attack"), 0),
            new SearchTreeGroupEntry(new GUIContent("Attack"), 1),
            new SearchTreeEntry(new GUIContent("Attack Node", _indentationIcon))
            {
                userData = "AttackNode", level = 2
            },
            // Bouton pour ajouter une node de départ
            new SearchTreeEntry(new GUIContent("Start Node", _indentationIcon))
            {
                userData = "StartNode", level = 2
            }
        };
        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        var worldMousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent,
                context.screenMousePosition - _window.position.position);
        var localMousePosition = _graphView.contentViewContainer.WorldToLocal(worldMousePosition);

        switch (searchTreeEntry.userData)
        {
            case "AttackNode":
                _graphView.CreateNode("Attaque Node", localMousePosition);
                return true;

            case "StartNode":
                _graphView.CreateStartNode(localMousePosition, "Start Node");
                return true;

            default:
                return false;
        }
    }
}
