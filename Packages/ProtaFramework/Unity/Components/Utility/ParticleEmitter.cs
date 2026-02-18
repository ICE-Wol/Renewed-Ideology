using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prota.Unity
{
    public class EmitterVisualRecord
    {
        public GameObject root;
        public GameObject instance;
        public ParticleSystem[] ps;
        
        static ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1000];
        
        public static Dictionary<GameObject, EmitterVisualRecord> records = new();
        
        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            records.Clear();
        }
        
        public static void Emit(GameObject root, Vector2 position, float rotation, Vector2 velocity)
        {
            var ep = new ParticleSystem.EmitParams
            {
                position = position,
                rotation = rotation,
                velocity = velocity,
            };
            
            if (!records.TryGetValue(root, out var record))
            {
                record = new EmitterVisualRecord
                {
                    root = root,
                    instance = GameObject.Instantiate(root),
                };
                record.instance.transform.position = new Vector2(10000, 10000);
                record.ps = record.instance.GetComponentsInChildren<ParticleSystem>();
                records.Add(root, record);
            }
            
            foreach (var p in record.ps)
            {
                if (p.emission.burstCount == 0)
                {
                    p.Emit(1);
                    continue;
                }
                else
                {
                    var burst = p.emission.GetBurst(0);
                    var count = UnityEngine.Random.Range(burst.minCount, burst.maxCount);
                    p.Emit(ep, count);
                    continue;
                }
            }
        }
    }

    // public class ParticleEmitter : MonoBehaviour
    // {
    //     public GameObject root;
        
    //     public bool emitOnStart = false;
        
    //     void Start()
    //     {
    //         if (emitOnStart) Emit();
    //     }

    //     public void Emit()
    //     {
    //         var velocity = this.gameObject.TryGetVelocity(out var v) ? v : Vector2.zero;
    //         var ep = new ParticleSystem.EmitParams
    //         {
    //             position = this.transform.position,
    //             velocity = velocity,
    //             rotation = this.transform.rotation.eulerAngles.z,
    //         };
    //         EmitterVisualRecord.Emit(root, ep.position, ep.rotation, ep.velocity);
    //     }
    // }
}

