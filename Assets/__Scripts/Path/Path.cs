using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

[ExecuteAlways]
public class Path : MonoBehaviour
{
    [System.Serializable]
    public class Waypoint
    {
        public Vector3 position;
        public float speed = 1f;
        public int waitTime = 0;
    }

    public List<Waypoint> waypoints = new List<Waypoint>();
    public Color pathColor = Color.green;
    public float sphereRadius = 0.2f;
    public int curveResolution = 20;

    public bool reversePath;
    public bool flipPathByX;
    public bool addOffset;
    public Vector3 offset;
    private void Update()
    {
        if (reversePath)
        {
            waypoints.Reverse();
            reversePath = false;
        }

        if (flipPathByX) {
            FlipPathByX();
            flipPathByX = false;
        }

        if (addOffset) {
            foreach (var p in waypoints) {
                p.position += offset;
            }
            addOffset = false;
        }
    }
    
    public void FlipPathByX()
    {
        foreach (var p in waypoints) {
            p.position.x = -p.position.x;
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = pathColor;

        // Draw waypoints
        for (int i = 0; i < waypoints.Count; i++)
        {
            var waypoint = waypoints[i];
            Gizmos.DrawWireSphere(waypoint.position, sphereRadius);
            var gui = new GUIStyle();
            gui.fontSize = 50;
            gui.normal.textColor = Color.red;
            //Handles.Label(waypoint.position, i.ToString(), gui);
        }

        // Draw path
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            DrawCatmullRomSpline(i);
        }
    }

    private void DrawCatmullRomSpline(int pos)
    {
        Vector3 p0 = waypoints[ClampListPos(pos - 1)].position;
        Vector3 p1 = waypoints[pos].position;
        Vector3 p2 = waypoints[ClampListPos(pos + 1)].position;
        Vector3 p3 = waypoints[ClampListPos(pos + 2)].position;

        Vector3 lastPos = p1;

        float resolution = 1.0f / curveResolution;

        for (int i = 1; i <= curveResolution; i++)
        {
            float t = i * resolution;
            Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);
            Gizmos.DrawLine(lastPos, newPos);
            lastPos = newPos;
        }
    }

    public int ClampListPos(int pos)
    {
        if (pos < 0)
        {
            pos = waypoints.Count - 1;
        }
        if (pos > waypoints.Count - 1)
        {
            pos = 0;
        }
        return pos;
    }

    public Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        return 0.5f * (a + (b * t) + (c * t2) + (d * t3));
    }
}
