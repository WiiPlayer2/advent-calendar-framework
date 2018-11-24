using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;

[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    private ReorderableList reorderableList;

    private Path Path
    {
        get
        {
            return target as Path;
        }
    }

    private void OnEnable()
    {
        reorderableList = new ReorderableList(Path.Nodes, typeof(PathNode), true, true, true, true);

        // This could be used aswell, but I only advise this your class inherrits from UnityEngine.Object or has a CustomPropertyDrawer
        // Since you'll find your item using: serializedObject.FindProperty("list").GetArrayElementAtIndex(index).objectReferenceValue
        // which is a UnityEngine.Object
        // reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("list"), true, true, true, true);

        // Add listeners to draw events
        reorderableList.drawHeaderCallback += DrawHeader;
        reorderableList.drawElementCallback += DrawElement;

        reorderableList.onAddCallback += AddItem;
        reorderableList.onRemoveCallback += RemoveItem;
    }

    private void OnDisable()
    {
        // Make sure we don't get memory leaks etc.
        reorderableList.drawHeaderCallback -= DrawHeader;
        reorderableList.drawElementCallback -= DrawElement;

        reorderableList.onAddCallback -= AddItem;
        reorderableList.onRemoveCallback -= RemoveItem;
    }

    /// <summary>
    /// Draws the header of the list
    /// </summary>
    /// <param name="rect"></param>
    private void DrawHeader(Rect rect)
    {
        GUI.Label(rect, "Nodes");
    }

    /// <summary>
    /// Draws one element of the list (ListItemExample)
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="index"></param>
    /// <param name="active"></param>
    /// <param name="focused"></param>
    private void DrawElement(Rect rect, int index, bool active, bool focused)
    {
        PathNode item = Path.Nodes[index];

        // EditorGUI.BeginChangeCheck();
        // item.boolValue = EditorGUI.Toggle(new Rect(rect.x, rect.y, 18, rect.height), item.boolValue);
        // item.stringvalue = EditorGUI.TextField(new Rect(rect.x + 18, rect.y, rect.width - 18, rect.height), item.stringvalue);
        EditorGUI.LabelField(rect, $"{item.transform.position}");
        // if (EditorGUI.EndChangeCheck())
        // {
        //     EditorUtility.SetDirty(target);
        // }

        // If you are using a custom PropertyDrawer, this is probably better
        // EditorGUI.PropertyField(rect, serializedObject.FindProperty("list").GetArrayElementAtIndex(index));
        // Although it is probably smart to cach the list as a private variable ;)
    }

    private void AddItem(ReorderableList list)
    {
        var node = CreateNode(Path.transform.position);
        Path.Nodes.Add(node);
        Selection.activeGameObject = node.gameObject;

        EditorUtility.SetDirty(target);
    }

    private void RemoveItem(ReorderableList list)
    {
        var node = Path.Nodes[list.index];
        Path.Nodes.RemoveAt(list.index);
        DestroyImmediate(node.gameObject);

        EditorUtility.SetDirty(target);
    }

    private PathNode CreateNode(Vector3 position)
    {
        var gameObj = new GameObject($"{Path.name} | Path Node", typeof(PathNode));
        gameObj.transform.parent = Path.transform;
        gameObj.transform.position = position;
        var node = gameObj.GetComponent<PathNode>();
        node.Path = Path;
        return node;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(Path.IsBaked)
        {
            if(GUILayout.Button("Edit Nodes"))
            {
                foreach(var p in Path.Points)
                {
                    Path.Nodes.Add(CreateNode(p));
                }
                Path.IsBaked = false;
            }
        }
        else
        {
            reorderableList.DoLayoutList();
            if(GUILayout.Button("Bake Nodes"))
            {
                Path.BakePoints();
                foreach(var n in Path.Nodes)
                {
                    DestroyImmediate(n.gameObject);
                }
                Path.Nodes.Clear();
            }
        }
    }
}
