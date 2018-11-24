using UnityEngine;

[ExecuteInEditMode]
public class PathNode : MonoBehaviour
{
    [HideInInspector, SerializeField]
    public Path Path;

    private void Update()
    {
        if (Path == null)
            DestroyImmediate(gameObject);
    }

    private void OnDrawGizmos()
    {
        Path?.OnDrawGizmos();
        Gizmos.color = Path.GIZMO_COLOR;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }

    private void OnDrawGizmosSelected()
    {
        Path?.OnDrawGizmosSelected();
    }
}
