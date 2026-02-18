using System;
using UnityEngine;
using NUnit.Framework;
using Prota;
using Prota.Unity;

namespace Geometry
{
    public class PolarCoordTests
    {
        private const float Tolerance = 1e-6f;
        
        #region 构造函数测试
        
        [Test]
        public void Constructor_WithDistanceAndAngle_ShouldSetCorrectValues()
        {
            // Arrange & Act
            var polar = new PolarCoord(5f, 45f);
            
            // Assert
            Assert.AreEqual(5f, polar.r, Tolerance);
            Assert.AreEqual(45f, polar.a, Tolerance);
        }
        
        [Test]
        public void Constructor_WithCenterAndPoint_ShouldCalculateCorrectly()
        {
            // Arrange
            var center = new Vector2(0, 0);
            var point = new Vector2(3, 4);
            
            // Act
            var polar = new PolarCoord(center, point);
            
            // Assert
            Assert.AreEqual(5f, polar.r, Tolerance); // 距离应该是5
            Assert.AreEqual(53.130102f, polar.a, 0.1f); // 角度大约是53.13度
        }
        
        [Test]
        public void Constructor_WithCenterAndPoint_AtOrigin_ShouldHaveZeroDistance()
        {
            // Arrange
            var center = new Vector2(1, 1);
            var point = new Vector2(1, 1);
            
            // Act
            var polar = new PolarCoord(center, point);
            
            // Assert
            Assert.AreEqual(0f, polar.r, Tolerance);
        }
        
        #endregion
        
        #region ToVec 测试
        
        [Test]
        public void ToVec_ShouldReturnCorrectVector()
        {
            // Arrange
            var polar = new PolarCoord(5f, 0f); // 0度，距离5
            
            // Act
            var vec = polar.ToVec();
            
            // Assert
            Assert.AreEqual(5f, vec.x, Tolerance);
            Assert.AreEqual(0f, vec.y, Tolerance);
        }
        
        [Test]
        public void ToVec_At90Degrees_ShouldReturnUpVector()
        {
            // Arrange
            var polar = new PolarCoord(5f, 90f);
            
            // Act
            var vec = polar.ToVec();
            
            // Assert
            Assert.AreEqual(0f, vec.x, Tolerance);
            Assert.AreEqual(5f, vec.y, Tolerance);
        }
        
        [Test]
        public void ToVec_WithCenter_ShouldReturnCorrectPosition()
        {
            // Arrange
            var center = new Vector2(10, 20);
            var polar = new PolarCoord(5f, 0f);
            
            // Act
            var vec = polar.ToVec(center);
            
            // Assert
            Assert.AreEqual(15f, vec.x, Tolerance);
            Assert.AreEqual(20f, vec.y, Tolerance);
        }
        
        [Test]
        public void ToVec_At45Degrees_ShouldReturnCorrectVector()
        {
            // Arrange
            var polar = new PolarCoord(Mathf.Sqrt(2), 45f);
            
            // Act
            var vec = polar.ToVec();
            
            // Assert
            Assert.AreEqual(1f, vec.x, Tolerance);
            Assert.AreEqual(1f, vec.y, Tolerance);
        }
        
        #endregion
        
        #region 运算符测试
        
        [Test]
        public void OperatorAdd_ShouldAddDistanceAndAngle()
        {
            // Arrange
            var p1 = new PolarCoord(5f, 30f);
            var p2 = new PolarCoord(3f, 20f);
            
            // Act
            var result = p1 + p2;
            
            // Assert
            Assert.AreEqual(8f, result.r, Tolerance);
            Assert.AreEqual(50f, result.a, Tolerance);
        }
        
        [Test]
        public void OperatorSubtract_ShouldSubtractDistanceAndAngle()
        {
            // Arrange
            var p1 = new PolarCoord(5f, 30f);
            var p2 = new PolarCoord(3f, 20f);
            
            // Act
            var result = p1 - p2;
            
            // Assert
            Assert.AreEqual(2f, result.r, Tolerance);
            Assert.AreEqual(10f, result.a, Tolerance);
        }
        
        [Test]
        public void OperatorEquality_WithSameValues_ShouldReturnTrue()
        {
            // Arrange
            var p1 = new PolarCoord(5f, 30f);
            var p2 = new PolarCoord(5f, 30f);
            
            // Act & Assert
            Assert.IsTrue(p1 == p2);
            Assert.IsTrue(p1.Equals(p2));
        }
        
        [Test]
        public void OperatorEquality_WithDifferentValues_ShouldReturnFalse()
        {
            // Arrange
            var p1 = new PolarCoord(5f, 30f);
            var p2 = new PolarCoord(5f, 31f);
            var p3 = new PolarCoord(6f, 30f);
            
            // Act & Assert
            Assert.IsFalse(p1 == p2);
            Assert.IsFalse(p1 == p3);
        }
        
        [Test]
        public void OperatorInequality_WithDifferentValues_ShouldReturnTrue()
        {
            // Arrange
            var p1 = new PolarCoord(5f, 30f);
            var p2 = new PolarCoord(5f, 31f);
            
            // Act & Assert
            Assert.IsTrue(p1 != p2);
        }
        
        #endregion
        
        #region Scale 测试
        
        [Test]
        public void Scale_ShouldScaleDistanceOnly()
        {
            // Arrange
            var polar = new PolarCoord(5f, 30f);
            
            // Act
            var scaled = polar.Scale(2f);
            
            // Assert
            Assert.AreEqual(10f, scaled.r, Tolerance);
            Assert.AreEqual(30f, scaled.a, Tolerance);
        }
        
        [Test]
        public void Scale_WithZero_ShouldReturnZeroDistance()
        {
            // Arrange
            var polar = new PolarCoord(5f, 30f);
            
            // Act
            var scaled = polar.Scale(0f);
            
            // Assert
            Assert.AreEqual(0f, scaled.r, Tolerance);
            Assert.AreEqual(30f, scaled.a, Tolerance);
        }
        
        [Test]
        public void Scale_WithNegative_ShouldReturnNegativeDistance()
        {
            // Arrange
            var polar = new PolarCoord(5f, 30f);
            
            // Act
            var scaled = polar.Scale(-1f);
            
            // Assert
            Assert.AreEqual(-5f, scaled.r, Tolerance);
            Assert.AreEqual(30f, scaled.a, Tolerance);
        }
        
        #endregion
        
        #region Rotate 测试
        
        [Test]
        public void Rotate_WithAngle_ShouldAddAngle()
        {
            // Arrange
            var polar = new PolarCoord(5f, 30f);
            
            // Act
            var rotated = polar.Rotate(20f);
            
            // Assert
            Assert.AreEqual(5f, rotated.r, Tolerance);
            Assert.AreEqual(50f, rotated.a, Tolerance);
        }
        
        [Test]
        public void Rotate_WithAngleStruct_ShouldAddAngle()
        {
            // Arrange
            var polar = new PolarCoord(5f, 30f);
            var angle = 20f;
            
            // Act
            var rotated = polar.Rotate(angle);
            
            // Assert
            Assert.AreEqual(5f, rotated.r, Tolerance);
            Assert.AreEqual(50f, rotated.a, Tolerance);
        }
        
        [Test]
        public void Rotate_WithNegativeAngle_ShouldSubtractAngle()
        {
            // Arrange
            var polar = new PolarCoord(5f, 30f);
            
            // Act
            var rotated = polar.Rotate(-20f);
            
            // Assert
            Assert.AreEqual(5f, rotated.r, Tolerance);
            Assert.AreEqual(10f, rotated.a, Tolerance);
        }
        
        [Test]
        public void Rotate_360Degrees_ShouldReturnSameAngle()
        {
            // Arrange
            var polar = new PolarCoord(5f, 30f);
            
            // Act
            var rotated = polar.Rotate(360f);
            
            // Assert
            Assert.AreEqual(5f, rotated.r, Tolerance);
            Assert.AreEqual(390f, rotated.a, Tolerance); // 角度值会累加
        }
        
        #endregion
        
        #region Equals 和 GetHashCode 测试
        
        [Test]
        public void Equals_WithApproximatelyEqualDistance_ShouldReturnTrue()
        {
            // Arrange
            var p1 = new PolarCoord(5f, 30f);
            var p2 = new PolarCoord(5.0000001f, 30f); // 非常接近
            
            // Act & Assert
            Assert.IsTrue(p1.Equals(p2));
        }
        
        [Test]
        public void Equals_WithDifferentDistance_ShouldReturnFalse()
        {
            // Arrange
            var p1 = new PolarCoord(5f, 30f);
            var p2 = new PolarCoord(5.001f, 30f); // 超出容差
            
            // Act & Assert
            Assert.IsFalse(p1.Equals(p2));
        }
        
        [Test]
        public void GetHashCode_WithSameValues_ShouldReturnSameHash()
        {
            // Arrange
            var p1 = new PolarCoord(5f, 30f);
            var p2 = new PolarCoord(5f, 30f);
            
            // Act & Assert
            Assert.AreEqual(p1.GetHashCode(), p2.GetHashCode());
        }
        
        #endregion
        
        #region ToString 测试
        
        [Test]
        public void ToString_ShouldReturnFormattedString()
        {
            // Arrange
            var polar = new PolarCoord(5f, 30f);
            
            // Act
            var str = polar.ToString();
            
            // Assert
            Assert.IsTrue(str.Contains("PolarCoord"));
            Assert.IsTrue(str.Contains("5"));
            Assert.IsTrue(str.Contains("30"));
        }
        
        #endregion
        
        #region 边界情况测试
        
        [Test]
        public void ZeroDistance_ShouldWorkCorrectly()
        {
            // Arrange
            var polar = new PolarCoord(0f, 30f);
            
            // Act
            var vec = polar.ToVec();
            
            // Assert
            Assert.AreEqual(Vector2.zero, vec);
        }
        
        [Test]
        public void NegativeDistance_ShouldWorkCorrectly()
        {
            // Arrange
            var polar = new PolarCoord(-5f, 30f);
            
            // Act
            var vec = polar.ToVec();
            
            // Assert
            // 负距离应该指向相反方向
            var expected = new PolarCoord(5f, 210f).ToVec(); // 180度相反
            Assert.AreEqual(expected.x, vec.x, Tolerance);
            Assert.AreEqual(expected.y, vec.y, Tolerance);
        }
        
        [Test]
        public void LargeAngle_ShouldWorkCorrectly()
        {
            // Arrange
            var polar = new PolarCoord(5f, 720f); // 两圈
            
            // Act
            var vec = polar.ToVec();
            
            // Assert
            // 720度 = 0度
            var expected = new PolarCoord(5f, 0f).ToVec();
            Assert.AreEqual(expected.x, vec.x, Tolerance);
            Assert.AreEqual(expected.y, vec.y, Tolerance);
        }
        
        [Test]
        public void NegativeAngle_ShouldWorkCorrectly()
        {
            // Arrange
            var polar = new PolarCoord(5f, -30f);
            
            // Act
            var vec = polar.ToVec();
            
            // Assert
            var expected = new PolarCoord(5f, 330f).ToVec();
            Assert.AreEqual(expected.x, vec.x, Tolerance);
            Assert.AreEqual(expected.y, vec.y, Tolerance);
        }
        
        #endregion
        
        #region 往返转换测试
        
        [Test]
        public void RoundTrip_FromVectorToPolarAndBack_ShouldReturnOriginal()
        {
            // Arrange
            var originalVec = new Vector2(3, 4);
            var center = Vector2.zero;
            
            // Act
            var polar = new PolarCoord(center, originalVec);
            var resultVec = polar.ToVec(center);
            
            // Assert
            Assert.AreEqual(originalVec.x, resultVec.x, Tolerance);
            Assert.AreEqual(originalVec.y, resultVec.y, Tolerance);
        }
        
        [Test]
        public void RoundTrip_WithNonZeroCenter_ShouldReturnOriginal()
        {
            // Arrange
            var center = new Vector2(10, 20);
            var point = new Vector2(13, 24);
            
            // Act
            var polar = new PolarCoord(center, point);
            var resultPoint = polar.ToVec(center);
            
            // Assert
            Assert.AreEqual(point.x, resultPoint.x, Tolerance);
            Assert.AreEqual(point.y, resultPoint.y, Tolerance);
        }
        
        #endregion
    }
}

