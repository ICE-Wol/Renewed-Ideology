using UnityEngine;
using System;
using System.Runtime.InteropServices;
using Prota;

namespace Prota.Unity
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct HSVColor
    {
        public float hue;
        
        public float saturation;
        
        public float value;
        
        public float h
        {
            get => hue;
            set => hue = value;
        }
        
        public float s
        {
            get => saturation;
            set => saturation = value;
        }
        
        public float v
        {
            get => value;
            set => this.value = value;
        }
        
        public HSVColor(float h, float s, float v)
        {
            hue = h;
            saturation = s;
            value = v;
        }
        
        public HSVColor(Color c)
        {
            RGB2HSV(c.r, c.g, c.b, out hue, out saturation, out value);
        }
        
        public static explicit operator Color(HSVColor hsv)
        {
            HSV2RGB(hsv.hue, hsv.saturation, hsv.value, out float r, out float g, out float b);
            return new Color(r, g, b);
        }
        
        public static explicit operator HSVColor(Color c)
        {
            return new HSVColor(c);
        }
        
        public Color ToColor(float alpha = 1f)
        {
            HSV2RGB(hue, saturation, value, out float r, out float g, out float b);
            return new Color(r, g, b, alpha);
        }
        
        public override string ToString()
        {
            return $"HSV({hue}, {saturation}, {value})";
        }
        
        public static void HSV2RGB(float h, float s, float v, out float r, out float g, out float b)
        {
            if (s == 0)
            {
                r = g = b = v;
                return;
            }

            // h is 0..1
            // sector 0 to 5
            float sectorPos = h * 6.0f;
            int sectorNumber = (int)Mathf.Floor(sectorPos);
            float fractionalSector = sectorPos - sectorNumber;

            float p = v * (1.0f - s);
            float q = v * (1.0f - (s * fractionalSector));
            float t = v * (1.0f - (s * (1.0f - fractionalSector)));

            // sectorNumber can be 6 if h == 1.0f, treat it as 0
            if (sectorNumber == 6) sectorNumber = 0;
            
            switch (sectorNumber)
            {
                case 0:
                    r = v;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = v;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = v;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = v;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = v;
                    break;
                case 5:
                    r = v;
                    g = p;
                    b = q;
                    break;
                default:
                    // Should not happen if h is within [0, 1]
                    // If h is outside, we might want to normalize it or handle it.
                    // For now, let's assume normalized h or handle modulo logic if needed, 
                    // but standard Color.HSVToRGB usually expects 0-1.
                    // Let's implement wrap logic if sectorNumber is weird, but Floor on 0-1 shouldn't be.
                    // Actually, if h < 0 or h > 1, we might want to Repeat it.
                    // Let's rely on caller or just handle modulo logic implicitly by Ensure h is 0-1?
                    // Unity's Color.HSVToRGB clamps or wraps? 
                    // Let's follow HSLColor logic. HSLColor seems to handle offsets with Repeat.
                    // But inside HSL2RGB, it calls Hue2RGB which handles t < 0 and t > 1.
                    
                    // Let's just handle fallback
                    r = v;
                    g = v;
                    b = v;
                    break;
            }
        }
        
        public static void RGB2HSV(float r, float g, float b, out float h, out float s, out float v)
        {
            Color.RGBToHSV(new Color(r, g, b), out h, out s, out v);
            
            // Or implement manually to avoid Color struct overhead if performance matters, 
            // but Color.RGBToHSV is efficient and standard in Unity.
            // HSLColor implemented RGB2HSL manually.
            // Let's implement manually to match the style and be independent if needed, 
            // and also to ensure we have full control (e.g. consistency).
            
            /*
            float min = Mathf.Min(r, Mathf.Min(g, b));
            float max = Mathf.Max(r, Mathf.Max(g, b));
            float delta = max - min;

            v = max;

            if (delta == 0)
            {
                s = 0;
                h = 0; // undefined really, but 0 is standard
            }
            else
            {
                s = delta / max; // max != 0 because delta != 0 implies max > min >= 0. 
                                 // If max was 0, then min must be 0, so delta is 0, handled above.
                
                if (r == max)
                {
                    h = (g - b) / delta;
                }
                else if (g == max)
                {
                    h = 2 + (b - r) / delta;
                }
                else
                {
                    h = 4 + (r - g) / delta;
                }
                
                h *= 60; // degrees
                if (h < 0)
                    h += 360;
                    
                h /= 360.0f; // 0..1
            }
            */
            
            // Actually, Unity's Color.RGBToHSV is fine. But HSLColor.cs implemented it manually.
            // Let's stick to manual implementation to be "pure" and consistent with HSLColor.
            
            float min = (r < g) ? ((r < b) ? r : b) : ((g < b) ? g : b);
            float max = (r > g) ? ((r > b) ? r : b) : ((g > b) ? g : b);
            float delta = max - min;
            
            v = max;
            
            if (Mathf.Approximately(delta, 0))
            {
                h = 0;
                s = 0;
            }
            else
            {
                s = (max <= 0) ? 0 : (delta / max);
                
                if (r >= max)
                {
                    h = (g - b) / delta;
                }
                else if (g >= max)
                {
                    h = 2.0f + (b - r) / delta;
                }
                else
                {
                    h = 4.0f + (r - g) / delta;
                }
                
                h /= 6.0f;
                
                if (h < 0)
                    h += 1.0f;
            }
        }
    }
    
    public static partial class UnityMethodExtensions
    {
        public static HSVColor ToHSV(this Color c)
        {
            return (HSVColor)c;
        }
    }
}
