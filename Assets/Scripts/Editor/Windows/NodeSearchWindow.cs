using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using Mekaiju.AI.Objet;

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

    private string GetAttackCategory(Type attackType)
    {
        if (attackType == null)
        {
            Debug.LogError("Type reçu est NULL !");
            return "Uncategorized";
        }

        // Trouver tous les ScriptableObjects de type KaijuAttack
        string[] guids = AssetDatabase.FindAssets("t:KaijuAttack", new[] { "Assets/Resources/Kaijus" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            KaijuAttack kaijuAttack = AssetDatabase.LoadAssetAtPath<KaijuAttack>(path);

            if (kaijuAttack == null)
                continue;

            // Vérifier si le KaijuAttack contient un `attack` du type recherché
            if (kaijuAttack.attack != null && kaijuAttack.attack.GetType() == attackType)
            {
                // Obtenir le chemin relatif après "Resources/"
                string relativePath = Path.GetDirectoryName(path).Replace("\\", "/");
                int index = relativePath.IndexOf("Resources/");
                if (index >= 0)
                    relativePath = relativePath.Substring(index + 10); // Supprime "Resources/"

                string[] folders = relativePath.Split('/');

                if (folders.Length >= 2)
                {
                    return $"{folders[^2]}/"; // Retourne "Kaijus"
                }

                return folders.Length > 0 ? folders[^1] : "Uncategorized";
            }
        }

        Debug.LogWarning($"Aucun KaijuAttack trouvé pour {attackType.FullName}");
        return "Uncategorized";
    }


    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry>
    {
        new SearchTreeGroupEntry(new GUIContent("Add Attack"), 0)
    };

        Type attackInterface = typeof(Mekaiju.AI.Attack.Attack);
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

            // Ajout du Kaiju (niveau 1)
            if (!kaijuGroups.ContainsKey(kaijuName))
            {
                var kaijuEntry = new SearchTreeGroupEntry(new GUIContent(kaijuName), 1);
                tree.Add(kaijuEntry);
                kaijuGroups[kaijuName] = kaijuEntry;
            }

            // Ajouter les attaques sous la phase (niveau 3)
            foreach (var attackType in group)
            {
                tree.Add(new SearchTreeEntry(new GUIContent(attackType.Name, _indentationIcon))
                {
                    userData = attackType.Name,
                    level = 2
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
