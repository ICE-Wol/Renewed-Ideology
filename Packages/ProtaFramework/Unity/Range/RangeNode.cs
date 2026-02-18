using System;
using System.Collections.Generic;
using Prota;
using Prota.Unity;
using UnityEngine;

namespace Prota.Unity
{
	public class RangeNode : FastMonoComponent
	{
		/// <summary>
		/// RangeNode.owner 用于 RangeQuery 查询到 RangeNode 时,
		/// 索引到真正需要操作/探查的数据对象.
		/// 它是不会也不需要被修改的, 所以在注册 RangeNode 的时候一并注册进去了.
		/// </summary>
		public MonoBehaviour owner = null!;
		
		public RangeLayer rangeLayer = RangeLayer.None;
		
		public float radius = 0.1f;
		
		bool registered = false;
		
		void OnValidate()
		{
			if(!Application.isPlaying) return;
			if(monoGameObject.IsPrefab()) return;
			
			if(rangeLayer == RangeLayer.None)
			{
				Debug.LogError("rangeLayer is not set", this);
				return;
			}
		}
		
		protected override void Awake()
		{
			base.Awake();
			if(owner == null)
				throw new InvalidOperationException($"owner not found in parent {this.GetNamePath()}");
		}
		
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if(AppQuit.isQuitting) return;
			RangeQueryManager.instance.Unregister(this);
			registered = false;
		}
		
		void OnEnable()
		{
			RangeQueryManager.instance.Register(this);
			registered = true;
		}
		
		void OnDisable()
		{
			if(AppQuit.isQuitting) return;
			RangeQueryManager.instance.Unregister(this);	
			registered = false;
		}
		
		public void UpdateData()
		{
			if(!registered) return;
			RangeQueryManager.instance.Unregister(this);
			RangeQueryManager.instance.Register(this);
		}
		
		void OnDrawGizmosSelected()
		{
			var oldColor = Gizmos.color;
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(monoTransform.position, radius);
			Gizmos.color = oldColor;
		}
	}


}
