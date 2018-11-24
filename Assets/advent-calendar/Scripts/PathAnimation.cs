﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathAnimation : MonoBehaviour
{
    private interface IPathAlgorithm
    {
        Vector3 GetPosition(float t);
    }

    private class SingleBezierAlgorithm : IPathAlgorithm
    {
        private readonly Vector3[] points;

        private readonly Vector3[] buffer;

        public SingleBezierAlgorithm(params Vector3[] points)
        {
            this.points = points;
            buffer = new Vector3[points.Length];
        }

        public Vector3 GetPosition(float t)
        {
            Array.Copy(points, buffer, points.Length);
            return GetPosition(t, points.Length);
        }

        private Vector3 GetPosition(float t, int length)
        {
            if (length == 0)
                return Vector3.zero;
            if (length == 1)
                return buffer[0];

            for (var i = 0; i < length - 1; i++)
            {
                buffer[i] = Vector3.Lerp(buffer[i], buffer[i + 1], t);
            }
            return GetPosition(t, length - 1);
        }
    }

    private class BezierSplinesAlgorithm : IPathAlgorithm
    {
        // <offset, length, spline>
        private readonly List<Tuple<float, float, SingleBezierAlgorithm>> splines = new List<Tuple<float, float, SingleBezierAlgorithm>>();

        private readonly Vector3[] points;

        public BezierSplinesAlgorithm(Vector3[] points)
        {
            this.points = points;
            if (points.Length < 3)
                return;

            CreateSplines();
        }

        private Vector3[] CalcSplinePoints()
        {
            var splinePoints = new Vector3[((points.Length - 2) * 2) + 1];

            splinePoints[0] = points[0];
            var j = 1;
            for (var i = 1; i < points.Length - 1; i++)
            {
                splinePoints[j] = points[i];
                splinePoints[j + 1] = Vector3.Lerp(points[i], points[i + 1], 0.5f);
                j += 2;
            }
            splinePoints[splinePoints.Length - 1] = points[points.Length - 1];
            return splinePoints;
        }

        private float CalcTotalLength()
        {
            var totalLength = 0f;
            for (var i = 0; i < points.Length - 1; i++)
            {
                totalLength += (points[i + 1] - points[i]).magnitude;
            }
            return totalLength;
        }

        private void CreateSplines()
        {
            var splinePoints = CalcSplinePoints();
            var totalLength = CalcTotalLength();
            var currentOffset = 0f;
            for (var i = 0; i < splinePoints.Length - 2; i += 2)
            {
                var length = (splinePoints[i] - splinePoints[i + 1]).magnitude + (splinePoints[i + 1] - splinePoints[i + 2]).magnitude;
                splines.Add(Tuple.Create(
                    currentOffset / totalLength,
                    length / totalLength,
                    new SingleBezierAlgorithm(splinePoints[i], splinePoints[i + 1], splinePoints[i + 2])));
                currentOffset += length;
            }
        }

        public Vector3 GetPosition(float t)
        {
            switch (points.Length)
            {
                case 0:
                    return Vector3.zero;
                case 1:
                    return points[0];
                case 2:
                    return Vector3.Lerp(points[0], points[1], t);
            }

            var data = splines.LastOrDefault(o => t >= o.Item1);
            if (data == null)
                return Vector3.zero;

            var scaledT = (t - data.Item1) / data.Item2;
            return data.Item3.GetPosition(scaledT);
        }
    }

    public enum PathAlgorithm
    {
        SingleBezier,
        BezierSplines,
    }

    public const int TESSELATION = 100;

    private const float TESSELATION_STEP = 1f / TESSELATION;

    public static readonly Color GIZMO_COLOR = Color.green;

    public static readonly Color GIZMO_PATH_COLOR = Color.cyan;

    [SerializeField]
    private PathAlgorithm algorithmType = PathAlgorithm.SingleBezier;

    [SerializeField, HideInInspector]
    public List<PathAnimationNode> Nodes;

    [SerializeField, HideInInspector]
    private Vector3[] points;

    private IPathAlgorithm pathAlgorithm;

    private void Start()
    {
        LoadPoints();
    }

    internal void OnDrawGizmos()
    {
        LoadPoints();

        Gizmos.color = GIZMO_COLOR;
        for (var i = 1; i < points.Length; i++)
        {
            Gizmos.DrawLine(points[i - 1], points[i]);
        }
    }

    private void LoadPoints()
    {
        points = Nodes?.Select(o => o.transform.position).ToArray() ?? new Vector3[0];
        switch (algorithmType)
        {
            case PathAlgorithm.SingleBezier:
                pathAlgorithm = new SingleBezierAlgorithm(points);
                break;
            case PathAlgorithm.BezierSplines:
                pathAlgorithm = new BezierSplinesAlgorithm(points);
                break;
            default:
                throw new NotImplementedException($"Algorithm {algorithmType} is not implemented yet.");
        }
    }

    internal void OnDrawGizmosSelected()
    {
        LoadPoints();
        if (points?.Length < 2)
            return;

        Gizmos.color = GIZMO_PATH_COLOR;
        var lastPoint = GetPosition(0);

        for (var t = TESSELATION_STEP; t <= 1f; t += TESSELATION_STEP)
        {
            var currentPoint = GetPosition(t);
            Gizmos.DrawLine(lastPoint, currentPoint);
            lastPoint = currentPoint;
        }
    }

    public Vector3 GetPosition(float t)
    {
        return pathAlgorithm.GetPosition(t);
    }
}
