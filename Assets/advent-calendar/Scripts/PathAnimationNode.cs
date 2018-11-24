using UnityEngine;

[ExecuteInEditMode]
public class PathAnimationNode : MonoBehaviour
{
    [HideInInspector, SerializeField]
    private PathAnimation path;

    private void Start()
    {
        path = GetComponentInParent<PathAnimation>();
        if (path == null)
            Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        path?.OnDrawGizmos();
        Gizmos.color = PathAnimation.GIZMO_COLOR;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }

    private void OnDrawGizmosSelected()
    {
        path?.OnDrawGizmosSelected();
    }
}
