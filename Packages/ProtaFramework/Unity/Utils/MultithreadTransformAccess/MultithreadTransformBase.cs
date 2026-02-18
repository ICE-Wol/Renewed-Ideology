using UnityEngine;

namespace Prota.Unity
{

	public class MultithreadTransformBase : FastMonoComponent
	{
		static TransformIndexedCollection<MultithreadTransformBase> all;

		[RuntimeInitializeOnLoadMethod]
		static void Init()
		{
			all = new TransformIndexedCollection<MultithreadTransformBase>(128, x => x.transform);
		}
		
		protected override void Awake()
		{
			base.Awake();

			if (!all.TryAdd(this))
				throw new System.Exception("Internal error on adding to TransformIndexedCollection");
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if(!all.TryRemove(this))
				throw new System.Exception("Internal error on removing from TransformIndexedCollection");
		}
		
	}

}