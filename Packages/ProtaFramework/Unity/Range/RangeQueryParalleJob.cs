using System;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Jobs;
using UnityEngine.Profiling;

namespace Prota.Unity
{
	public class RangeQueryGetPositionContext : IMultithreadJobTransformsContext
	{
		AllQueryData queryData = null!;
		
		public void Setup(AllQueryData queryData)
		{
			this.queryData = queryData;
		}

		public void Execute(int elementIndex, ref TransformAccess transform, int threadIndex)
		{
			ref var data = ref queryData.rawData[elementIndex];
			data.position = transform.position;
		}
		
		public TransformAccessArray GetTransformAccessArray()
			=> queryData.transforms;
	}

	public class RangeNodeGetPositionContext : IMultithreadJobTransformsContext
	{
		RangeLayer layer;
		
		public void Setup(RangeLayer layer)
		{
			this.layer = layer;
		}

		public void Execute(int elementIndex, ref TransformAccess transform, int threadIndex)
		{
			ref var data = ref RangeQueryManager.instance.layers[layer.id].rawData[elementIndex];
			data.position = transform.position;
		}

		public TransformAccessArray GetTransformAccessArray()
			=> RangeQueryManager.instance.layers[layer.id].transforms;
	}


	public class ComputeContactsContext : MultithreadJobParallelForContext
	{
		public struct ContactsRecord
		{
			public int indexInLayer;
			public float distanceToEdge;
			public float sqrDistance;
			public float angle;
		}
		
		public List<ContactsRecord>[] contactsPerThread;
		public AllQueryData allQueryData = null!;
		public RangeLayerData[] layers = Array.Empty<RangeLayerData>();
		
		public ComputeContactsContext()
		{
			contactsPerThread = new List<ContactsRecord>[JobsUtility.MaxJobThreadCount];
		}
		
		public void Setup(AllQueryData queryData, RangeLayerData[] layers)
		{
			this.allQueryData = queryData;
			this.layers = layers;
		}
		
		public int GetJobCount()
		{
			return allQueryData.Count;
		}

		public void Execute(int elementIndex, int threadIndex)
		{
			var contacts = contactsPerThread[threadIndex] ??= new();
			
			ref var queryData = ref allQueryData.rawData[elementIndex];
			var queryRadius = queryData.radius;
			var queryLayer = queryData.rangeLayer;
			var needAngleProperty = queryData.sortMode == RangeQuerySortMode.Angle;
			var needDistanceToEdge = queryData.sortMode == RangeQuerySortMode.DistanceToEdge;
			Profiler.BeginSample("ComputeContactsJobContext.Job.Execute.Loop");
			
			// 只查询相关的 layer collection
			if (!(queryLayer.id >= 0 && queryLayer.id < layers.Length))
				throw new ArgumentException($"layerIndex {queryLayer.id} is out of range");
			var layerData = layers[queryLayer.id];
			
			for(var j = 0; j < layerData.Count; j++)
			{
				ref var nodeData = ref layerData.rawData[j];
				var sqrDistance = (queryData.position - nodeData.position).sqrMagnitude;
				var radiusSum = queryRadius + nodeData.radius;
				var threshold = radiusSum * radiusSum;
				if(sqrDistance > threshold) continue;
				
				contacts.Add(new ContactsRecord {
					indexInLayer = j,
					distanceToEdge = needDistanceToEdge ? (queryData.position - nodeData.position).magnitude - nodeData.radius : 0f,
					sqrDistance = sqrDistance,
					angle = needAngleProperty ? (queryData.position - nodeData.position).Angle() : 0f,	
				});
			}
			
			Profiler.EndSample();
			
			Profiler.BeginSample("ComputeContactsJobContext.Job.Execute.Sort");
			switch(queryData.sortMode)
			{
				case RangeQuerySortMode.None:
					break;
				
				case RangeQuerySortMode.Distance:
					contacts.Sort((a, b) => a.sqrDistance.CompareTo(b.sqrDistance));
					break;
				
				case RangeQuerySortMode.Angle:
					contacts.Sort((a, b) => a.angle.CompareTo(b.angle));
					break;
					
				case RangeQuerySortMode.DistanceToEdge:
					contacts.Sort((a, b) => a.distanceToEdge.CompareTo(b.distanceToEdge));
					break;
			}
			Profiler.EndSample();
			
			var queryObject = allQueryData[elementIndex];
			var queryContacts = queryObject.contacts;
			var queryContactOwners = queryObject.contactOwners;
			queryContacts.Clear();
			queryContactOwners.Clear();
			for(int i = 0; i < contacts.Count; i++)
			{
				var contact = contacts[i];
				var indexInLayer = contact.indexInLayer;
				queryContacts.Add(layerData[indexInLayer]);
				queryContactOwners.Add(layerData.rawData[indexInLayer].owner);
			}
			
			contacts.Clear();
		}
	}

}
