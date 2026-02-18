using System;
using UnityEngine;

namespace Prota.Unity
{
	[CreateAssetMenu(fileName = "RandomSamplerAsset", menuName = "Prota Framework/DataAsset/RandomSamplerAsset")]
    public class RandomSamplerAsset : ScriptableObject, ISerializationCallbackReceiver
    {
        public AnimationCurve probabilityDensity = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        [SerializeField] int sampleCount = 64;

        [NonSerialized] float[] cdf;   // 前缀积分表
        [NonSerialized] float step;

        public float Sample()
        {
            return Sample(UnityEngine.Random.value);
        }

        public float Sample(float u)
        {
            if (cdf == null || cdf.Length < 2)
                return Mathf.Clamp01(u);

            u = Mathf.Clamp01(u);
            if (u <= 0f) return 0f;
            if (u >= 1f) return 1f;

            int n = cdf.Length;

            // 直接算 CDF 区间下标
            float kf = u * (n - 1);
            int k0 = Mathf.FloorToInt(kf);
            if (k0 >= n - 1) return 1f;

            float c0 = cdf[k0];
            float c1 = cdf[k0 + 1];
            if (c1 <= c0)
                return k0 * step;

            // 区间内反插值
            float t = (u - c0) / (c1 - c0);
            return (k0 + t) * step;
        }

        public void OnAfterDeserialize()
        {
            if (sampleCount < 2) sampleCount = 2;
            if (probabilityDensity == null)
                probabilityDensity = AnimationCurve.Linear(0f, 1f, 1f, 1f);

            step = 1f / (sampleCount - 1);
            cdf = new float[sampleCount];

            // 1. 等距采样 PDF
            float prev = Mathf.Max(0f, probabilityDensity.Evaluate(0f));
            float area = 0f;
            cdf[0] = 0f;

            for (int i = 1; i < sampleCount; i++)
            {
                float x = i * step;
                float v = Mathf.Max(0f, probabilityDensity.Evaluate(x));

                // 2. 梯形积分
                area += 0.5f * (prev + v) * step;
                cdf[i] = area;

                prev = v;
            }

            // 3. 归一化
            if (area <= 0f)
            {
                for (int i = 0; i < sampleCount; i++)
                    cdf[i] = i * step;
                return;
            }

            for (int i = 0; i < sampleCount; i++)
                cdf[i] /= area;

            cdf[sampleCount - 1] = 1f;
        }

        public void OnBeforeSerialize() { }
    }
}