using System;
using System.Collections.Generic;
using Prota;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
using Prota.Unity;

namespace Prota.Unity
{

	public static class UpdateQueryPositionsJobContext
	{
		public static Vector2[] positions;
		
		public static void Setup(Vector2[] positions)
		{
			UpdateQueryPositionsJobContext.positions = positions;
		}
		
		public static void Reset()
		{
			positions = null;
		}
	}

}
