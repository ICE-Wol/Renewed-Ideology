using _Scripts;
using UnityEngine;

public class DistortCtrl : MonoBehaviour
{
    public static float strength = 0.5f;
    
    // 世界坐标
    public static float radius = 5;
    
    // 世界坐标
    public static Vector2 center = new Vector2(0.5f, 0.5f);
    private static readonly int Strength = Shader.PropertyToID("_Strength");
    private static readonly int Radius = Shader.PropertyToID("_Radius");
    private static readonly int Center = Shader.PropertyToID("_Center");

    Material mat => GetComponent<Renderer>().material;
    
    void Update()
    {
        mat.SetFloat(Strength, strength);
        mat.SetFloat(Radius, radius);
        mat.SetVector(Center, Camera.main.WorldToScreenPoint(center));
    }
    
    public static void Set(float strength, float radius, Vector2 center)
    {
        DistortCtrl.strength = strength;
        DistortCtrl.radius = radius;
        DistortCtrl.center = center;
    }
    
    void OnDestroy()
    {
        Destroy(mat);
    }
    
}
