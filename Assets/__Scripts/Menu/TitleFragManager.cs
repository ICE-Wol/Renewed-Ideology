using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Tools;
using DG.Tweening;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;

public class TitleFragManager : MonoBehaviour
{
    struct RotatePos {
        public bool isReleased;
        public float radius;
        public float angle;
        public Vector3 rotation;
        public Vector2 approachRate;
    }


    public MeshRenderer fragPrefab;
    public Vector2 unitNum;
    public Vector3 startPos;
    public float edgeLength;
    public bool startChanging;
    public int timer;
    
    private MeshRenderer[,] _frags;
    private Vector3[,] _vertexPosition;
    
    private Vector3[,] _tarPosition;
    private Vector3[,] _curRotation;
    
    private RotatePos[,] _rotatePos;
    public Rotator rot;
    
    public Transform[] fragParents;
    
    private void InitFrag() {
        _vertexPosition = new Vector3[(int)unitNum.x + 1, (int)unitNum.y + 1];
        _frags = new MeshRenderer[(int)unitNum.x, (int)unitNum.y];
        _tarPosition = new Vector3[(int)unitNum.x, (int)unitNum.y];
        _rotatePos = new RotatePos[(int)unitNum.x, (int)unitNum.y];
        _curRotation = new Vector3[(int)unitNum.x, (int)unitNum.y];
        
        for (int i = 0; i < unitNum.x + 1; i++) {
            for (int j = 0; j < unitNum.y + 1; j++) {
                _vertexPosition[i, j] = startPos + edgeLength * i * Vector3.right + edgeLength * j * Vector3.up;
            }
        }

        for (int i = 0; i < unitNum.x; i++) {
            for (int j = 0; j < unitNum.y; j++) {
                var pos = (_vertexPosition[i, j] + _vertexPosition[i + 1, j] + _vertexPosition[i + 1, j + 1] + _vertexPosition[i, j + 1]) / 4f;
                pos = pos.SetZ(-5f);
                _frags[i, j] = Instantiate(fragPrefab, transform);
                _frags[i, j].transform.localPosition = pos;
                Vector2 basePoint = new Vector2(i / unitNum.x, j / unitNum.y);

                Vector2[] uv = new Vector2[4];

                uv[0] = basePoint;
                uv[2] = basePoint + Vector2.up / unitNum.y;
                uv[1] = basePoint + Vector2.right / unitNum.x;
                uv[3] = basePoint + Vector2.right / unitNum.x + Vector2.up / unitNum.y;

                _frags[i, j].gameObject.GetComponent<MeshFilter>().mesh.uv = uv;
                _frags[i, j].transform.localScale = new Vector3(edgeLength, edgeLength, 1);
            }
        }
    }

    private void InitRotatePos() {
        for (int i = 0; i < unitNum.x; i++) {
            for (int j = 0; j < unitNum.y; j++) {
                _rotatePos[j, i].radius = Random.Range(4, 8);
                _rotatePos[i,j].rotation = new Vector3(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));

               if (_rotatePos[j, i].radius - 5f <= 0.01f) {
                    _frags[j, i].transform.parent = fragParents[0];
                    _rotatePos[j, i].radius = 3.5f;
                }else if (_rotatePos[j, i].radius - 6f <= 0.01f) {
                    _frags[j, i].transform.parent = fragParents[1];
                    _rotatePos[j, i].radius = 4.5f;
                }else if (_rotatePos[j, i].radius - 7f <= 0.01f) {
                    _frags[j, i].transform.parent = fragParents[2];
                }

                _rotatePos[j, i].angle = (i * unitNum.x + j) / (unitNum.x * unitNum.y - 3000) * 360f;
                _rotatePos[j, i].isReleased = false;
                _rotatePos[j, i].approachRate = new Vector2(Random.Range(16, 32f), Random.Range(16, 32f)); 
            }
        }
    }

    private void Awake() {
        InitFrag();
        InitRotatePos();
        increaseRadius = 0;
    }

    public Vector3 keyPosition;
    public float increaseRadius;

    private void Update() {
        if (Input.anyKeyDown) {
            startChanging = true;
        }

        if (startChanging) {
            rot.rotateMultiplier = 0.1f;
            
            /*keyPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition).SetZ(0f);
            increaseRadius += 0.01f;
            for (int i = 0; i < unitNum.x; i++) {
                for (int j = 0; j < unitNum.y; j++) {
                    if (_rotatePos[j, i].isReleased) continue;
                    var distance = Vector2.Distance(_frags[j, i].transform.position, keyPosition);
                    //Debug.Log(j + " " + i + " " + distance + increaseRadius + _rotatePos[j, i].isReleased);
                    if (distance < increaseRadius) {
                        _rotatePos[j, i].isReleased = true;
                    }
                }
            }*/
            
            UpdateFrags();
        }

        timer++;
    }

    private void UpdateFrags() {
        for (int i = 0; i < unitNum.x; i++) {
            for (int j = 0; j < unitNum.y; j++) {
                //if (_rotatePos[j, i].isReleased) {
                    var x = _rotatePos[j, i].radius * Mathf.Cos(Mathf.Deg2Rad * (_rotatePos[i, j].angle));
                    var y = _rotatePos[j, i].radius * Mathf.Sin(Mathf.Deg2Rad * (_rotatePos[i, j].angle));

                    _tarPosition[j, i] = new Vector3(x, y, 0);

                    _frags[j, i].transform.localPosition =
                        _frags[j, i].transform.localPosition.ApproachValue(_tarPosition[i, j],
                            _rotatePos[j, i].approachRate).SetZ(0f);
                    _curRotation[j, i].ApproachRef(_rotatePos[j, i].rotation, 32f);
                    _frags[j, i].transform.rotation = Quaternion.Euler(_curRotation[j, i]);
               // }
            }
        }
    }
}
