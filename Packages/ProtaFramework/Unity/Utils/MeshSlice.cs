using System.Collections.Generic;
using UnityEngine;
using Prota;
using System.Linq;

namespace Prota.Unity
{
    public static partial class Algorithm
    {
        public readonly struct MeshSliceInput
        {
            public readonly Vector3[] vertices;
            public readonly Vector2[] uvs;
            public readonly Color[] colors;
            public readonly int[] indices;
            public readonly Vector2[] slicerCurve;

            public MeshSliceInput(Vector3[] vertices, Vector2[] uvs, Color[] colors, int[] indices, Vector2[] slicerCurve)
            {
                this.vertices = vertices;
                this.uvs = uvs;
                this.colors = colors;
                this.indices = indices;
                this.slicerCurve = slicerCurve;
            }

            public MeshSliceInput(Mesh mesh, Vector2[] slicerCurve)
            {
                if (mesh == null) throw new System.ArgumentNullException(nameof(mesh));
                if (slicerCurve == null) throw new System.ArgumentNullException(nameof(slicerCurve));
                
                this.vertices = mesh.vertices;
                this.uvs = mesh.uv;
                this.colors = mesh.colors.Length > 0 ? mesh.colors : Enumerable.Repeat(Color.white, mesh.vertexCount).ToArray();
                this.indices = mesh.triangles;
                this.slicerCurve = slicerCurve;
            }
        }

        public readonly struct MeshSliceResult
        {
            public readonly Vector3[] vertices;
            public readonly Vector2[] uvs;
            public readonly Color[] colors;
            public readonly int[] indices;

            public MeshSliceResult(Vector3[] vertices, Vector2[] uvs, Color[] colors, int[] indices)
            {
                this.vertices = vertices;
                this.uvs = uvs;
                this.colors = colors;
                this.indices = indices;
            }
        }

        private static bool LineSegmentsIntersect(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
        {
            float d = (b2.y - b1.y) * (a2.x - a1.x) - (b2.x - b1.x) * (a2.y - a1.y);
            if (d == 0) return false;
            
            float ua = ((b2.x - b1.x) * (a1.y - b1.y) - (b2.y - b1.y) * (a1.x - b1.x)) / d;
            float ub = ((a2.x - a1.x) * (a1.y - b1.y) - (a2.y - a1.y) * (a1.x - b1.x)) / d;
            
            return ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1;
        }

        public static List<MeshSliceResult> SliceMesh2D(in MeshSliceInput input)
        {
            // Check for self-intersection
            for (int i = 0; i < input.slicerCurve.Length - 1; i++)
            {
                for (int j = i + 2; j < input.slicerCurve.Length - 1; j++)
                {
                    var p1 = new Vector3(input.slicerCurve[i].x, input.slicerCurve[i].y, 0);
                    var p2 = new Vector3(input.slicerCurve[i + 1].x, input.slicerCurve[i + 1].y, 0);
                    var p3 = new Vector3(input.slicerCurve[j].x, input.slicerCurve[j].y, 0);
                    var p4 = new Vector3(input.slicerCurve[j + 1].x, input.slicerCurve[j + 1].y, 0);
                    if (LineSegmentsIntersect(p1, p2, p3, p4))
                    {
                        throw new System.Exception("Slicer curve cannot intersect itself");
                    }
                }
            }

            var submeshes = new List<(List<Vector3> vertices, List<Vector2> uvs, List<Color> colors, List<int> indices)>();

            // Find intersections and split triangles
            for (int i = 0; i < input.indices.Length; i += 3)
            {
                var tri = new int[] { input.indices[i], input.indices[i + 1], input.indices[i + 2] };

                bool isIntersected = false;
                for (int j = 0; j < input.slicerCurve.Length - 1; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        var slicerP1 = new Vector3(input.slicerCurve[j].x, input.slicerCurve[j].y, 0);
                        var slicerP2 = new Vector3(input.slicerCurve[j + 1].x, input.slicerCurve[j + 1].y, 0);
                        if (LineSegmentsIntersect(
                            input.vertices[tri[k]], 
                            input.vertices[tri[(k + 1) % 3]],
                            slicerP1, slicerP2))
                        {
                            isIntersected = true;
                            break;
                        }
                    }
                    if (isIntersected) break;
                }

                // If triangle isn't intersected, add it to appropriate submesh
                if (!isIntersected)
                {
                    bool isInside = IsPointInPolygon(input.vertices[tri[0]], input.slicerCurve);
                    int submeshIndex = isInside ? 0 : 1;
                    
                    while (submeshes.Count <= submeshIndex)
                        submeshes.Add((new List<Vector3>(), new List<Vector2>(), new List<Color>(), new List<int>()));
                        
                    var submesh = submeshes[submeshIndex];
                    int baseIndex = submesh.vertices.Count;
                    
                    submesh.vertices.AddRange(new[] { input.vertices[tri[0]], input.vertices[tri[1]], input.vertices[tri[2]] });
                    submesh.uvs.AddRange(new[] { input.uvs[tri[0]], input.uvs[tri[1]], input.uvs[tri[2]] });
                    submesh.colors.AddRange(new[] { input.colors[tri[0]], input.colors[tri[1]], input.colors[tri[2]] });
                    submesh.indices.AddRange(new[] { baseIndex, baseIndex + 1, baseIndex + 2 });
                }
            }

            // Create result objects
            List<MeshSliceResult> results = new List<MeshSliceResult>();
            foreach (var submesh in submeshes)
            {
                results.Add(new MeshSliceResult(
                    vertices: submesh.vertices.ToArray(),
                    uvs: submesh.uvs.ToArray(),
                    colors: submesh.colors.ToArray(),
                    indices: submesh.indices.ToArray()
                ));
            }

            return results;
        }

        private static bool IsPointInPolygon(Vector3 point, Vector2[] polygon)
        {
            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].y > point.y) != (polygon[j].y > point.y)) &&
                    (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / 
                    (polygon[j].y - polygon[i].y) + polygon[i].x))
                {
                    inside = !inside;
                }
            }
            return inside;
        }

        #if UNITY_EDITOR
        public static partial class Tests
        {
            public static void SliceMesh2D_SingleTriangle_ShouldSplitCorrectly()
            {
                // Arrange
                var input = new MeshSliceInput(
                    vertices: new Vector3[] {
                        new Vector3(0, 0, 0),
                        new Vector3(1, 0, 0),
                        new Vector3(0, 1, 0)
                    },
                    uvs: new Vector2[] {
                        new Vector2(0, 0),
                        new Vector2(1, 0),
                        new Vector2(0, 1)
                    },
                    colors: new Color[] {
                        Color.white,
                        Color.white,
                        Color.white
                    },
                    indices: new int[] { 0, 1, 2 },
                    slicerCurve: new Vector2[] {
                        new Vector2(-1, 0.5f),
                        new Vector2(2, 0.5f)
                    }
                );

                // Act
                var results = SliceMesh2D(in input);

                // Assert
                UnityEngine.Debug.Assert(results.Count == 2, "Should create two submeshes");
            }

            public static void SliceMesh2D_SelfIntersectingCurve_ShouldThrowException()
            {
                var input = new MeshSliceInput(
                    vertices: new Vector3[] { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 1, 0) },
                    uvs: new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1) },
                    colors: new Color[] { Color.white, Color.white, Color.white },
                    indices: new int[] { 0, 1, 2 },
                    slicerCurve: new Vector2[] {
                        new Vector2(0, 0),
                        new Vector2(1, 1),
                        new Vector2(1, 0),
                        new Vector2(0, 1)
                    }
                );

                bool exceptionThrown = false;
                try 
                {
                    SliceMesh2D(in input);
                }
                catch (System.Exception)
                {
                    exceptionThrown = true;
                }
                UnityEngine.Debug.Assert(exceptionThrown, "Should throw exception for self-intersecting curve");
            }
        }
        #endif
    }
}
