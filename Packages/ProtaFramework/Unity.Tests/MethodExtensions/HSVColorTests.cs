using NUnit.Framework;
using UnityEngine;
using Prota.Unity;
using System;

namespace Prota.Unity.Tests.MethodExtensions
{
    public class HSVColorTests
    {
        // Test cases: R, G, B
        static float[][] RgbTestCases =
        {
            new float[] { 0.0f, 0.0f, 0.0f }, // Black
            new float[] { 1.0f, 1.0f, 1.0f }, // White
            new float[] { 1.0f, 0.0f, 0.0f }, // Red
            new float[] { 0.0f, 1.0f, 0.0f }, // Green
            new float[] { 0.0f, 0.0f, 1.0f }, // Blue
            new float[] { 1.0f, 1.0f, 0.0f }, // Yellow
            new float[] { 0.0f, 1.0f, 1.0f }, // Cyan
            new float[] { 1.0f, 0.0f, 1.0f }, // Magenta
            new float[] { 0.5f, 0.5f, 0.5f }, // Gray
            new float[] { 0.2f, 0.4f, 0.6f }, // Random 1
            new float[] { 0.8f, 0.1f, 0.5f }, // Random 2
        };

        [Test]
        [TestCaseSource(nameof(RgbTestCases))]
        public void RGB_To_HSV_Matches_Unity_Implementation(float r, float g, float b)
        {
            var color = new Color(r, g, b);
            
            // Unity implementation
            Color.RGBToHSV(color, out float expectedH, out float expectedS, out float expectedV);
            
            // My implementation
            var hsv = new HSVColor(color);
            
            float tolerance = 0.001f;
            
            Assert.That(hsv.h, Is.EqualTo(expectedH).Within(tolerance), $"Hue mismatch for RGB({r},{g},{b})");
            Assert.That(hsv.s, Is.EqualTo(expectedS).Within(tolerance), $"Saturation mismatch for RGB({r},{g},{b})");
            Assert.That(hsv.v, Is.EqualTo(expectedV).Within(tolerance), $"Value mismatch for RGB({r},{g},{b})");
        }

        [Test]
        [TestCaseSource(nameof(RgbTestCases))]
        public void HSV_To_RGB_Matches_Unity_Implementation(float r, float g, float b)
        {
            // First get valid HSV from Unity
            Color.RGBToHSV(new Color(r, g, b), out float h, out float s, out float v);
            
            var hsv = new HSVColor(h, s, v);
            var color = hsv.ToColor();
            
            // Unity implementation
            var expectedColor = Color.HSVToRGB(h, s, v);
            
            float tolerance = 0.001f;
            
            Assert.That(color.r, Is.EqualTo(expectedColor.r).Within(tolerance), $"R mismatch for HSV({h},{s},{v})");
            Assert.That(color.g, Is.EqualTo(expectedColor.g).Within(tolerance), $"G mismatch for HSV({h},{s},{v})");
            Assert.That(color.b, Is.EqualTo(expectedColor.b).Within(tolerance), $"B mismatch for HSV({h},{s},{v})");
        }
        
        [Test]
        public void HSV_RoundTrip_Preserves_Color()
        {
            for(int i = 0; i < 100; i++)
            {
                float r = UnityEngine.Random.value;
                float g = UnityEngine.Random.value;
                float b = UnityEngine.Random.value;
                
                var originalColor = new Color(r, g, b);
                var hsv = new HSVColor(originalColor);
                var finalColor = hsv.ToColor();
                
                float tolerance = 0.001f;
                Assert.That(finalColor.r, Is.EqualTo(originalColor.r).Within(tolerance), "R mismatch in roundtrip");
                Assert.That(finalColor.g, Is.EqualTo(originalColor.g).Within(tolerance), "G mismatch in roundtrip");
                Assert.That(finalColor.b, Is.EqualTo(originalColor.b).Within(tolerance), "B mismatch in roundtrip");
            }
        }
        
        [Test]
        public void Implicit_Conversion_Works()
        {
            var c = new Color(1, 0, 0);
            HSVColor hsv = (HSVColor)c;
            Assert.That(hsv.h, Is.EqualTo(0).Within(0.001f));
            Assert.That(hsv.s, Is.EqualTo(1).Within(0.001f));
            Assert.That(hsv.v, Is.EqualTo(1).Within(0.001f));
            
            Color c2 = (Color)hsv;
            Assert.That(c2.r, Is.EqualTo(1).Within(0.001f));
            Assert.That(c2.g, Is.EqualTo(0).Within(0.001f));
            Assert.That(c2.b, Is.EqualTo(0).Within(0.001f));
        }
    }
}
