using UnityEditor;
using UnityEngine;

public class KaijuAttackTree : EditorWindow
{
    private Vector2 scrollPosition;
    private Vector2 dragOffset;
    private bool isDragging;
    private float zoom = 1.0f;
    private const float zoomMin = 0.5f;
    private const float zoomMax = 2.0f;

    [MenuItem("Window/Kaiju Attack Tree")]
    public static void ShowWindow()
    {
        GetWindow<KaijuAttackTree>("Kaiju Attack Tree");
    }

    private void OnGUI()
    {
        HandleEvents();
        DrawGrid();
    }

    private void HandleEvents()
    {
        Event e = Event.current;

        if (e.type == EventType.ScrollWheel)
        {
            float zoomDelta = -e.delta.y * 0.05f;
            zoom = Mathf.Clamp(zoom + zoomDelta, zoomMin, zoomMax);
            Repaint();
        }

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
}