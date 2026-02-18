using System;
using System.Collections.Generic;
using Prota;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace Prota.Unity
{
	public interface IMultithreadJobTransformsContext
	{
		public TransformAccessArray GetTransformAccessArray();
		void Execute(int elementIndex, ref TransformAccess transform, int threadIndex);
	}
	
	public class MultithreadJobTransforms<T> : MultithreadJobBase
		where T: class, IMultithreadJobTransformsContext, new()
	{
		string _jobName;
		public string jobName => _jobName ??= $"MultithreadJobTransforms<{typeof(T).Name}>";
		
		public struct Job : IJobParallelForTransform
		{
			[NativeSetThreadIndex]
			public int threadIndex;
			
			public int contextId;
			
			public void Execute(int elementIndex, TransformAccess transform)
			{
				var jobWrapper = (MultithreadJobTransforms<T>)MultithreadManager.instance.jobs[contextId];
				using (new ProfilerScope(jobWrapper.jobName))
				{
					jobWrapper.context.Execute(elementIndex, ref transform, threadIndex);
				}
			}
		}
		
		public int batchCount = 8;
		
		T _context = new();
		
		public T context
		{
			get => _context;
			set => _context = value;
		}
		
		public JobHandle Schedule(JobHandle dependsOn)
		{
			var job = new Job();
			job.contextId = id;
			return job.Schedule(context.GetTransformAccessArray(), dependsOn);
		}
	}
}
