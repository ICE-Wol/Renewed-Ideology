using System;
using UnityEngine;
using NUnit.Framework;
using Prota.Unity;

namespace Geometry
{
    public class LineTests
    {
        private const float Tolerance = 1e-6f;
        
        #region 基本属性测试
        
        [Test]
        public void Line_FromTwoPoints_ShouldCreateCorrectLine()
        {
            // Arrange
            var line = new Line(new Vector2(0, 0), new Vector2(3, 4));
            
            // Act & Assert
            Assert.AreEqual(new Vector2(0, 0), line.point);
            Assert.AreEqual(3f, line.direction.x, Tolerance);
            Assert.AreEqual(4f, line.direction.y, Tolerance);
            Assert.AreEqual(5f, line.direction.magnitude, Tolerance);
        }
        
        [Test]
        public void Line_FromSegment_ShouldCreateCorrectLine()
        {
            // Arrange
            var segment = new Segment(new Vector2(1, 2), new Vector2(5, 8));
            var line = new Line(segment);
            
            // Act & Assert
            Assert.AreEqual(new Vector2(1, 2), line.point);
            Vector2 expectedDir = new Vector2(5, 8) - new Vector2(1, 2);
            Assert.AreEqual(expectedDir.x, line.direction.x, Tolerance);
            Assert.AreEqual(expectedDir.y, line.direction.y, Tolerance);
            Assert.AreEqual(expectedDir.magnitude, line.direction.magnitude, Tolerance);
        }
        
        [Test]
        public void Line_FromTwoPoints_ZeroLength_ShouldThrowException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Line(new Vector2(1, 1), new Vector2(1, 1)));
        }
        
        [Test]
        public void Line_FromZeroLengthSegment_ShouldThrowException()
        {
            // Arrange
            var segment = new Segment(new Vector2(1, 1), new Vector2(1, 1));
            
            // Act & Assert - 直接测试Line构造函数
            Assert.Throws<ArgumentException>(() => new Line(segment));
            // 通过 line 属性访问也会抛出异常
            Assert.Throws<ArgumentException>(() => { var _ = segment.line; });
        }
        
        #endregion
        
        #region 平行检测测试
        
        [Test]
        public void IsParallelWith_ParallelLines_ShouldReturnTrue()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(2, 0));
            var line2 = new Line(new Vector2(0, 1), new Vector2(2, 1));
            
            // Act
            var result = line1.IsParallelWith(line2);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsParallelWith_NonParallelLines_ShouldReturnFalse()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(2, 0));
            var line2 = new Line(new Vector2(0, 0), new Vector2(1, 1));
            
            // Act
            var result = line1.IsParallelWith(line2);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsParallelWith_VerticalParallelLines_ShouldReturnTrue()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(0, 2));
            var line2 = new Line(new Vector2(1, 0), new Vector2(1, 2));
            
            // Act
            var result = line1.IsParallelWith(line2);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsParallelWith_DiagonalParallelLines_ShouldReturnTrue()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(2, 2));
            var line2 = new Line(new Vector2(1, 0), new Vector2(3, 2));
            
            // Act
            var result = line1.IsParallelWith(line2);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsParallelWith_OppositeDirectionParallelLines_ShouldReturnTrue()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(2, 0));
            var line2 = new Line(new Vector2(2, 1), new Vector2(0, 1));
            
            // Act
            var result = line1.IsParallelWith(line2);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsParallelWith_IdenticalLines_ShouldReturnTrue()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(2, 0));
            var line2 = new Line(new Vector2(0, 0), new Vector2(2, 0));
            
            // Act
            var result = line1.IsParallelWith(line2);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsParallelWith_ColinearLines_ShouldReturnTrue()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(2, 0));
            var line2 = new Line(new Vector2(1, 0), new Vector2(3, 0));
            
            // Act
            var result = line1.IsParallelWith(line2);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        #endregion
        
        #region 共线检测测试
        
        [Test]
        public void IsCollinearWith_CollinearLines_ShouldReturnTrue()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(2, 0));
            var line2 = new Line(new Vector2(1, 0), new Vector2(3, 0));
            
            // Act
            var result = line1.IsCollinearWith(line2);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsCollinearWith_ParallelButNotCollinear_ShouldReturnFalse()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(2, 0));
            var line2 = new Line(new Vector2(0, 1), new Vector2(2, 1));
            
            // Act
            var result = line1.IsCollinearWith(line2);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsCollinearWith_NonParallelLines_ShouldReturnFalse()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(2, 0));
            var line2 = new Line(new Vector2(0, 0), new Vector2(1, 1));
            
            // Act
            var result = line1.IsCollinearWith(line2);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsCollinearWith_IdenticalLines_ShouldReturnTrue()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(2, 0));
            var line2 = new Line(new Vector2(0, 0), new Vector2(2, 0));
            
            // Act
            var result = line1.IsCollinearWith(line2);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsCollinearWith_OverlappingLines_ShouldReturnTrue()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(4, 0));
            var line2 = new Line(new Vector2(2, 0), new Vector2(6, 0));
            
            // Act
            var result = line1.IsCollinearWith(line2);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        #endregion
        
        #region 相等性测试
        
        [Test]
        public void Equals_IdenticalLines_ShouldReturnTrue()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(2, 0));
            var line2 = new Line(new Vector2(0, 0), new Vector2(2, 0));
            
            // Act & Assert
            Assert.IsTrue(line1.Equals(line2));
            Assert.IsTrue(line1 == line2);
            Assert.IsFalse(line1 != line2);
        }
        
        [Test]
        public void Equals_CollinearLines_ShouldReturnTrue()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(2, 0));
            var line2 = new Line(new Vector2(1, 0), new Vector2(3, 0));
            
            // Act & Assert
            Assert.IsTrue(line1.Equals(line2));
        }
        
        [Test]
        public void Equals_ParallelButNotCollinear_ShouldReturnFalse()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(2, 0));
            var line2 = new Line(new Vector2(0, 1), new Vector2(2, 1));
            
            // Act & Assert
            Assert.IsFalse(line1.Equals(line2));
            Assert.IsTrue(line1 != line2);
        }
        
        [Test]
        public void Equals_NonParallelLines_ShouldReturnFalse()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(2, 0));
            var line2 = new Line(new Vector2(0, 0), new Vector2(1, 1));
            
            // Act & Assert
            Assert.IsFalse(line1.Equals(line2));
        }
        
        #endregion

        #region 垂直检测测试
        
        [Test]
        public void IsPerpendicular_PerpendicularLines_ShouldReturnTrue()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(1, 0));
            var line2 = new Line(new Vector2(0, 0), new Vector2(0, 1));
            
            // Act
            var result = line1.IsPerpendicular(line2);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsPerpendicular_PerpendicularLines_Diagonal_ShouldReturnTrue()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(1, 1));
            var line2 = new Line(new Vector2(0, 0), new Vector2(-1, 1));
            
            // Act
            var result = line1.IsPerpendicular(line2);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsPerpendicular_NonPerpendicularLines_ShouldReturnFalse()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(1, 0));
            var line2 = new Line(new Vector2(0, 0), new Vector2(1, 1));
            
            // Act
            var result = line1.IsPerpendicular(line2);
            
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsPerpendicular_ParallelLines_ShouldReturnFalse()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(1, 0));
            var line2 = new Line(new Vector2(0, 1), new Vector2(1, 1));
            
            // Act
            var result = line1.IsPerpendicular(line2);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        #endregion
    }
}

