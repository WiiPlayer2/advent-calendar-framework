using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(TextMesh))]
public class TimelockedBoxDisplay : MonoBehaviour
{
    [SerializeField]
    private TimeLock timeLock;

    private TextMesh textMesh;

    private void Start()
    {
        textMesh = GetComponent<TextMesh>();
        Update();
    }

    private void Update()
    {
        textMesh.text = timeLock?.Day.ToString();
    }
}
