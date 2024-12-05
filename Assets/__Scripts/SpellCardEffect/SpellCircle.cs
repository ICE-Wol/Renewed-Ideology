using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Enemy;
using _Scripts.Player;
using _Scripts.Tools;
using UnityEngine;
using Random = UnityEngine.Random;
using MEC;

public class SpellCircle: MonoBehaviour {
    public enum  SpellCircleState {
        Collapse,
        Expand,
        Idle,
        Shrink,
    }

    private SpellCircleState _curState;
    public void SetState(SpellCircleState state) {
        _curState = state;
        switch (state) {
            case SpellCircleState.Collapse:
                radius = 8f;
                innerToOuterRadiusRatio = 0.1f;
                SetRadius();
                break;
            case SpellCircleState.Expand:
                radius = 0;
                SetRadius();
                Timing.RunCoroutine(Expand().CancelWith(gameObject));
                break;
            case SpellCircleState.Idle:
                break;
            case SpellCircleState.Shrink:
                StopAllCoroutines();
                _randApproachX = Random.Range(16f, 32f);
                _randApproachY = Random.Range(16f, 32f);
                Timing.RunCoroutine(Shrink().CancelWith(gameObject));
                break;
        }
    }

    private float _randApproachX;
    private float _randApproachY;
    public float idleMaxRadius = 8f;
    public void OnState() {
        switch (_curState) {
            case SpellCircleState.Collapse:
                radius.ApproachRef(0, 4f);
                innerToOuterRadiusRatio.ApproachRef(0.3f, 32f);
                SetRadius();
                if (radius < 0.1f) {
                    SetState(SpellCircleState.Expand);
                }
                break;
            case SpellCircleState.Expand:
                break;
            case SpellCircleState.Idle:
                radius = idleMaxRadius * damageable.curTime/ damageable.maxTime;
                SetRadius();
                break;
            case SpellCircleState.Shrink:
                var playerPos = PlayerCtrl.Player.transform.position;
                var x = transform.position.x;
                var y = transform.position.y;
                x.ApproachRef(playerPos.x, _randApproachX);
                y.ApproachRef(playerPos.y, _randApproachY);
                transform.position = new Vector3(x, y, -5);
                
                
                break;
            
        }
    }
    
    private void SetRadius() {
        for (int i = 0; i < dotNum; i++) {
            var angle = i * 360f / dotNum;
            var rr = Pos(radius, angle);
            pos[i] = (radius, angle);
            vertices[InnerID(i)] = rr.inner;
            vertices[OuterID(i)] = rr.outer;
        }
        pos[dotNum] = pos[0];
        vertices[InnerID(dotNum)] = vertices[0];
        vertices[OuterID(dotNum)] = vertices[OuterID(0)];
        
        mesh.vertices = vertices;
    }
    
    private void Update() {
        OnState();
        Rotate();
    }
    
    
    public Damageable damageable;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public Mesh mesh;

    public int dotNum;
    
    public Vector3[] vertices;
    public int[] triangles;
    public Vector3[] uvs;
    public (float radius, float angle)[] pos;

    /// <summary>
    /// use outer radius to calculate inner radius
    /// </summary>
    public float innerToOuterRadiusRatio = 0.7f;

    public float radius = 5f;

    public float repeatTimes = 5;

    int InnerID(int i) {
        return i;
    }

    int OuterID(int i) {
        return i + dotNum + 1;
    }

    /// <summary>
    /// turn radius and angle into a position
    /// </summary>
    /// <param name="outerR">outer radius</param>
    /// <param name="angle">angle</param>
    /// <returns>inner and outer pos</returns>
    (Vector2 inner, Vector2 outer) Pos(float outerR, float angle) {
        float innerR = innerToOuterRadiusRatio * outerR;
        var x = innerR * Mathf.Cos(angle * Mathf.Deg2Rad);
        var y = innerR * Mathf.Sin(angle * Mathf.Deg2Rad);
        var innerPos = new Vector3(x, y,-5);
        x = outerR * Mathf.Cos(angle * Mathf.Deg2Rad);
        y = outerR * Mathf.Sin(angle * Mathf.Deg2Rad);
        var outerPos = new Vector3(x, y,-5);
        return (innerPos, outerPos);
    }

    private void Init() {
        mesh = new Mesh();
        vertices = new Vector3[dotNum * 2 + 2];
        uvs = new Vector3[dotNum * 2 + 2];
        triangles = new int[dotNum * 2];
        pos = new (float radius, float angle)[dotNum + 1];

        for (int i = 0; i < dotNum; i++) {
            var angle = i * 360f / dotNum;
            var rr = Pos(radius, angle);
            pos[i] = (radius, angle);
            vertices[InnerID(i)] = rr.inner;
            vertices[OuterID(i)] = rr.outer;
            uvs[InnerID(i)] = new Vector3((float)i / dotNum * repeatTimes, lineSpriteType / 8f, 0);
            uvs[OuterID(i)] = new Vector3((float)i / dotNum * repeatTimes, (lineSpriteType + 1) / 8f, 0);
        }

        pos[dotNum] = pos[0];
        vertices[InnerID(dotNum)] = vertices[0];
        vertices[OuterID(dotNum)] = vertices[OuterID(0)];
        uvs[InnerID(dotNum)] = new Vector3(repeatTimes, lineSpriteType / 8f, 0);
        uvs[OuterID(dotNum)] = new Vector3(repeatTimes, (lineSpriteType + 1) / 8f, 0);

        triangles = new int[dotNum * 6];
        var m = dotNum * 2;
        for (int i = 0; i < dotNum; i++) {
            triangles[i * 6] = InnerID(i);
            triangles[i * 6 + 1] = InnerID(i + 1);
            triangles[i * 6 + 2] = OuterID(i);
            triangles[i * 6 + 3] = InnerID(i + 1);
            triangles[i * 6 + 4] = OuterID(i + 1);
            triangles[i * 6 + 5] = OuterID(i);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.SetUVs(0, uvs);
        mesh.RecalculateBounds();
        meshFilter.mesh = mesh;
    }

    IEnumerator<float> Expand() {
        innerToOuterRadiusRatio = 0.2f;
        transform.localScale = Vector3.zero;
        var localScale = 0f;
        for (int changeProgress = 0; changeProgress < 25; changeProgress += 1) {
            localScale.ApproachRef(1, 8f);
            transform.localScale = localScale * Vector3.one;
            
            for (int i = 0; i < dotNum; i++) {
                var angle = pos[i].angle;
                var curRad = pos[i].radius;
                var tarRad = //6f * (Mathf.Sin(Mathf.Deg2Rad * angle * 6) / 2 + 1f);
                    6f * (0.1f - Mathf.Sin(Mathf.Deg2Rad * angle * 6));
                    //2f * (0.1f - Mathf.Sin(Mathf.Deg2Rad * angle * 3));
                curRad = curRad.ApproachRef(tarRad, 8f);
                
                pos[i] = (curRad, angle);
                //fill it back to where it comes from
                var rr = Pos(curRad, angle);
                
                vertices[InnerID(i)] = rr.inner;
                vertices[OuterID(i)] = rr.outer;
            }
            vertices[InnerID(dotNum)] = vertices[0];
            vertices[OuterID(dotNum)] = vertices[OuterID(0)];
            
            mesh.vertices = vertices;
            yield return Timing.WaitForOneFrame; 
        }

        for (int changeProgress = 0; changeProgress < 100; changeProgress += 1) {
            for (int i = 0; i < dotNum; i++) {
                var angle = pos[i].angle;
                var curRad = pos[i].radius;
                var tarRad = 5f;
                curRad = curRad.ApproachRef(tarRad, 16f);
                pos[i] = (curRad, angle);
                //fill it back to where it comes from
                var rr = Pos(curRad, angle);

                vertices[InnerID(i)] = rr.inner;
                vertices[OuterID(i)] = rr.outer;
            }

            vertices[InnerID(dotNum)] = vertices[0];
            vertices[OuterID(dotNum)] = vertices[OuterID(0)];

            mesh.vertices = vertices;

            if (changeProgress > 30) {
                innerToOuterRadiusRatio.ApproachRef(0.85f, 16f);
            }
            
            yield return Timing.WaitForOneFrame;
        }
        
        radius = 5f;
        SetRadius();
        SetState(SpellCircleState.Idle);
    }
    IEnumerator<float> Shrink() {
        for (int changeProgress = 0; changeProgress < 100; changeProgress += 1) {
            for (int i = 0; i < dotNum; i++) {
                var angle = pos[i].angle;
                var curRad = pos[i].radius;
                var tarRad = 3f * (0.1f - Mathf.Sin(Mathf.Deg2Rad * angle * 6))/2f;
                curRad = curRad.ApproachRef(tarRad, 16f);
                pos[i] = (curRad, angle);
                //fill it back to where it comes from
                var rr = Pos(curRad, angle);
                
                vertices[InnerID(i)] = rr.inner;
                vertices[OuterID(i)] = rr.outer;
            }
            vertices[InnerID(dotNum)] = vertices[0];
            vertices[OuterID(dotNum)] = vertices[OuterID(0)];
            
            mesh.vertices = vertices;
            //innerToOuterRadiusRatio -= 0.005f;
            innerToOuterRadiusRatio.ApproachRef(0.2f, 4f);
            yield return Timing.WaitForOneFrame;
        }

        float curScale = 1f;
        float curAlpha = 0.3f;
        for (int changeProgress = 0; changeProgress < 100; changeProgress += 1) {
            curScale.ApproachRef(0, 32f);
            transform.localScale = curScale * Vector3.one;
            innerToOuterRadiusRatio.ApproachRef(0.2f, 4f);
            curAlpha.ApproachRef(0, 16f);
            //meshRenderer.material.color = meshRenderer.material.color.SetAlpha(curAlpha);
            meshRenderer.material.SetFloat(Alpha, curAlpha);
            yield return Timing.WaitForOneFrame;
        }
        
        gameObject.SetActive(false);
    }
    


    private float _timer;
    [Tooltip("use to divide timer, >= 1 required")]
    public float speedMultiplier;

    [Tooltip("0-6 available")]
    public int lineSpriteType;

    private static readonly int Alpha = Shader.PropertyToID("_Alpha");

    private void Rotate() {
        for (int i = 0; i < dotNum; i++) {
            uvs[InnerID(i)] = new Vector3((float)i / dotNum * repeatTimes + _timer / speedMultiplier,
                lineSpriteType / 8f, 0);
            uvs[OuterID(i)] = new Vector3((float)i / dotNum * repeatTimes + _timer / speedMultiplier,
                (lineSpriteType + 1) / 8f, 0);
        }

        uvs[InnerID(dotNum)] = new Vector3(repeatTimes + _timer / speedMultiplier, lineSpriteType / 8f, 0);
        uvs[OuterID(dotNum)] = new Vector3(repeatTimes + _timer / speedMultiplier, (lineSpriteType + 1) / 8f, 0);
        mesh.SetUVs(0, uvs);
        _timer++;
    }

    private void Awake() {
        Init();
    }

    public void ResetCircle() {
        //Init();
        transform.localScale = Vector3.one;
        meshRenderer.material.SetFloat(Alpha, 0.3f);
        SetState(SpellCircleState.Collapse);
        transform.localPosition = new Vector3(0, 0, -5f);
    }
}
