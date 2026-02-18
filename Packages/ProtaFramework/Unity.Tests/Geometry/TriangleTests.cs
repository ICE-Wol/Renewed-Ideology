using System;
using UnityEngine;
using NUnit.Framework;
using Prota.Unity;

namespace Geometry
{
    public class TriangleTests
    {
        private const float Tolerance = 1e-6f;
        
        #region 基本属性测试
        
        [Test]
        public void SignedArea_CounterClockwiseTriangle_ShouldReturnPositive()
        {
            // Arrange - 逆时针三角形 (0,0) -> (1,0) -> (0,1)
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var area = triangle.signedArea;
            
            // Assert
            Assert.Greater(area, 0f);
            Assert.AreEqual(0.5f, area, Tolerance);
        }
        
        [Test]
        public void SignedArea_ClockwiseTriangle_ShouldReturnNegative()
        {
            // Arrange - 顺时针三角形 (0,0) -> (0,1) -> (1,0)
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0));
            
            // Act
            var area = triangle.signedArea;
            
            // Assert
            Assert.Less(area, 0f);
            Assert.AreEqual(-0.5f, area, Tolerance);
        }
        
        [Test]
        public void Area_ShouldReturnAbsoluteValue()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            var triangle2 = new Triangle(new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0));
            
            // Act
            var area1 = triangle1.area;
            var area2 = triangle2.area;
            
            // Assert
            Assert.AreEqual(0.5f, area1, Tolerance);
            Assert.AreEqual(0.5f, area2, Tolerance);
            Assert.AreEqual(area1, area2, Tolerance);
        }
        
        [Test]
        public void Area_LargeTriangle_ShouldReturnCorrectArea()
        {
            // Arrange - 底边4，高3的三角形
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(4, 0), new Vector2(2, 3));
            
            // Act
            var area = triangle.area;
            
            // Assert
            Assert.AreEqual(6f, area, Tolerance);
        }
        
        [Test]
        public void IsCounterClockwise_CounterClockwiseTriangle_ShouldReturnTrue()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var result = triangle.isCounterClockwise;
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsCounterClockwise_ClockwiseTriangle_ShouldReturnFalse()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0));
            
            // Act
            var result = triangle.isCounterClockwise;
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsClockwise_ClockwiseTriangle_ShouldReturnTrue()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0));
            
            // Act
            var result = triangle.isClockwise;
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsClockwise_CounterClockwiseTriangle_ShouldReturnFalse()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var result = triangle.isClockwise;
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void Center_ShouldReturnCentroid()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(3, 0), new Vector2(0, 3));
            
            // Act
            var center = triangle.center;
            
            // Assert
            Assert.AreEqual(new Vector2(1, 1), center);
        }
        
        [Test]
        public void Edges_ShouldReturnThreeSegments()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var (ab, bc, ca) = triangle.Edges;
            
            // Assert
            Assert.AreEqual(new Segment(new Vector2(0, 0), new Vector2(1, 0)), ab);
            Assert.AreEqual(new Segment(new Vector2(1, 0), new Vector2(0, 1)), bc);
            Assert.AreEqual(new Segment(new Vector2(0, 1), new Vector2(0, 0)), ca);
        }
        
        #endregion
        
        #region 点是否在三角形内测试（包括边界）
        
        [Test]
        public void ContainsPoint_PointInside_ShouldReturnTrue()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            var point = new Vector2(1, 0.5f);
            
            // Act
            var result = triangle.ContainsPoint(point);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void ContainsPoint_PointAtVertex_ShouldReturnTrue()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            var point = new Vector2(0, 0);
            
            // Act
            var result = triangle.ContainsPoint(point);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void ContainsPoint_PointOnEdge_ShouldReturnTrue()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            var point = new Vector2(1, 0);
            
            // Act
            var result = triangle.ContainsPoint(point);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void ContainsPoint_PointOutside_ShouldReturnFalse()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            var point = new Vector2(3, 3);
            
            // Act
            var result = triangle.ContainsPoint(point);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void ContainsPoint_PointAtCenter_ShouldReturnTrue()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            var center = triangle.center;
            
            // Act
            var result = triangle.ContainsPoint(center);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        #endregion
        
        #region 点是否在三角形内测试（不包括边界）
        
        [Test]
        public void ContainsPointExclusive_PointInside_ShouldReturnTrue()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            var point = new Vector2(1, 0.5f);
            
            // Act
            var result = triangle.ContainsPointExclusive(point);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void ContainsPointExclusive_PointAtVertex_ShouldReturnFalse()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            var point = new Vector2(0, 0);
            
            // Act
            var result = triangle.ContainsPointExclusive(point);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void ContainsPointExclusive_PointOnEdge_ShouldReturnFalse()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            var point = new Vector2(1, 0);
            
            // Act
            var result = triangle.ContainsPointExclusive(point);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void ContainsPointExclusive_PointOutside_ShouldReturnFalse()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            var point = new Vector2(3, 3);
            
            // Act
            var result = triangle.ContainsPointExclusive(point);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        #endregion
        
        #region 相等性测试
        
        [Test]
        public void Equals_SameVerticesSameOrder_ShouldReturnTrue()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            var triangle2 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var result = triangle1.Equals(triangle2);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void Equals_SameVerticesDifferentOrder_ShouldReturnFalse()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            var triangle2 = new Triangle(new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 0));
            
            // Act
            var result = triangle1.Equals(triangle2);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void Equals_DifferentVertices_ShouldReturnFalse()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            var triangle2 = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(0, 1));
            
            // Act
            var result = triangle1.Equals(triangle2);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void OperatorEquals_SameTriangles_ShouldReturnTrue()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            var triangle2 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act & Assert
            Assert.IsTrue(triangle1 == triangle2);
            Assert.IsFalse(triangle1 != triangle2);
        }
        
        [Test]
        public void OperatorNotEquals_DifferentTriangles_ShouldReturnTrue()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            var triangle2 = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(0, 1));
            
            // Act & Assert
            Assert.IsTrue(triangle1 != triangle2);
            Assert.IsFalse(triangle1 == triangle2);
        }
        
        [Test]
        public void OperatorNotEquals_DifferentOrder_ShouldReturnTrue()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            var triangle2 = new Triangle(new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 0));
            
            // Act & Assert - 不同顺序的三角形应该不相等
            Assert.IsTrue(triangle1 != triangle2);
            Assert.IsFalse(triangle1 == triangle2);
        }
        
        [Test]
        public void GetHashCode_SameTriangles_ShouldReturnSameHash()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            var triangle2 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var hash1 = triangle1.GetHashCode();
            var hash2 = triangle2.GetHashCode();
            
            // Assert
            Assert.AreEqual(hash1, hash2);
        }
        
        [Test]
        public void GetHashCode_DifferentOrder_ShouldReturnDifferentHash()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            var triangle2 = new Triangle(new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 0));
            
            // Act
            var hash1 = triangle1.GetHashCode();
            var hash2 = triangle2.GetHashCode();
            
            // Assert - 不同顺序的三角形应该有不同的哈希值
            Assert.AreNotEqual(hash1, hash2);
        }
        
        #endregion
        
        #region SameTriangle 测试
        
        [Test]
        public void SameTriangle_SameVerticesSameOrder_ShouldReturnTrue()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            var triangle2 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var result = triangle1.SameTriangle(triangle2);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void SameTriangle_SameVerticesDifferentOrder_ShouldReturnTrue()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            var triangle2 = new Triangle(new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 0));
            
            // Act
            var result = triangle1.SameTriangle(triangle2);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void SameTriangle_DifferentVertices_ShouldReturnFalse()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            var triangle2 = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(0, 1));
            
            // Act
            var result = triangle1.SameTriangle(triangle2);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void SameTriangle_AllPermutations_ShouldReturnTrue()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            var permutations = new[]
            {
                new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1)),
                new Triangle(new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0)),
                new Triangle(new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 1)),
                new Triangle(new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 0)),
                new Triangle(new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0)),
                new Triangle(new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 0))
            };
            
            // Act & Assert
            foreach (var perm in permutations)
            {
                Assert.IsTrue(triangle1.SameTriangle(perm), $"Triangle {perm.a}, {perm.b}, {perm.c} should be same as original");
            }
        }
        
        [Test]
        public void SameTriangle_WithRotatedTriangle_ShouldReturnTrue()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var rotated1 = triangle.rotatedForward;
            var rotated2 = triangle.rotatedBackward;
            
            // Assert
            Assert.IsTrue(triangle.SameTriangle(rotated1));
            Assert.IsTrue(triangle.SameTriangle(rotated2));
        }
        
        [Test]
        public void SameTriangle_WithReversedTriangle_ShouldReturnTrue()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var reversed = triangle.reversedTriangle;
            
            // Assert
            Assert.IsTrue(triangle.SameTriangle(reversed));
        }
        
        #endregion
        
        #region CompareTo 测试
        
        [Test]
        public void CompareTo_SameTriangles_ShouldReturnZero()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            var triangle2 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var result = triangle1.CompareTo(triangle2);
            
            // Assert
            Assert.AreEqual(0, result);
        }
        
        [Test]
        public void CompareTo_LargerArea_ShouldReturnPositive()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            var triangle2 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var result = triangle1.CompareTo(triangle2);
            
            // Assert
            Assert.Greater(result, 0);
        }
        
        [Test]
        public void CompareTo_SmallerArea_ShouldReturnNegative()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            var triangle2 = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            
            // Act
            var result = triangle1.CompareTo(triangle2);
            
            // Assert
            Assert.Less(result, 0);
        }
        
        #endregion
        
        #region 边界情况测试
        
        [Test]
        public void DegenerateTriangle_ZeroArea_ShouldHandleCorrectly()
        {
            // Arrange - 三个点共线
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0));
            
            // Act
            var area = triangle.area;
            var signedArea = triangle.signedArea;
            
            // Assert
            Assert.AreEqual(0f, area, Tolerance);
            Assert.AreEqual(0f, signedArea, Tolerance);
        }
        
        [Test]
        public void DegenerateTriangle_ContainsPoint_ShouldThrowException()
        {
            // Arrange - 三个点共线
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0));
            var point = new Vector2(1, 0);
            
            // Act & Assert
            // 共线情况下，应该抛出异常
            Assert.Throws<InvalidOperationException>(() => triangle.ContainsPoint(point));
        }
        
        [Test]
        public void VerySmallTriangle_ShouldHandleCorrectly()
        {
            // Arrange
            var triangle = new Triangle(
                new Vector2(0, 0), 
                new Vector2(0.0001f, 0), 
                new Vector2(0, 0.0001f));
            
            // Act
            var area = triangle.area;
            var isCCW = triangle.isCounterClockwise;
            
            // Assert
            Assert.Greater(area, 0f);
            Assert.IsTrue(isCCW);
        }
        
        [Test]
        public void LargeTriangle_ShouldHandleCorrectly()
        {
            // Arrange
            var triangle = new Triangle(
                new Vector2(0, 0), 
                new Vector2(1000, 0), 
                new Vector2(500, 1000));
            
            // Act
            var area = triangle.area;
            var center = triangle.center;
            
            // Assert
            Assert.AreEqual(500000f, area, Tolerance);
            Assert.AreEqual(500f, center.x, 0.1f);
            Assert.AreEqual(333.333333f, center.y, 0.1f);
        }
        
        [Test]
        public void NegativeCoordinates_ShouldWorkCorrectly()
        {
            // Arrange
            var triangle = new Triangle(
                new Vector2(-1, -1), 
                new Vector2(1, -1), 
                new Vector2(0, 1));
            
            // Act
            var area = triangle.area;
            var center = triangle.center;
            
            // Assert
            Assert.AreEqual(2f, area, Tolerance);
            Assert.AreEqual(0f, center.x, Tolerance);
            Assert.AreEqual(-0.333333f, center.y, Tolerance);
        }
        
        #endregion
        
        #region 综合测试
        
        [Test]
        public void TriangleProperties_ConsistencyCheck()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            
            // Act
            var area = triangle.area;
            var signedArea = triangle.signedArea;
            var isCCW = triangle.isCounterClockwise;
            var isCW = triangle.isClockwise;
            var center = triangle.center;
            
            // Assert
            Assert.AreEqual(Mathf.Abs(signedArea), area, Tolerance);
            Assert.IsTrue(isCCW != isCW || Mathf.Abs(signedArea) < Tolerance);
            Assert.IsTrue(triangle.ContainsPoint(center));
        }
        
        
        #endregion
        
        #region 顶点旋转测试
        
        [Test]
        public void RotatedForward_ShouldRotateVerticesForward()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var rotated = triangle.rotatedForward;
            
            // Assert
            Assert.AreEqual(new Vector2(1, 0), rotated.a);
            Assert.AreEqual(new Vector2(0, 1), rotated.b);
            Assert.AreEqual(new Vector2(0, 0), rotated.c);
        }
        
        [Test]
        public void RotatedBackward_ShouldRotateVerticesBackward()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var rotated = triangle.rotatedBackward;
            
            // Assert
            Assert.AreEqual(new Vector2(0, 1), rotated.a);
            Assert.AreEqual(new Vector2(0, 0), rotated.b);
            Assert.AreEqual(new Vector2(1, 0), rotated.c);
        }
        
        [Test]
        public void RotatedForward_ThreeTimes_ShouldReturnOriginal()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var rotated1 = triangle.rotatedForward;
            var rotated2 = rotated1.rotatedForward;
            var rotated3 = rotated2.rotatedForward;
            
            // Assert
            Assert.AreEqual(triangle.a, rotated3.a);
            Assert.AreEqual(triangle.b, rotated3.b);
            Assert.AreEqual(triangle.c, rotated3.c);
        }
        
        [Test]
        public void RotatedBackward_ThreeTimes_ShouldReturnOriginal()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var rotated1 = triangle.rotatedBackward;
            var rotated2 = rotated1.rotatedBackward;
            var rotated3 = rotated2.rotatedBackward;
            
            // Assert
            Assert.AreEqual(triangle.a, rotated3.a);
            Assert.AreEqual(triangle.b, rotated3.b);
            Assert.AreEqual(triangle.c, rotated3.c);
        }
        
        [Test]
        public void RotatedForward_ThenRotatedBackward_ShouldReturnOriginal()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var rotated = triangle.rotatedForward.rotatedBackward;
            
            // Assert
            Assert.AreEqual(triangle.a, rotated.a);
            Assert.AreEqual(triangle.b, rotated.b);
            Assert.AreEqual(triangle.c, rotated.c);
        }
        
        [Test]
        public void RotatedBackward_ThenRotatedForward_ShouldReturnOriginal()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var rotated = triangle.rotatedBackward.rotatedForward;
            
            // Assert
            Assert.AreEqual(triangle.a, rotated.a);
            Assert.AreEqual(triangle.b, rotated.b);
            Assert.AreEqual(triangle.c, rotated.c);
        }
        
        [Test]
        public void RotatedForward_ShouldPreserveArea()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            
            // Act
            var rotated = triangle.rotatedForward;
            
            // Assert
            Assert.AreEqual(triangle.area, rotated.area, Tolerance);
        }
        
        [Test]
        public void RotatedBackward_ShouldPreserveArea()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            
            // Act
            var rotated = triangle.rotatedBackward;
            
            // Assert
            Assert.AreEqual(triangle.area, rotated.area, Tolerance);
        }
        
        [Test]
        public void RotatedForward_ShouldPreserveOrientation()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var rotated = triangle.rotatedForward;
            
            // Assert
            Assert.AreEqual(triangle.isCounterClockwise, rotated.isCounterClockwise);
            Assert.AreEqual(triangle.isClockwise, rotated.isClockwise);
        }
        
        [Test]
        public void RotatedBackward_ShouldPreserveOrientation()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var rotated = triangle.rotatedBackward;
            
            // Assert
            Assert.AreEqual(triangle.isCounterClockwise, rotated.isCounterClockwise);
            Assert.AreEqual(triangle.isClockwise, rotated.isClockwise);
        }
        
        [Test]
        public void RotatedForward_ShouldPreserveCenter()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            
            // Act
            var rotated = triangle.rotatedForward;
            
            // Assert
            Assert.AreEqual(triangle.center, rotated.center);
        }
        
        [Test]
        public void RotatedBackward_ShouldPreserveCenter()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            
            // Act
            var rotated = triangle.rotatedBackward;
            
            // Assert
            Assert.AreEqual(triangle.center, rotated.center);
        }
        
        [Test]
        public void RotatedForward_ShouldBeSameTriangleAfterRotation()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var rotated = triangle.rotatedForward;
            
            // Assert - 旋转后的三角形应该与原三角形是同一个三角形（顶点相同但顺序不同）
            Assert.IsTrue(triangle.SameTriangle(rotated));
            Assert.IsFalse(triangle.Equals(rotated)); // 但顺序不同，所以 Equals 应该返回 false
        }
        
        [Test]
        public void RotatedBackward_ShouldBeSameTriangleAfterRotation()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var rotated = triangle.rotatedBackward;
            
            // Assert - 旋转后的三角形应该与原三角形是同一个三角形（顶点相同但顺序不同）
            Assert.IsTrue(triangle.SameTriangle(rotated));
            Assert.IsFalse(triangle.Equals(rotated)); // 但顺序不同，所以 Equals 应该返回 false
        }
        
        [Test]
        public void RotatedForward_WithDifferentTriangles_ShouldWorkCorrectly()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(3, 0), new Vector2(1.5f, 3));
            var triangle2 = new Triangle(new Vector2(10, 10), new Vector2(20, 10), new Vector2(15, 20));
            
            // Act
            var rotated1 = triangle1.rotatedForward;
            var rotated2 = triangle2.rotatedForward;
            
            // Assert
            Assert.AreEqual(new Vector2(3, 0), rotated1.a);
            Assert.AreEqual(new Vector2(1.5f, 3), rotated1.b);
            Assert.AreEqual(new Vector2(0, 0), rotated1.c);
            
            Assert.AreEqual(new Vector2(20, 10), rotated2.a);
            Assert.AreEqual(new Vector2(15, 20), rotated2.b);
            Assert.AreEqual(new Vector2(10, 10), rotated2.c);
        }
        
        [Test]
        public void RotatedBackward_WithDifferentTriangles_ShouldWorkCorrectly()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(3, 0), new Vector2(1.5f, 3));
            var triangle2 = new Triangle(new Vector2(10, 10), new Vector2(20, 10), new Vector2(15, 20));
            
            // Act
            var rotated1 = triangle1.rotatedBackward;
            var rotated2 = triangle2.rotatedBackward;
            
            // Assert
            Assert.AreEqual(new Vector2(1.5f, 3), rotated1.a);
            Assert.AreEqual(new Vector2(0, 0), rotated1.b);
            Assert.AreEqual(new Vector2(3, 0), rotated1.c);
            
            Assert.AreEqual(new Vector2(15, 20), rotated2.a);
            Assert.AreEqual(new Vector2(10, 10), rotated2.b);
            Assert.AreEqual(new Vector2(20, 10), rotated2.c);
        }
        
        [Test]
        public void ReversedTriangle_ShouldReverseVertexOrder()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var reversed = triangle.reversedTriangle;
            
            // Assert
            Assert.AreEqual(new Vector2(0, 0), reversed.a);
            Assert.AreEqual(new Vector2(0, 1), reversed.b);
            Assert.AreEqual(new Vector2(1, 0), reversed.c);
        }
        
        [Test]
        public void ReversedTriangle_Twice_ShouldReturnOriginal()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var reversed = triangle.reversedTriangle.reversedTriangle;
            
            // Assert
            Assert.AreEqual(triangle.a, reversed.a);
            Assert.AreEqual(triangle.b, reversed.b);
            Assert.AreEqual(triangle.c, reversed.c);
        }
        
        [Test]
        public void ReversedTriangle_ShouldReverseOrientation()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            bool originalIsCCW = triangle.isCounterClockwise;
            
            // Act
            var reversed = triangle.reversedTriangle;
            
            // Assert
            Assert.AreNotEqual(originalIsCCW, reversed.isCounterClockwise);
            if (originalIsCCW)
            {
                Assert.IsTrue(reversed.isClockwise);
            }
            else
            {
                Assert.IsTrue(reversed.isCounterClockwise);
            }
        }
        
        [Test]
        public void ReversedTriangle_ShouldPreserveArea()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            
            // Act
            var reversed = triangle.reversedTriangle;
            
            // Assert
            Assert.AreEqual(triangle.area, reversed.area, Tolerance);
        }
        
        [Test]
        public void ReversedTriangle_ShouldPreserveCenter()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            
            // Act
            var reversed = triangle.reversedTriangle;
            
            // Assert
            Assert.AreEqual(triangle.center, reversed.center);
        }
        
        [Test]
        public void ReversedTriangle_ShouldHaveOppositeSignedArea()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var reversed = triangle.reversedTriangle;
            
            // Assert
            Assert.AreEqual(-triangle.signedArea, reversed.signedArea, Tolerance);
        }
        
        [Test]
        public void ReversedTriangle_ShouldBeSameTriangleAfterReversal()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var reversed = triangle.reversedTriangle;
            
            // Assert - 反转后的三角形应该与原三角形是同一个三角形（顶点相同但顺序不同）
            Assert.IsTrue(triangle.SameTriangle(reversed));
            Assert.IsFalse(triangle.Equals(reversed)); // 但顺序不同，所以 Equals 应该返回 false
        }
        
        [Test]
        public void ReversedTriangle_WithClockwiseTriangle_ShouldBecomeCounterClockwise()
        {
            // Arrange - 创建一个顺时针三角形
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0));
            
            // Act
            var reversed = triangle.reversedTriangle;
            
            // Assert
            Assert.IsTrue(triangle.isClockwise);
            Assert.IsTrue(reversed.isCounterClockwise);
        }
        
        [Test]
        public void ReversedTriangle_WithCounterClockwiseTriangle_ShouldBecomeClockwise()
        {
            // Arrange - 创建一个逆时针三角形
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var reversed = triangle.reversedTriangle;
            
            // Assert
            Assert.IsTrue(triangle.isCounterClockwise);
            Assert.IsTrue(reversed.isClockwise);
        }
        
        [Test]
        public void ReversedTriangle_WithDifferentTriangles_ShouldWorkCorrectly()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(3, 0), new Vector2(1.5f, 3));
            var triangle2 = new Triangle(new Vector2(10, 10), new Vector2(20, 10), new Vector2(15, 20));
            
            // Act
            var reversed1 = triangle1.reversedTriangle;
            var reversed2 = triangle2.reversedTriangle;
            
            // Assert
            Assert.AreEqual(new Vector2(0, 0), reversed1.a);
            Assert.AreEqual(new Vector2(1.5f, 3), reversed1.b);
            Assert.AreEqual(new Vector2(3, 0), reversed1.c);
            
            Assert.AreEqual(new Vector2(10, 10), reversed2.a);
            Assert.AreEqual(new Vector2(15, 20), reversed2.b);
            Assert.AreEqual(new Vector2(20, 10), reversed2.c);
        }
        
        [Test]
        public void ReversedTriangle_ThenRotatedForward_ThreeTimes_ShouldReturnReversed()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var reversed = triangle.reversedTriangle;
            var reversedThenForward3 = reversed.rotatedForward.rotatedForward.rotatedForward;
            
            // Assert - 旋转3次应该回到原状
            Assert.AreEqual(reversed.a, reversedThenForward3.a);
            Assert.AreEqual(reversed.b, reversedThenForward3.b);
            Assert.AreEqual(reversed.c, reversedThenForward3.c);
        }
        
        [Test]
        public void ReversedTriangle_ThenRotatedBackward_ThreeTimes_ShouldReturnReversed()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var reversed = triangle.reversedTriangle;
            var reversedThenBackward3 = reversed.rotatedBackward.rotatedBackward.rotatedBackward;
            
            // Assert - 旋转3次应该回到原状
            Assert.AreEqual(reversed.a, reversedThenBackward3.a);
            Assert.AreEqual(reversed.b, reversedThenBackward3.b);
            Assert.AreEqual(reversed.c, reversedThenBackward3.c);
        }
        
        #endregion
        
        #region 退化三角形测试
        
        [Test]
        public void IsDegenerateToPoint_ThreeSamePoints_ShouldReturnTrue()
        {
            // Arrange - 三个点完全相同
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
            
            // Act
            var result = triangle.isDegenerateToPoint;
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsDegenerateToPoint_ThreeVeryClosePoints_ShouldReturnTrue()
        {
            // Arrange - 三个点非常接近（在 epsilon 范围内）
            // 使用更小的偏移量，确保所有点之间的距离平方都小于 epsSq (1e-12)
            float eps = 1e-6f;
            float offset = eps * 0.3f; // 使用 0.3 而不是 0.5，确保最大距离平方 < 1e-12
            var triangle = new Triangle(
                new Vector2(0, 0), 
                new Vector2(offset, offset), 
                new Vector2(-offset, -offset));
            
            // Act
            var result = triangle.isDegenerateToPoint;
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsDegenerateToPoint_ValidTriangle_ShouldReturnFalse()
        {
            // Arrange - 正常三角形
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var result = triangle.isDegenerateToPoint;
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsDegenerateToPoint_CollinearTriangle_ShouldReturnFalse()
        {
            // Arrange - 共线三角形（但不是退化到点）
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0));
            
            // Act
            var result = triangle.isDegenerateToPoint;
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsCollinear_ThreeCollinearPoints_ShouldReturnTrue()
        {
            // Arrange - 三个点共线
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0));
            
            // Act
            var result = triangle.isCollinear;
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsCollinear_DiagonalCollinearPoints_ShouldReturnTrue()
        {
            // Arrange - 三个点在对角线上共线
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 2));
            
            // Act
            var result = triangle.isCollinear;
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsCollinear_ValidTriangle_ShouldReturnFalse()
        {
            // Arrange - 正常三角形
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            
            // Act
            var result = triangle.isCollinear;
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsCollinear_DegenerateToPoint_ShouldReturnFalse()
        {
            // Arrange - 退化到点
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
            
            // Act
            var result = triangle.isCollinear;
            
            // Assert - 退化到点不应该被认为是共线
            Assert.IsFalse(result);
        }
        
        [Test]
        public void ContainsPoint_DegenerateToPoint_ShouldThrowException()
        {
            // Arrange - 退化到点
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
            var point = new Vector2(1, 1);
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => triangle.ContainsPoint(point));
        }
        
        [Test]
        public void ContainsPoint_CollinearTriangle_ShouldThrowException()
        {
            // Arrange - 共线三角形
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0));
            var point = new Vector2(1, 1);
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => triangle.ContainsPoint(point));
        }
        
        [Test]
        public void ContainsPointExclusive_DegenerateToPoint_ShouldThrowException()
        {
            // Arrange - 退化到点
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
            var point = new Vector2(1, 1);
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => triangle.ContainsPointExclusive(point));
        }
        
        [Test]
        public void ContainsPointExclusive_CollinearTriangle_ShouldThrowException()
        {
            // Arrange - 共线三角形
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0));
            var point = new Vector2(1, 1);
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => triangle.ContainsPointExclusive(point));
        }
        
        [Test]
        public void IsDegenerateToPoint_WithVerySmallDistances_ShouldReturnTrue()
        {
            // Arrange - 距离非常小但大于0
            float tinyDist = 1e-7f; // 小于 Eps (1e-6f)
            var triangle = new Triangle(
                new Vector2(0, 0), 
                new Vector2(tinyDist, 0), 
                new Vector2(0, tinyDist));
            
            // Act
            var result = triangle.isDegenerateToPoint;
            
            // Assert - 距离平方小于 Eps^2，应该返回 true
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsCollinear_WithVerySmallArea_ShouldReturnTrue()
        {
            // Arrange - 面积非常小但三个点不重合
            // 使用 1e-12 的偏差，面积约为 5e-13，小于 OrientationEps (1e-12)
            var triangle = new Triangle(
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(2, 1e-12f)); // 几乎共线但略有偏差
            
            // Act
            var result = triangle.isCollinear;
            
            // Assert - 面积小于 OrientationEps，应该返回 true
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsCollinear_WithLargerArea_ShouldReturnFalse()
        {
            // Arrange - 面积虽然小但大于阈值
            var triangle = new Triangle(
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(0.5f, 1e-5f)); // 面积约为 5e-6，大于 OrientationEps (1e-12)
            
            // Act
            var result = triangle.isCollinear;
            
            // Assert
            Assert.IsFalse(result);
        }
        
        #endregion
    }
}

