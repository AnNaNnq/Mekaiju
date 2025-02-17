using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class KaijuAttackTree : EditorWindow
{
    private Vector2 scrollPosition;
    private Vector2 dragOffset;
    private bool isDragging;
    private float zoom = 1.0f;
    private const float zoomMin = 0.5f;
    private const float zoomMax = 2.0f;

    // Liste des nodes
    private List<Node> nodes = new List<Node>();
    private Node selectedNode;  // Référence à la node sélectionnée

    [MenuItem("Window/Kaiju Attack Tree")]
    public static void ShowWindow()
    {
        GetWindow<KaijuAttackTree>("Kaiju Attack Tree");
    }

    private void OnGUI()
    {
        HandleEvents();
        DrawGrid();
        DrawNodes();
    }

    private void HandleEvents()
    {
        Event e = Event.current;

        // Gestion du zoom avec la molette
        if (e.type == EventType.ScrollWheel)
        {
            float zoomDelta = -e.delta.y * 0.05f;
            zoom = Mathf.Clamp(zoom + zoomDelta, zoomMin, zoomMax);
            Repaint();
        }

        // Gestion du déplacement avec le bouton du milieu (clic molette)
        if (e.type == EventType.MouseDown && e.button == 2)
        {
            isDragging = true;
            dragOffset = e.mousePosition;
            EditorGUIUtility.SetWantsMouseJumping(1);
        }
        else if (e.type == EventType.MouseDrag && e.button == 2 && isDragging)
        {
            Vector2 delta = e.mousePosition - dragOffset;
            scrollPosition += delta;
            dragOffset = e.mousePosition;
            EditorGUIUtility.SetWantsMouseJumping(1);
            EditorGUIUtility.AddCursorRect(new Rect(0, 0, position.width, position.height), MouseCursor.Pan);
            Repaint();
        }
        else if (e.type == EventType.MouseUp && e.button == 2)
        {
            isDragging = false;
            EditorGUIUtility.SetWantsMouseJumping(0);
            Repaint();
        }

        // Ajout du menu contextuel avec clic droit
        if (e.type == EventType.MouseDown && e.button == 1)
        {
            ShowContextMenu(e.mousePosition);
            e.Use(); // Empêche le clic droit de déclencher l'événement par défaut
        }

        // Sélectionner une node sur un clic gauche
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Vector2 mousePosition = e.mousePosition;

            // Ajuster la position de la souris en prenant en compte le zoom et le défilement
            mousePosition = (mousePosition - scrollPosition) / zoom;

            foreach (var node in nodes)
            {
                Rect nodeRect = new Rect(node.position.x * zoom, node.position.y * zoom, 150, 50);

                if (nodeRect.Contains(mousePosition))  // Si la souris est sur la node
                {
                    selectedNode = node;  // Sélectionner la node
                    Debug.Log("touch");   // Affiche "touch" dans la console
                    Repaint();
                    break;
                }
            }
        }

        if (isDragging)
        {
            EditorGUIUtility.AddCursorRect(new Rect(0, 0, position.width, position.height), MouseCursor.Pan);
        }
    }

    private void DrawGrid()
    {
        int gridSpacing = Mathf.RoundToInt(20 * zoom);
        int gridOpacity = 50;
        Color gridColor = new Color(0f, 0f, 0f, gridOpacity / 255f);

        Handles.BeginGUI();
        Handles.color = gridColor;

        float width = position.width * 2;
        float height = position.height * 2;

        for (float x = -scrollPosition.x % gridSpacing; x < width; x += gridSpacing)
            Handles.DrawLine(new Vector3(x, 0, 0), new Vector3(x, height, 0));

        for (float y = -scrollPosition.y % gridSpacing; y < height; y += gridSpacing)
            Handles.DrawLine(new Vector3(0, y, 0), new Vector3(width, y, 0));

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    // Méthode pour afficher les nodes
    private void DrawNodes()
    {
        foreach (var node in nodes)
        {
            // Crée un style personnalisé pour la node avec la couleur de fond #08494d et le texte en blanc
            GUIStyle nodeStyle = new GUIStyle(GUI.skin.box)
            {
                normal =
                {
                    background = MakeTex(1, 1, new Color(8f / 255f, 73f / 255f, 77f / 255f)), // Couleur #08494d pour le fond
                    textColor = Color.white // Texte en blanc
                },
                alignment = TextAnchor.MiddleCenter
            };

            // Dessiner les nodes en tenant compte du zoom et du scroll
            Rect nodeRect = new Rect(node.position.x * zoom + scrollPosition.x, node.position.y * zoom + scrollPosition.y, 150, 50);

            // Si la node est sélectionnée, dessiner une bordure blanche autour
            if (node == selectedNode)
            {
                EditorGUI.DrawRect(nodeRect, Color.white);  // Bordure blanche
            }

            GUI.Box(nodeRect, node.text, nodeStyle);  // Dessiner la node
        }
    }

    // Méthode pour créer une texture de couleur unie (pour la couleur de fond de la node)
    private Texture2D MakeTex(int width, int height, Color color)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++) pix[i] = color;
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(pix);
        texture.Apply();
        return texture;
    }

    // Méthode pour afficher le menu contextuel
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu contextMenu = new GenericMenu();

        contextMenu.AddItem(new GUIContent("Hello World"), false, () =>
        {
            // Calcule la position réelle de la souris en prenant en compte le zoom et le scroll
            Vector2 adjustedMousePosition = (mousePosition - scrollPosition) / zoom;

            // Ajout d'une nouvelle node à la position de la souris ajustée
            Node newNode = new Node
            {
                position = adjustedMousePosition,  // Position relative ajustée
                text = "Hello World"
            };
            nodes.Add(newNode);

            Repaint();  // Repeindre pour afficher la nouvelle node
        });

        contextMenu.ShowAsContext();
    }

    // Classe Node pour représenter une node
    private class Node
    {
        public Vector2 position;
        public string text;
    }
}
