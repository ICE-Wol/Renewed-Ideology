using UnityEngine;


public static class ParallaxUtils
{
	/// <summary>
	/// 
	/// </summary>
	public static void ComputeParallax(
		Vector2 posCamera,	// 相机世界位置.
		Vector2 posObj,		// 物体世界位置.
		float zCamera,		// 相机离视平面的距离. 视平面为 z=0. 相机位置为 z=-zCamera.
		float zObj,			// 物体离视平面的距离. 物体位置为 z=zObj.
		out float scaleObj,	// 物体投影在视平面上对应的缩放乘数.
		out Vector2 positionOnRefPlane,		// 物体投影到视平面的相对位置.
		out Vector2 positionOnWorldSpace	// 物体投影到视平面的世界位置.
	)
	{
		// 计算缩放比例
		scaleObj = zCamera / (zCamera + zObj);

		var relativePos = posObj - posCamera;

		// 相对于参考平面的位置 (在挪到了z之后)
		positionOnRefPlane = relativePos * scaleObj;
		
		// 世界空间位置 = 相机位置 + 参考平面位置偏移
		positionOnWorldSpace = posCamera + positionOnRefPlane;
	}
	
}