using Mekaiju.AI;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System.Linq;
using System.Collections.Generic;

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

        // TextField pour afficher le nom du fichier
        var t_fileNameTextField = new TextField("File Name");
        t_fileNameTextField.SetValueWithoutNotify(_fileName);
        t_fileNameTextField.MarkDirtyRepaint();
        t_fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
        t_toolbar.Add(t_fileNameTextField);


        var savedGraphs = GetSavedGraphFiles();
        var defaultFile = savedGraphs.Count > 0 ? savedGraphs[0] : "";

        // Dropdown pour choisir un fichier
        var loadDropdown = new DropdownField(savedGraphs, defaultFile);
        loadDropdown.RegisterValueChangedCallback(evt => _fileName = evt.newValue); // Met à jour _fileName
        t_toolbar.Add(loadDropdown);

        var t_nodeSaveButton = new Button(() => { RequestDataOperation(true); });
        t_nodeSaveButton.text = "Save";
        t_nodeSaveButton.clicked += () =>
        {
            RequestDataOperation(true);  // Sauvegarder le fichier
            UpdateDropdown(loadDropdown);  // Recharger les fichiers dans le dropdown
        };
        t_toolbar.Add(t_nodeSaveButton);

        // Bouton pour charger le fichier sélectionné
        var loadButton = new Button(() =>
        {
            var selectedFile = loadDropdown.value;  // Récupère le fichier sélectionné
            LoadSelectedGraph(selectedFile);         // Charge le fichier
            t_fileNameTextField.SetValueWithoutNotify(selectedFile);  // Met à jour le TextField avec le nom du fichier
        });
        loadButton.text = "Load";
        t_toolbar.Add(loadButton);

        rootVisualElement.Add(t_toolbar);
    }

    // Fonction pour recharger les fichiers dans le dropdown après un save
    private void UpdateDropdown(DropdownField loadDropdown)
    {
        var updatedFiles = GetSavedGraphFiles();  // Récupère la liste mise à jour
        loadDropdown.choices = updatedFiles;      // Met à jour la liste des fichiers
        if (!updatedFiles.Contains(loadDropdown.value))
        {
            loadDropdown.value = updatedFiles.FirstOrDefault();  // Sélectionne le premier fichier si le précédent est supprimé ou changé
        }
    }




    private void LoadSelectedGraph(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return;
        _fileName = fileName;
        RequestDataOperation(false);
    }

    private static List<string> GetSavedGraphFiles()
    {
        string path = "Assets/Resources/Kaijus/AttackGraph";
        if (!Directory.Exists(path)) return new List<string>();

        return Directory.GetFiles(path, "*.asset")
            .Select(Path.GetFileNameWithoutExtension)
            .ToList();
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
