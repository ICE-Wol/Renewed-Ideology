using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Prota.Unity
{
    /// <summary>
    /// RAII-style Profiler scope for automatic BeginSample/EndSample management.
    /// Usage: using (new ProfilerScope("MyOperation")) { ... }
    /// </summary>
    public readonly struct ProfilerScope : IDisposable
    {
        /// <summary>
        /// Creates a new Profiler scope with the specified name.
        /// </summary>
        /// <param name="name">The name of the profiler sample</param>
        public ProfilerScope(string name)
        {
            Profiler.BeginSample(name);
        }
        
        /// <summary>
        /// Creates a new Profiler scope with the specified name and object context.
        /// </summary>
        /// <param name="name">The name of the profiler sample</param>
        /// <param name="obj">The object context for the profiler sample</param>
        public ProfilerScope(string name, UnityEngine.Object obj)
        {
            Profiler.BeginSample(name, obj);
        }
        
        /// <summary>
        /// Disposes the scope and calls EndSample.
        /// </summary>
        public void Dispose()
        {
            Profiler.EndSample();
        }
    }
    
}
