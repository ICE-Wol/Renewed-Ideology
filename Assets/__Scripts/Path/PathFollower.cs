using System.Collections;
using System.Collections.Generic;
using _Scripts.Tools;
using MEC;
using UnityEngine;
using UnityEngine.Events;
public class PathFollower : MonoBehaviour
{
    public enum LoopType
    {
        None,       // 禁用循环
        Loop,       // 循环模式
        PingPong    // 往返模式
    }

    public Path path;
    public LoopType loopType = LoopType.None; // 使用枚举类型来选择循环模式
    public UnityEvent[] openings;             // UnityEvent 数组

    private float distanceTravelled;
    public int currentSegment;
    private float segmentStartDistance;
    private float segmentEndDistance;
    private float segmentLength;
    private float fixedDeltaTime;
    private bool isReversing;
    private bool isMoving = true;

    public int curWaitTime = 0;
    public float curWaitSpeed;

    private void Start()
    {
        if (path == null || path.waypoints.Count < 4)
        {
            Debug.LogWarning("Path is not defined or does not have enough waypoints.");
            return;
        }
        InitializeSegment(0);
        fixedDeltaTime = Time.fixedDeltaTime;
    }

    private IEnumerator<float> Wait(int time) {
        for (int i = 0; i < time; i++)
            yield return Timing.WaitForOneFrame;
    }
    
    private void FixedUpdate()
    {
        if (!isMoving || path == null || path.waypoints.Count < 4)
        {
            return;
        }

        //-1 didnt stop 
        //-2 has stopped
        //>0 is waiting
        if (curWaitTime == -1) {
            curWaitTime = path.waypoints[currentSegment].waitTime;
            curWaitSpeed = path.waypoints[currentSegment].speed;
        }
        if (curWaitTime > 0) {
            curWaitSpeed.ApproachRef(0, 32f);
            distanceTravelled += (isReversing ? -1 : 1) * curWaitSpeed * fixedDeltaTime;
            transform.position = GetPositionOnPath(distanceTravelled);
            curWaitTime--;
            return;
        }

        if (curWaitTime == 0) {
            curWaitTime = -2;
        }
        
        float speed = path.waypoints[currentSegment].speed;
        distanceTravelled += (isReversing ? -1 : 1) * speed * fixedDeltaTime;

        if (isReversing)
        {
            if (distanceTravelled < segmentStartDistance)
            {
                distanceTravelled = segmentStartDistance;
                curWaitTime = -1;
                AdvanceSegment();
            }
        }
        else
        {
            if (distanceTravelled > segmentEndDistance)
            {
                distanceTravelled = segmentEndDistance;
                curWaitTime = -1;
                AdvanceSegment();
            }
        }

        Vector3 position = GetPositionOnPath(distanceTravelled);
        transform.position = position;
    }

    private void AdvanceSegment()
    {
        if (isReversing)
        {
            currentSegment--;
            if (currentSegment < 0)
            {
                HandleEndOfPath();
                if (!isMoving) return;
            }
        }
        else
        {
            currentSegment++;
            if (currentSegment >= path.waypoints.Count - 1)
            {
                HandleEndOfPath();
                if (!isMoving) return;
            }
        }
        InitializeSegment(currentSegment);
        TriggerNodeEvent(currentSegment); // 触发节点事件
    }

    private void HandleEndOfPath()
    {
        switch (loopType)
        {
            case LoopType.None:
                isMoving = false;
                break;
            case LoopType.Loop:
                currentSegment = 0;
                distanceTravelled = 0f;
                break;
            case LoopType.PingPong:
                isReversing = !isReversing;
                if (isReversing)
                {
                    currentSegment = path.waypoints.Count - 2;
                    distanceTravelled = GetTotalPathLength(currentSegment + 1);
                }
                else
                {
                    currentSegment = 0;
                    distanceTravelled = 0f;
                }
                break;
        }
    }

    private void InitializeSegment(int segmentIndex)
    {
        currentSegment = segmentIndex;
        segmentStartDistance = GetTotalPathLength(currentSegment);
        segmentEndDistance = GetTotalPathLength(currentSegment + 1);
        segmentLength = segmentEndDistance - segmentStartDistance;
    }

    private Vector3 GetPositionOnPath(float distance)
    {
        float t = (distance - segmentStartDistance) / segmentLength;
        t = Mathf.Clamp01(t); // 保证 t 在 0 到 1 之间

        Vector3 p0 = path.waypoints[path.ClampListPos(currentSegment - 1)].position;
        Vector3 p1 = path.waypoints[currentSegment].position;
        Vector3 p2 = path.waypoints[path.ClampListPos(currentSegment + 1)].position;
        Vector3 p3 = path.waypoints[path.ClampListPos(currentSegment + 2)].position;

        return path.GetCatmullRomPosition(t, p0, p1, p2, p3);
    }

    private float GetTotalPathLength(int upToSegment = -1)
    {
        float length = 0f;
        for (int i = 0; i < path.waypoints.Count - 1; i++)
        {
            if (upToSegment != -1 && i >= upToSegment)
            {
                break;
            }

            Vector3 p0 = path.waypoints[path.ClampListPos(i - 1)].position;
            Vector3 p1 = path.waypoints[i].position;
            Vector3 p2 = path.waypoints[path.ClampListPos(i + 1)].position;
            Vector3 p3 = path.waypoints[path.ClampListPos(i + 2)].position;

            Vector3 lastPos = p1;
            float resolution = 1.0f / path.curveResolution;
            for (int j = 1; j <= path.curveResolution; j++)
            {
                float t = j * resolution;
                Vector3 newPos = path.GetCatmullRomPosition(t, p0, p1, p2, p3);
                length += Vector3.Distance(lastPos, newPos);
                lastPos = newPos;
            }
        }
        return length;
    }

    private void TriggerNodeEvent(int nodeIndex)
    {
        if (openings != null && nodeIndex < openings.Length && openings[nodeIndex] != null)
        {
            openings[nodeIndex].Invoke();
        }
    }
}
