using System;
using System.Collections.Generic;
using Prota;
using UnityEngine;
using UnityEngine.Profiling;
using Prota.Unity;
using Unity.Jobs;
using Unity.Collections;

namespace Prota.Unity
{
	public struct RangeNodeData
	{
		public MonoBehaviour owner;
		public Vector2 position;
		public float radius;
	}
	
	public struct RangeQueryData
	{
		public float radius;
		public RangeQuerySortMode sortMode;
		public Vector2 position;
		public RangeLayer rangeLayer;
	}
	
	public class RangeLayerData : TransformIndexedCollectionComponent<RangeNode, RangeNodeData>
	{
		public RangeLayerData() : base(32) { }
	}
	
	public class AllQueryData : TransformIndexedCollectionComponent<RangeQuery, RangeQueryData>
		{
			public AllQueryData() : base(64) { }
		}
		

	public class RangeQueryManager : SingletonComponent<RangeQueryManager>, IMultithreadTask
	{
		public bool isJobRunning { get; set; }
		int layersCapacity => RangeLayer.nameToLayer.Count;
		
		// 按 Layer 分别存储的数据
		[NonSerialized] public RangeLayerData[] layers = Array.Empty<RangeLayerData>();
		
		[NonSerialized] public AllQueryData queryData = null!;
		
		public int layerCount => layersCapacity;
		
		public MultithreadJobTransforms<RangeQueryGetPositionContext> rangeQueryGetPositionJob = null!;
		public MultithreadJobTransforms<RangeNodeGetPositionContext>[] rangeNodeGetPositionJobs = null!;
		public MultithreadJobParallelFor<ComputeContactsContext> computeContactsJob = null!;

		[NonSerialized] bool inited = false;

		protected override void Awake()
		{
			base.Awake();
			if(!inited) Init();
		}
		
		public void Init()
		{
			if(inited) return;
			
			// 初始化数据
			queryData = new AllQueryData();
			layers = new RangeLayerData[layersCapacity];
			for(int i = 0; i < layersCapacity; i++)
			{
				layers[i] = new RangeLayerData();
			}
			
			// 初始化各个job.
			rangeQueryGetPositionJob = new() { batchCount = 16 };
			rangeQueryGetPositionJob.context.Setup(queryData);
			
			rangeNodeGetPositionJobs = new MultithreadJobTransforms<RangeNodeGetPositionContext>[layersCapacity];
			for(int i = 0; i < layersCapacity; i++)
			{
				rangeNodeGetPositionJobs[i] = new() { batchCount = 16 };
				rangeNodeGetPositionJobs[i].context.Setup(new RangeLayer(i));
			}
			
			computeContactsJob = new() { batchCount = 4 };
			computeContactsJob.context.Setup(queryData, layers);
			
			MultithreadManager.instance.AddTask(this);
			
			inited = true;
			
		}
		
		protected override void OnDestroy()
		{
			rangeQueryGetPositionJob = null!;
			rangeNodeGetPositionJobs = null!;
			computeContactsJob = null!;
			
			for(int i = 0; i < layers.Length; i++)
			{
				layers[i].Dispose();
			}
			
			queryData.Dispose();
			
			base.OnDestroy();
			
			if(AppQuit.isQuitting) return;
			MultithreadManager.instance.RemoveTask(this);
		}
		
		// =================================================================================================
		// =================================================================================================
		
		public void Register(RangeNode node)
		{
			Init();
			if (isJobRunning) throw new InvalidOperationException("Cannot register RangeNode while job is running.");
			if(node.rangeLayer == RangeLayer.None) throw new InvalidOperationException("RangeNode.layer is None.");

			var layerId = node.rangeLayer.id;
			if(!(0 < layerId && layerId <= layers.Length))
				throw new ArgumentOutOfRangeException(nameof(node.rangeLayer), $"RangeLayer.id must be in range [1, {layers.Length}]. Current value: {layerId}");
			var layer = layers[layerId];
			layer.TryAdd(node, new RangeNodeData {
				owner = node.owner,
				position = node.transform.position,
				radius = node.radius,
			});
		}
		
		public void Unregister(RangeNode node)
		{
			Init();
			if (isJobRunning) throw new InvalidOperationException("Cannot deregister RangeNode while job is running.");
			if(node.rangeLayer == RangeLayer.None) throw new InvalidOperationException("RangeNode.layer is None.");
			var layerId = node.rangeLayer.id;
			layers[layerId].TryRemove(node);
		}
		
		public void Register(RangeQuery query)
		{
			Init();
			if (isJobRunning) throw new InvalidOperationException("Cannot register RangeQuery while job is running.");
			if(query.rangeLayer == RangeLayer.None) throw new InvalidOperationException("RangeQuery.layer is None.");
			queryData.TryAdd(query, new RangeQueryData {
				radius = query.radius,
				sortMode = query.sortMode,
				position = query.transform.position,
				rangeLayer = query.rangeLayer,
			});
		}
		
		public void Deregister(RangeQuery query)
		{
			Init();
			if (isJobRunning) throw new InvalidOperationException("Cannot deregister RangeQuery while job is running.");
			if(query.rangeLayer == RangeLayer.None) throw new InvalidOperationException("RangeQuery.layer is None.");
			queryData.TryRemove(query);
		}
		
		// =================================================================================================
		// =================================================================================================
		
		// Inspector 统计信息
		public int GetNodeCount(RangeLayer layer)
		{
			if(layer == RangeLayer.None) return 0;
			return layers[layer.id].Count;
		}
		
		public int GetQueryCount(RangeLayer layer)
		{
			if(layer == RangeLayer.None) return 0;
			var count = 0;
			for(var i = 0; i < queryData.Count; i++)
			{
				if(queryData.rawData[i].rangeLayer == layer)
					count++;
			}
			return count;
		}
		
		public int GetTotalQueryCount()
		{
			return queryData.Count;
		}
		
		// =================================================================================================
		// =================================================================================================
		
		public JobHandle OnJobStart(JobHandle dependsOn)
		{
			using (new ProfilerScope("RangeQueryManager.ComputeContacts Get all positions"))
			{
				var getPositionsJobHandle = rangeQueryGetPositionJob.Schedule(dependsOn);

				var handles = new NativeArray<JobHandle>(layersCapacity + 1, Allocator.Temp);
				for (int i = 0; i < layersCapacity; i++)
				{
					handles[i] = rangeNodeGetPositionJobs[i].Schedule(dependsOn);
				}
				handles[layersCapacity] = getPositionsJobHandle;
				var getHandles = JobHandle.CombineDependencies(handles);
				handles.Dispose();

				var jobHandle = computeContactsJob.Schedule(getHandles);

				return jobHandle;
			}
		}
		
		public void OnBeforeJobComplete()
		{
		}
		
		public void OnJobComplete()
		{
			
		}
		
		public Type[] GetDependencies()
		{
			return Array.Empty<Type>();
		}
		
		public Type[] GetCompleteDependencies()
		{
			return Array.Empty<Type>();
		}
	}
}
