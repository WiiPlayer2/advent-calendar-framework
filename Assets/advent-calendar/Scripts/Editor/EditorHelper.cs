using System.Diagnostics;
using UnityEditor;

public static class EditorHelper
{
    // https://answers.unity.com/questions/33597/is-it-possible-to-create-a-tag-programmatically.html
    public static void EnsureTagExists(string tag)
    {
        // Open tag manager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        // First check if it is not already present
        bool found = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(tag)) { found = true; break; }
        }

        // if not found, add it
        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
            n.stringValue = tag;
        }

        // and to save the changes
        tagManager.ApplyModifiedProperties();
    }

    public static void EnsureLayerExists(string layerName, int layerIndex = 10)
    {
        // Open tag manager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        // For Unity 5 we need this too
        SerializedProperty layersProp = tagManager.FindProperty("layers");

        // --- Unity 5 ---
        SerializedProperty sp = layersProp.GetArrayElementAtIndex(layerIndex);
        if (sp != null) sp.stringValue = layerName;
        // and to save the changes
        tagManager.ApplyModifiedProperties();
    }
}
