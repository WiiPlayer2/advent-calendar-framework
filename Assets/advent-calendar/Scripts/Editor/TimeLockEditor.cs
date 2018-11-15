using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TimeLock))]
public class TimeLockEditor : Editor
{
    private void OnEnable()
    {
        EditorHelper.EnsureTagExists(TimeLock.TAG);
    }
}
