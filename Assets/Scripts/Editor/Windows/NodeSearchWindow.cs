using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

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

    private string GetAttackCategory(Type type)
    {
        string path = AssetDatabase.FindAssets($"t:MonoScript {type.Name}")
            .Select(AssetDatabase.GUIDToAssetPath)
            .FirstOrDefault(p => Path.GetFileNameWithoutExtension(p) == type.Name);

        if (string.IsNullOrEmpty(path))
            return "Uncategorized";

        // Obtenir le chemin relatif (sans "Assets/")
        string relativePath = Path.GetDirectoryName(path).Replace("\\", "/");
        if (relativePath.StartsWith("Assets/"))
            relativePath = relativePath.Substring(7);

        // Découper le chemin en dossiers
        string[] folders = relativePath.Split('/');

        // Vérifier qu'on a au moins deux dossiers (Kaiju / Phase)
        if (folders.Length >= 2)
        {
            return $"{folders[^2]}/{folders[^1]}"; // Retourne "Kaiju/Phase"
        }

        return folders.Length > 0 ? folders[^1] : "Uncategorized";
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry>
    {
        new SearchTreeGroupEntry(new GUIContent("Add Attack"), 0)
    };

        Type attackInterface = typeof(Mekaiju.AI.Attack.IAttack);
        var attackTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => attackInterface.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

        // Grouper les attaques par leur catégorie (Kaiju/Phase)
        var groupedAttacks = attackTypes.GroupBy(GetAttackCategory);

        Dictionary<string, SearchTreeGroupEntry> kaijuGroups = new();

        foreach (var group in groupedAttacks)
        {
            string[] levels = group.Key.Split('/');

            if (levels.Length < 2) continue; // On s'assure qu'on a bien "Kaiju/Phase"

            string kaijuName = levels[0];
            string phaseName = levels[1];

            // Ajout du Kaiju (niveau 1)
            if (!kaijuGroups.ContainsKey(kaijuName))
            {
                var kaijuEntry = new SearchTreeGroupEntry(new GUIContent(kaijuName), 1);
                tree.Add(kaijuEntry);
                kaijuGroups[kaijuName] = kaijuEntry;
            }

            // Ajout de la phase sous le Kaiju (niveau 2)
            var phaseEntry = new SearchTreeGroupEntry(new GUIContent(phaseName), 2);
            tree.Add(phaseEntry);

            // Ajouter les attaques sous la phase (niveau 3)
            foreach (var attackType in group)
            {
                tree.Add(new SearchTreeEntry(new GUIContent(attackType.Name, _indentationIcon))
                {
                    userData = attackType.Name,
                    level = 3
                });
            }
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
