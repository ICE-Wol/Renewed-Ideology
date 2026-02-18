using System;
using System.Collections.Generic;
using Prota;
using Prota.Unity;
using UnityEngine;

namespace Prota.Unity
{

	public enum RangeQuerySortMode
	{
		None,
		// 按到目标中心的距离排序
		Distance,
		// 按朝向目标的极角排序
		Angle,
		// 按到目标最外层的距离排序
		DistanceToEdge,
	}

	public class RangeQuery : MonoBehaviour
	{
		public RangeLayer rangeLayer = RangeLayer.None;
		
		public RangeQuerySortMode sortMode = RangeQuerySortMode.None;
		
		public float radius = 10f;

		public List<RangeNode> contacts = new();
		public List<MonoBehaviour> contactOwners = new();

		public Action onContactsUpdateOnWorkerThread = null;
		
		public bool registered = false;
		
		void OnEnable()
		{
			RangeQueryManager.instance.Register(this);
			contacts.Clear();
			contactOwners.Clear();
			registered = true;
		}
		
		void OnDisable()
		{
			if(AppQuit.isQuitting) return;
			RangeQueryManager.instance.Deregister(this);
			contacts.Clear();
			contactOwners.Clear();
			registered = false;
		}
		
		void OnDestroy()
		{
			if(AppQuit.isQuitting) return;
			RangeQueryManager.instance.Deregister(this);
			registered = false;
		}
		
		public void UpdateData()
		{
			if(!registered) return;
			RangeQueryManager.instance.Deregister(this);
			RangeQueryManager.instance.Register(this);
		}
		
		// ============================================================================
		// ============================================================================
		
		void OnDrawGizmosSelected()
		{
			ProtaDebug.DrawCircle(transform.position, radius, Color.red);
		}
	}

}
