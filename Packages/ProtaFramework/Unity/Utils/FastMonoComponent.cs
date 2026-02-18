using UnityEngine;
using System;

namespace Prota.Unity
{
	public class FastMonoComponent : MonoBehaviour
	{
		[NonSerialized]
		public new GameObject gameObject = null!;

		[NonSerialized]
		public new Transform transform = null!;

		public RectTransform rectTransform
			=> transform as RectTransform ?? throw new Exception("transform is not a RectTransform");

		protected virtual void Awake()
		{
			this.gameObject = base.gameObject;
			this.transform = base.transform;
		}

		protected virtual void OnDestroy()
		{
			this.gameObject = null;
			this.transform = null;
		}

		public GameObject monoGameObject => base.gameObject;

		public Transform monoTransform => base.transform;
	}
}
