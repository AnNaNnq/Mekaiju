using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            new SearchTreeGroupEntry(new GUIContent("Attack"), 1)
        };

        // Récupérer tous les scripts implémentant IAttack
        Type attackInterface = typeof(Mekaiju.AI.IAttack);
        var attackTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => attackInterface.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

        foreach (var attackType in attackTypes)
        {
            tree.Add(new SearchTreeEntry(new GUIContent(attackType.Name, _indentationIcon))
            {
                userData = attackType.Name,
                level = 2
            });
        }

        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        var worldMousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent,
                context.screenMousePosition - _window.position.position);
        var localMousePosition = _graphView.contentViewContainer.WorldToLocal(worldMousePosition);

        if (searchTreeEntry.userData.ToString() != null)
        {
            _graphView.CreateNode(searchTreeEntry.userData.ToString(), localMousePosition);
            return true;
        }
        return false;
    }
}
