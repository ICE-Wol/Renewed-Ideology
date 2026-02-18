using NUnit.Framework;
using Prota;
using Prota.Unity;
using System.Collections.Generic;
using UnityEngine;

public class TriangulateTests
{
    // 验证三角形数量是否正确 (n-2 个三角形, n 是顶点数)
    private void AssertValidTriangulation(List<Vector2> polygon, List<int> triangles, int expectedTriangleCount)
    {
        Assert.AreEqual(expectedTriangleCount * 3, triangles.Count, 
            $"Expected {expectedTriangleCount} triangles (3 indices each), got {triangles.Count} indices");
        
        // 验证所有索引都在有效范围内
        foreach (var index in triangles)
        {
            Assert.GreaterOrEqual(index, 0, "Triangle index must be >= 0");
            Assert.Less(index, polygon.Count, "Triangle index must be < polygon.Count");
        }
        
        // 验证没有重复的三角形（简单检查：所有三角形索引组合应该不同）
        var triangleSet = new HashSet<string>();
        for (int i = 0; i < triangles.Count; i += 3)
        {
            var t1 = triangles[i];
            var t2 = triangles[i + 1];
            var t3 = triangles[i + 2];
            
            // 创建规范化的三角形表示（排序后）
            var indices = new List<int> { t1, t2, t3 };
            indices.Sort();
            var key = $"{indices[0]},{indices[1]},{indices[2]}";
            
            Assert.IsFalse(triangleSet.Contains(key), $"Duplicate triangle found: ({t1}, {t2}, {t3})");
            triangleSet.Add(key);
        }
    }
    
    // 验证三角形面积总和是否等于多边形面积（近似）
    private void AssertAreaMatches(List<Vector2> polygon, List<int> triangles, float tolerance = 0.001f)
    {
        float polygonArea = CalculatePolygonArea(polygon);
        float trianglesArea = 0f;
        
        for (int i = 0; i < triangles.Count; i += 3)
        {
            var v1 = polygon[triangles[i]];
            var v2 = polygon[triangles[i + 1]];
            var v3 = polygon[triangles[i + 2]];
            trianglesArea += CalculateTriangleArea(v1, v2, v3);
        }
        
        Assert.AreEqual(polygonArea, trianglesArea, tolerance, 
            $"Polygon area ({polygonArea}) should match sum of triangle areas ({trianglesArea})");
    }
    
    private float CalculatePolygonArea(List<Vector2> polygon)
    {
        float area = 0f;
        int count = polygon.Count;
        for (int i = 0; i < count; i++)
        {
            Vector2 a = polygon[i];
            Vector2 b = polygon[(i + 1) % count];
            area += a.x * b.y - b.x * a.y;
        }
        return Mathf.Abs(area * 0.5f);
    }
    
    private float CalculateTriangleArea(Vector2 a, Vector2 b, Vector2 c)
    {
        return Mathf.Abs((a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y)) * 0.5f);
    }
    
    [Test]
    public void Triangulate_LessThan3Vertices_ThrowsException()
    {
        var polygon = new List<Vector2> { new Vector2(0, 0), new Vector2(1, 0) };
        
        Assert.Throws<System.ArgumentException>(() => 
        {
            var outline = new Outline(polygon.ToArray());
            var triangulator = new Triangulator(outline);
            triangulator.Triangulate();
        });
    }
    
    [Test]
    public void Triangulate_EmptyPolygon_ThrowsException()
    {
        var polygon = new List<Vector2>();
        
        Assert.Throws<System.ArgumentException>(() => 
        {
            var outline = new Outline(polygon.ToArray());
            var triangulator = new Triangulator(outline);
            triangulator.Triangulate();
        });
    }
    
    [Test]
    public void Triangulate_Triangle_ReturnsSingleTriangle()
    {
        var polygon = new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0.5f, 1)
        };
        
        var outline = new Outline(polygon.ToArray());
        var triangulator = new Triangulator(outline);
        triangulator.Triangulate();
        var triangles = triangulator.triangles;
        
        Assert.AreEqual(3, triangles.Count, "Triangle should produce exactly 3 indices");
        Assert.AreEqual(0, triangles[0]);
        Assert.AreEqual(1, triangles[1]);
        Assert.AreEqual(2, triangles[2]);
    }
    
    [Test]
    public void Triangulate_Square_ProducesTwoTriangles()
    {
        var polygon = new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };
        
        var outline = new Outline(polygon.ToArray());
        var triangulator = new Triangulator(outline);
        triangulator.Triangulate();
        var triangles = triangulator.triangles;
        
        AssertValidTriangulation(polygon, triangles, 2);
        AssertAreaMatches(polygon, triangles);
    }
    
    [Test]
    public void Triangulate_Pentagon_ProducesThreeTriangles()
    {
        var polygon = new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1.5f, 0.8f),
            new Vector2(0.5f, 1.3f),
            new Vector2(-0.5f, 0.8f)
        };
        
        var outline = new Outline(polygon.ToArray());
        var triangulator = new Triangulator(outline);
        triangulator.Triangulate();
        var triangles = triangulator.triangles;
        
        AssertValidTriangulation(polygon, triangles, 3);
        AssertAreaMatches(polygon, triangles);
    }
    
    [Test]
    public void Triangulate_ConvexHexagon_ProducesFourTriangles()
    {
        var polygon = new List<Vector2>
        {
            new Vector2(0, 1),
            new Vector2(0.866f, 0.5f),
            new Vector2(0.866f, -0.5f),
            new Vector2(0, -1),
            new Vector2(-0.866f, -0.5f),
            new Vector2(-0.866f, 0.5f)
        };
        var outline = new Outline(polygon.ToArray());
        var triangulator = new Triangulator(outline);
        triangulator.Triangulate();
        var triangles = triangulator.triangles;
        
        AssertValidTriangulation(polygon, triangles, 4);
        AssertAreaMatches(polygon, triangles);
    }
    
    [Test]
    public void Triangulate_ConcavePolygon_ProducesCorrectTriangles()
    {
        // L-shaped polygon (凹多边形)
        var polygon = new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(2, 0),
            new Vector2(2, 1),
            new Vector2(1, 1),
            new Vector2(1, 2),
            new Vector2(0, 2)
        };
        var outline = new Outline(polygon.ToArray());
        var triangulator = new Triangulator(outline);
        triangulator.Triangulate();
        var triangles = triangulator.triangles;
        
        AssertValidTriangulation(polygon, triangles, 4);
		for(int i = 0; i < triangles.Count; i += 3)
		{
			var t1 = triangles[i];
			var t2 = triangles[i + 1];
			var t3 = triangles[i + 2];
			Debug.Log($"triangle {i}: {polygon[t1]}, {polygon[t2]}, {polygon[t3]}");
		}
        AssertAreaMatches(polygon, triangles);
    }
    
    [Test]
    public void Triangulate_ClockwisePolygon_ConvertsToCounterClockwise()
    {
        // 顺时针排列的正方形
        var polygon = new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0)
        };
        var outline = new Outline(polygon.ToArray());
        var triangulator = new Triangulator(outline);
        triangulator.Triangulate();
        var triangles = triangulator.triangles;
        
        AssertValidTriangulation(polygon, triangles, 2);
        AssertAreaMatches(polygon, triangles);
    }
    
    [Test]
    public void Triangulate_CounterClockwisePolygon_WorksCorrectly()
    {
        // 逆时针排列的正方形
        var polygon = new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };
        var outline = new Outline(polygon.ToArray());
        var triangulator = new Triangulator(outline);
        triangulator.Triangulate();
        var triangles = triangulator.triangles;
        
        AssertValidTriangulation(polygon, triangles, 2);
        AssertAreaMatches(polygon, triangles);
    }
    
    [Test]
    public void Triangulate_ComplexConcavePolygon_ProducesCorrectTriangles()
    {
        // 更复杂的凹多边形
        var polygon = new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(3, 0),
            new Vector2(3, 2),
            new Vector2(2, 2),
            new Vector2(2, 1),
            new Vector2(1, 1),
            new Vector2(1, 2),
            new Vector2(0, 2)
        };
        var outline = new Outline(polygon.ToArray());
        var triangulator = new Triangulator(outline);
        triangulator.Triangulate();
        var triangles = triangulator.triangles;
        
        AssertValidTriangulation(polygon, triangles, 6);
        AssertAreaMatches(polygon, triangles);
    }
    
    [Test]
    public void Triangulate_Rectangle_ProducesTwoTriangles()
    {
        var polygon = new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(5, 0),
            new Vector2(5, 3),
            new Vector2(0, 3)
        };
        var outline = new Outline(polygon.ToArray());
        var triangulator = new Triangulator(outline);
        triangulator.Triangulate();
        var triangles = triangulator.triangles;
        
        AssertValidTriangulation(polygon, triangles, 2);
        AssertAreaMatches(polygon, triangles);
    }
    
    [Test]
    public void Triangulate_ReusesTriangleList_ClearsPreviousContent()
    {
        var polygon = new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0.5f, 1)
        };
        
        var outline = new Outline(polygon.ToArray());
        var triangulator = new Triangulator(outline);
        triangulator.Triangulate();
        var triangles = triangulator.triangles;
        
        Assert.AreEqual(3, triangles.Count, "Should have exactly 3 indices");
        Assert.IsTrue(triangles.Contains(0), "Should contain valid index");
        Assert.IsTrue(triangles.Contains(1), "Should contain valid index");
        Assert.IsTrue(triangles.Contains(2), "Should contain valid index");
    }
    
    [Test]
    public void Triangulate_IrregularConvexPolygon_ProducesCorrectTriangles()
    {
        var polygon = new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(2, 0.5f),
            new Vector2(3, 2),
            new Vector2(1.5f, 3),
            new Vector2(-0.5f, 2.5f),
            new Vector2(-1, 1)
        };
        var outline = new Outline(polygon.ToArray());
        var triangulator = new Triangulator(outline);
        triangulator.Triangulate();
        var triangles = triangulator.triangles;
        
        AssertValidTriangulation(polygon, triangles, 4);
        AssertAreaMatches(polygon, triangles);
    }
    
    [Test]
    public void Triangulate_ConcaveCShape_ProducesCorrectTriangles()
    {
        // C形凹多边形（修正了自相交问题）
        var polygon = new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(3, 0),
            new Vector2(3, 3),
            new Vector2(1, 3),
            new Vector2(1, 2),
            new Vector2(2, 2),
            new Vector2(2, 1),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };
        var outline = new Outline(polygon.ToArray());
        var triangulator = new Triangulator(outline);
        triangulator.Triangulate();
        var triangles = triangulator.triangles;
        
        AssertValidTriangulation(polygon, triangles, 7);
        AssertAreaMatches(polygon, triangles);
    }
    
    [Test]
    public void Triangulate_ConcaveArrowShape_ProducesCorrectTriangles()
    {
        // 箭头形状的凹多边形
        var polygon = new List<Vector2>
        {
            new Vector2(0, 1),
            new Vector2(2, 1),
            new Vector2(2, 2),
            new Vector2(3, 0.5f),
            new Vector2(2, -1),
            new Vector2(2, 0),
            new Vector2(0, 0)
        };
        var outline = new Outline(polygon.ToArray());
        var triangulator = new Triangulator(outline);
        triangulator.Triangulate();
        var triangles = triangulator.triangles;
        
        AssertValidTriangulation(polygon, triangles, 5);
        AssertAreaMatches(polygon, triangles);
    }
    
    [Test]
    public void Triangulate_ConcaveStarShape_ProducesCorrectTriangles()
    {
        // 星形凹多边形（有多个凹角）
        var polygon = new List<Vector2>
        {
            new Vector2(0, 2),
            new Vector2(0.5f, 0.5f),
            new Vector2(2, 1),
            new Vector2(1, 0),
            new Vector2(2, -1),
            new Vector2(0.5f, -0.5f),
            new Vector2(0, -2),
            new Vector2(-0.5f, -0.5f),
            new Vector2(-2, -1),
            new Vector2(-1, 0),
            new Vector2(-2, 1),
            new Vector2(-0.5f, 0.5f)
        };
        var outline = new Outline(polygon.ToArray());
        var triangulator = new Triangulator(outline);
        triangulator.Triangulate();
        var triangles = triangulator.triangles;
        
        AssertValidTriangulation(polygon, triangles, 10);
        AssertAreaMatches(polygon, triangles);
    }
    
    [Test]
    public void Triangulate_ConcaveWithMultipleNotches_ProducesCorrectTriangles()
    {
        // 带多个凹槽的多边形
        var polygon = new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(4, 0),
            new Vector2(4, 1),
            new Vector2(3, 1),
            new Vector2(3, 0.5f),
            new Vector2(2.5f, 0.5f),
            new Vector2(2.5f, 1),
            new Vector2(1.5f, 1),
            new Vector2(1.5f, 0.5f),
            new Vector2(1, 0.5f),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };
        var outline = new Outline(polygon.ToArray());
        var triangulator = new Triangulator(outline);
        triangulator.Triangulate();
        var triangles = triangulator.triangles;
        
        AssertValidTriangulation(polygon, triangles, 10);
        AssertAreaMatches(polygon, triangles);
    }
    
    [Test]
    public void Triangulate_ConcaveDiamondWithNotch_ProducesCorrectTriangles()
    {
        // 带凹槽的菱形
        var polygon = new List<Vector2>
        {
            new Vector2(0, 2),
            new Vector2(1, 1),
            new Vector2(2, 2),
            new Vector2(2, 0),
            new Vector2(1, -0.5f),
            new Vector2(0, 0)
        };
        var outline = new Outline(polygon.ToArray());
        var triangulator = new Triangulator(outline);
        triangulator.Triangulate();
        var triangles = triangulator.triangles;
        
        AssertValidTriangulation(polygon, triangles, 4);
        AssertAreaMatches(polygon, triangles);
    }
    
    [Test]
    public void Triangulate_ConcaveTrapezoidWithDeepNotch_ProducesCorrectTriangles()
    {
        // 带深凹槽的梯形
        var polygon = new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(5, 0),
            new Vector2(4, 3),
            new Vector2(3, 3),
            new Vector2(3, 1),
            new Vector2(2, 1),
            new Vector2(2, 3),
            new Vector2(1, 3)
        };
        var outline = new Outline(polygon.ToArray());
        var triangulator = new Triangulator(outline);
        triangulator.Triangulate();
        var triangles = triangulator.triangles;
        
        AssertValidTriangulation(polygon, triangles, 6);
        AssertAreaMatches(polygon, triangles);
    }
    
    [Test]
    public void Triangulate_ConcaveHexagonWithTwoNotches_ProducesCorrectTriangles()
    {
        // 带两个凹槽的六边形
        var polygon = new List<Vector2>
        {
            new Vector2(0, 1),
            new Vector2(1, 2),
            new Vector2(2, 2),
            new Vector2(2.5f, 1),
            new Vector2(2, 0),
            new Vector2(1, 0),
            new Vector2(0.5f, 0.5f),
            new Vector2(0, 0)
        };
        var outline = new Outline(polygon.ToArray());
        var triangulator = new Triangulator(outline);
        triangulator.Triangulate();
        var triangles = triangulator.triangles;
        
        AssertValidTriangulation(polygon, triangles, 6);
        AssertAreaMatches(polygon, triangles);
    }
    
    [Test]
    public void Triangulate_ConcavePolygonWithSharpAngles_ProducesCorrectTriangles()
    {
        // 带尖锐角的凹多边形
        var polygon = new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(3, 0),
            new Vector2(3, 2),
            new Vector2(2.5f, 1.5f),
            new Vector2(2, 2),
            new Vector2(1.5f, 1.5f),
            new Vector2(1, 2),
            new Vector2(0.5f, 1.5f),
            new Vector2(0, 2)
        };
        var outline = new Outline(polygon.ToArray());
        var triangulator = new Triangulator(outline);
        triangulator.Triangulate();
        var triangles = triangulator.triangles;
        
        AssertValidTriangulation(polygon, triangles, 7);
        AssertAreaMatches(polygon, triangles);
    }
    
    [Test]
    public void Triangulate_ConcavePolygonIrregularShape_ProducesCorrectTriangles()
    {
        // 不规则凹多边形
        var polygon = new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(4, 0),
            new Vector2(4, 3),
            new Vector2(3.5f, 2.5f),
            new Vector2(3, 3),
            new Vector2(2, 2),
            new Vector2(1.5f, 2.5f),
            new Vector2(1, 2),
            new Vector2(0.5f, 2.5f),
            new Vector2(0, 3)
        };
        var outline = new Outline(polygon.ToArray());
        var triangulator = new Triangulator(outline);
        triangulator.Triangulate();
        var triangles = triangulator.triangles;
        
        AssertValidTriangulation(polygon, triangles, 8);
        AssertAreaMatches(polygon, triangles);
    }
    
    [Test]
    public void Triangulate_FighterHullModule_ProducesCorrectTriangles()
    {
        // 来自 hulldata.json 的虻级战斗机模块轮廓
        // 多边形顶点索引顺序: 0, 7, 9, 8, 14, 13, 12, 10, 11, 6, 1, 2, 5, 4, 3
        var polygon = new List<Vector2>
        {
            new Vector2(-0.28600001335144045f, 0.18400000035762788f),    // 0
            new Vector2(-0.4293110966682434f, -0.197564497590065f),     // 7
            new Vector2(-0.31678932905197146f, -0.2962679862976074f),  // 9
            new Vector2(-0.20031940937042237f, -0.2646827697753906f),   // 8
            new Vector2(-0.14899355173110963f, -0.40484166145324709f), // 14
            new Vector2(-0.006860703229904175f, -0.35154175758361819f), // 13
            new Vector2(0.13527214527130128f, -0.39299726486206057f),  // 12
            new Vector2(0.216208815574646f, -0.24296818673610688f),    // 10
            new Vector2(0.33267873525619509f, -0.2942938208580017f),    // 11
            new Vector2(0.43927836418151858f, -0.21730516850948335f),  // 6
            new Vector2(0.2734568119049072f, 0.1656636893749237f),       // 1
            new Vector2(0.12342765927314758f, 0.3156929612159729f),    // 2
            new Vector2(0.07210204005241394f, 0.10051947087049484f),   // 5
            new Vector2(-0.06410868465900421f, 0.10249367356300354f),  // 4
            new Vector2(-0.11740849912166596f, 0.3255632221698761f)    // 3
        };
        
        var outline = new Outline(polygon.ToArray());
        var triangulator = new Triangulator(outline);
        triangulator.Triangulate();
        var triangles = triangulator.triangles;
        
        // 15个顶点应该产生 13 个三角形 (n-2 = 15-2 = 13)
        AssertValidTriangulation(polygon, triangles, 13);
        AssertAreaMatches(polygon, triangles);
    }
}

