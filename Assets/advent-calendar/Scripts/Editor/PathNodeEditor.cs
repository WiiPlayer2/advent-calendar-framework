using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathNode))]
public class PathNodeEditor : Editor
{
    private PathNode PathNode
    {
        get
        {
            return target as PathNode;
        }
    }

    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Attach to Path"))
        {
            PathNode.transform.SetParent(PathNode.Path.transform, true);
        }
        if(GUILayout.Button("Detach / Make Global"))
        {
            PathNode.transform.SetParent(null, true);
        }
    }
}
