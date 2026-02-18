using System;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using Prota.Unity;

namespace Geometry
{
    public class GeometryTriangulatorTests
    {
        private const float Tolerance = 1e-6f;
        
        #region 基本三角剖分测试
        
        [Test]
        public void Triangulate_Triangle_ShouldReturnOneTriangle()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(0.5f, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var triangles = GeometryTriangulator.Triangulate(outline);
            
            // Assert
            Assert.AreEqual(1, triangles.Count);
            var triangle = triangles[0];
            Assert.IsTrue(triangle.ContainsPoint(new Vector2(0, 0), Tolerance));
            Assert.IsTrue(triangle.ContainsPoint(new Vector2(1, 0), Tolerance));
            Assert.IsTrue(triangle.ContainsPoint(new Vector2(0.5f, 1), Tolerance));
        }
        
        [Test]
        public void Triangulate_Square_ShouldReturnTwoTriangles()
        {
            // Arrange - 逆时针正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var triangles = GeometryTriangulator.Triangulate(outline);
            
            // Assert
            Assert.AreEqual(2, triangles.Count);
            
            // 验证所有三角形覆盖整个正方形
            float totalArea = 0f;
            foreach (var triangle in triangles)
            {
                totalArea += triangle.area;
            }
            Assert.AreEqual(1f, totalArea, Tolerance);
        }
        
        [Test]
        public void Triangulate_Pentagon_ShouldReturnThreeTriangles()
        {
            // Arrange - 五边形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1.5f, 0.8f), 
                new Vector2(0.5f, 1.5f), 
                new Vector2(-0.5f, 0.8f) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var triangles = GeometryTriangulator.Triangulate(outline);
            
            // Assert
            Assert.AreEqual(3, triangles.Count);
            
            // 验证所有三角形覆盖整个五边形
            float outlineArea = outline.area;
            float totalArea = 0f;
            foreach (var triangle in triangles)
            {
                totalArea += triangle.area;
            }
            Assert.AreEqual(outlineArea, totalArea, Tolerance);
        }
        
        [Test]
        public void Triangulate_ClockwiseSquare_ShouldWorkCorrectly()
        {
            // Arrange - 顺时针正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(0, 1), 
                new Vector2(1, 1), 
                new Vector2(1, 0) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var triangles = GeometryTriangulator.Triangulate(outline);
            
            // Assert
            Assert.AreEqual(2, triangles.Count);
            
            float outlineArea = outline.area;
            float totalArea = 0f;
            foreach (var triangle in triangles)
            {
                totalArea += triangle.area;
            }
            Assert.AreEqual(outlineArea, totalArea, Tolerance);
        }
        
        #endregion
        
        #region 凹多边形测试
        
        [Test]
        public void Triangulate_ConcavePolygon_ShouldWorkCorrectly()
        {
            // Arrange - L形多边形（凹多边形）
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(3, 0), 
                new Vector2(3, 1), 
                new Vector2(1, 1), 
                new Vector2(1, 3), 
                new Vector2(0, 3) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var triangles = GeometryTriangulator.Triangulate(outline);
            
            // Assert
            Assert.AreEqual(4, triangles.Count); // 6个顶点应该产生4个三角形
            
            float outlineArea = outline.area;
            float totalArea = 0f;
            foreach (var triangle in triangles)
            {
                totalArea += triangle.area;
            }
            Assert.AreEqual(outlineArea, totalArea, Tolerance);
        }
        
        [Test]
        public void Triangulate_StarShape_ShouldWorkCorrectly()
        {
            // Arrange - 星形（凹多边形）
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 2),
                new Vector2(0.5f, 0.5f),
                new Vector2(2, 0),
                new Vector2(0.5f, -0.5f),
                new Vector2(0, -2),
                new Vector2(-0.5f, -0.5f),
                new Vector2(-2, 0),
                new Vector2(-0.5f, 0.5f)
            };
            var outline = new Outline(vertices);
            
            // Act
            var triangles = GeometryTriangulator.Triangulate(outline);
            
            // Assert
            Assert.AreEqual(6, triangles.Count); // 8个顶点应该产生6个三角形
            
            float outlineArea = outline.area;
            float totalArea = 0f;
            foreach (var triangle in triangles)
            {
                totalArea += triangle.area;
            }
            Assert.AreEqual(outlineArea, totalArea, Tolerance);
        }
        
        #endregion
        
        #region 错误情况测试
        
        [Test]
        public void Triangulate_LessThanThreeVertices_ShouldThrowException()
        {
            // Arrange
            var vertices = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0) };
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
            {
                var outline = new Outline(vertices);
                GeometryTriangulator.Triangulate(outline);
            });
        }
        
        [Test]
        public void Triangulate_DegenerateToPoint_ShouldThrowException()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(0, 0), 
                new Vector2(0, 0) 
            };
            var outline = new Outline(vertices);
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                GeometryTriangulator.Triangulate(outline));
        }
        
        [Test]
        public void Triangulate_Collinear_ShouldThrowException()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(2, 0),
                new Vector2(3, 0)
            };
            var outline = new Outline(vertices);
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                GeometryTriangulator.Triangulate(outline));
        }
        
        #endregion
        
        #region 验证三角形覆盖
        
        [Test]
        public void Triangulate_TrianglesShouldCoverOutline()
        {
            // Arrange - 复杂多边形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 0), 
                new Vector2(3, 1), 
                new Vector2(2, 2), 
                new Vector2(0, 2), 
                new Vector2(-1, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var triangles = GeometryTriangulator.Triangulate(outline);
            
            // Assert
            float outlineArea = outline.area;
            float totalArea = 0f;
            foreach (var triangle in triangles)
            {
                totalArea += triangle.area;
            }
            Assert.AreEqual(outlineArea, totalArea, Tolerance);
        }
        
        [Test]
        public void Triangulate_TrianglesShouldNotOverlap()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 0), 
                new Vector2(2, 2), 
                new Vector2(0, 2) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var triangles = GeometryTriangulator.Triangulate(outline);
            
            // Assert - 检查三角形是否重叠（通过检查总面积是否等于多边形面积）
            float outlineArea = outline.area;
            float totalArea = 0f;
            foreach (var triangle in triangles)
            {
                totalArea += triangle.area;
            }
            Assert.AreEqual(outlineArea, totalArea, Tolerance);
        }
        
        [Test]
        public void Triangulate_AllTrianglesShouldBeCounterClockwise()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 0), 
                new Vector2(2, 2), 
                new Vector2(0, 2) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var triangles = GeometryTriangulator.Triangulate(outline);
            
            // Assert
            foreach (var triangle in triangles)
            {
                Assert.IsTrue(triangle.isCounterClockwise, "所有三角形应该是逆时针的");
            }
        }
        
        #endregion
        
        #region TriangulateToArray 测试
        
        [Test]
        public void TriangulateToArray_ShouldReturnArray()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var triangles = GeometryTriangulator.TriangulateToArray(outline);
            
            // Assert
            Assert.IsNotNull(triangles);
            Assert.AreEqual(2, triangles.Length);
        }
        
        #endregion
        
        #region 复杂形状测试
        
        [Test]
        public void Triangulate_ComplexPolygon_ShouldWorkCorrectly()
        {
            // Arrange - 复杂多边形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(4, 0), 
                new Vector2(4, 2), 
                new Vector2(3, 2), 
                new Vector2(3, 1), 
                new Vector2(1, 1), 
                new Vector2(1, 2), 
                new Vector2(0, 2) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var triangles = GeometryTriangulator.Triangulate(outline);
            
            // Assert
            Assert.AreEqual(6, triangles.Count); // 8个顶点应该产生6个三角形
            
            float outlineArea = outline.area;
            float totalArea = 0f;
            foreach (var triangle in triangles)
            {
                totalArea += triangle.area;
            }
            Assert.AreEqual(outlineArea, totalArea, Tolerance);
        }
        
        #endregion
    }
}

