using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Path))]
public class GiftAnimation : MonoBehaviour
{
    private class ChildTransformAnimation
    {
        private readonly GiftAnimation animation;

        private readonly GameObject gameObject;

        private Vector3 targetPosition;

        private Quaternion targetRotation;

        private Vector3 startingScale;

        private Vector3 targetScale;

        private Vector3 pathDifference;

        public ChildTransformAnimation(GiftAnimation animation, GameObject gameObject)
        {
            this.animation = animation;
            this.gameObject = gameObject;

            targetPosition = gameObject.transform.position;
            targetRotation = gameObject.transform.rotation;
            startingScale = gameObject.GetComponent<AnimationStartingScale>()?.StartingScale ?? Vector3.zero;
            targetScale = gameObject.transform.localScale;
            pathDifference = targetPosition - animation.path.GetPosition(1f);

            Update(0, 0, 0, animation.transform.position);
        }

        public void Update(float tPos, float tRotation, float tScale, Vector3 pathPostion)
        {
            gameObject.transform.position = pathPostion + pathDifference * tPos;
            gameObject.transform.rotation = Quaternion.LerpUnclamped(animation.transform.rotation, targetRotation, tRotation);
            gameObject.transform.localScale = Vector3.LerpUnclamped(startingScale, targetScale, tScale);
        }
    }

    private static readonly Color GIZMO_COLOR = Color.magenta;

    [SerializeField]
    private AnimationCurve positionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField]
    private AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField]
    private AnimationCurve scalingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField]
    private float animationTime = 5f;

    [SerializeField]
    private float waitTime = 1f;

    private Path path;

    private float timer = 0f;

    private ChildTransformAnimation[] childTransformAnimations;

    private void Start()
    {
        path = GetComponent<Path>();
        path.LoadAlgorithm();

        childTransformAnimations = new ChildTransformAnimation[transform.childCount];
        for (var i = 0; i < childTransformAnimations.Length; i++)
        {
            childTransformAnimations[i] = new ChildTransformAnimation(this, transform.GetChild(i).gameObject);
        }

        timer = -waitTime;
    }

    private void Update()
    {
        if (timer > animationTime)
            return;

        timer += Time.deltaTime;
        if (timer < 0f)
            return;

        var t = timer / animationTime;
        var tPos = positionCurve.Evaluate(t);
        var tRotation = rotationCurve.Evaluate(t);
        var tScale = scalingCurve.Evaluate(t);
        var pathPosition = path.GetPosition(tPos);
        foreach (var child in childTransformAnimations)
        {
            child.Update(tPos, tRotation, tScale, pathPosition);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = GIZMO_COLOR;
        Gizmos.DrawWireSphere(transform.position, 0.25f);
    }
}
