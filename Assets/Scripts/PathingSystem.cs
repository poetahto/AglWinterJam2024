using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.Splines;
namespace Ltg8
{
    public class PathingSystem : MonoBehaviour
    {
        [SerializeField] private PathData[] paths;
        [SerializeField] private Transform hillsStart;
        [SerializeField] private Transform cityEnd;

        public Vector3 HillsStartPosition => hillsStart.position;
        public Vector3 CityEndPosition => cityEnd.position;

        public Spline CreatePathBetween(PathType type, Vector3 start, Vector3 end)
        {
            SplineContainer path = GetPath(type);
            Spline result = new Spline();
            result.Copy(path.Spline);
            for (int i = 0; i < result.Count; i++)
                result[i] = result[i].Transform(path.transform.localToWorldMatrix);
            result.Insert(0, new BezierKnot(start));
            result.Add(new BezierKnot(end));
            return result;
        }

        private SplineContainer GetPath(PathType type)
        {
            foreach (PathData pathData in paths)
            {
                if (pathData.type == type)
                    return pathData.path;
            }
            return default;
        }

        [Serializable]
        public class PathData
        {
            public PathType type;
            public SplineContainer path;
        }
    }
}
