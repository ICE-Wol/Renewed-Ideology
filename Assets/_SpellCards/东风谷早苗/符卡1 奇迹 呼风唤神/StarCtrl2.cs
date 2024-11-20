using System;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Player;
using _Scripts.Tools;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using Config = _Scripts.EnemyBullet.Config;

public class StarCtrl2 : MonoBehaviour
{
    public PointLaserManager pointLaserManager;
    public DoubleSpeedApproach jadeSBulletPrefab;
    public DoubleSpeedApproach pointBulletPrefab;
    public DoubleSpeedApproach riceBulletPrefab;
    public DoubleSpeedApproach iceBulletPrefab;
    
    public float curRadius;
    public float tarRadius = 1f;
    
    public float rotation;
    public float rotSpeed = 1f;
    
    public int pNum = 5;

    public Vector3 tarPos;

    public float innerBulletAngle;

    public List<State> innerBulletList;
    public List<State> innerBulletListCopy;

    public int timer;
    private void Start() {
        tarPos = PlayerCtrl.Player.transform.position;
        //tarPos = new Vector3(0, 0, 0);
        innerBulletAngle = 240f;
        pointLaserManager.SetState(LaserState.Inactive);
        innerBulletList = new List<State>();
        innerBulletListCopy = new List<State>();
    }

    private bool CheckBulletInCircle(Vector2 point, float rad, State bullet) {
        return (point - (Vector2)bullet.transform.position).sqrMagnitude < rad * rad;
    }

    private void Update() {
        innerBulletListCopy = new List<State>();
        for (int i = 0; i < innerBulletList.Count; i++) {
            var b = innerBulletList[i];
            innerBulletListCopy.Add(b);
        }

        for(int i = 0; i < innerBulletListCopy.Count; i++) {
            var b = innerBulletListCopy[i];
            if(b == null) continue;//prevent erase bullet to null by player hit 
            if(!CheckBulletInCircle(tarPos, tarRadius, b)) {
                Debug.Log("Destroy");
                innerBulletList.Remove(b);
                b.SetState(EBulletStates.Destroying);
            }
        }
        
        curRadius.ApproachRef(tarRadius, 32f);
        if(tarRadius < 3f)tarRadius += 0.01f;
        else if (tarRadius > 3f) tarRadius += 0.001f;
        else if (tarRadius > 3.5f) {
            tarRadius = 3.5f;
        }

        rotation += rotSpeed;
        
        innerBulletAngle -= 0.1f;
        
        transform.position = transform.position.ApproachValue(tarPos, 32f);
        if (timer % 2 == 0) {
            for (int i = 0; i < pNum; i++) {
                var angle = i * 360f / pNum * 2f + 90f + rotation;
                var rad = angle * Mathf.Deg2Rad;
                var x = transform.position.x + curRadius * Mathf.Cos(rad);
                var y = transform.position.y + curRadius * Mathf.Sin(rad);
                pointLaserManager.laserPoints[i].transform.position = new Vector3(x, y, 0);
                
                if (transform.position.Equal(tarPos, 0.1f)) {
                    if (timer % 4 == 0) {
                        var b = Instantiate(jadeSBulletPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        b.endSpeed = 3f + Mathf.Sin(10 * timer * Mathf.Deg2Rad);
                        b.direction = angle; //180f + angle;
                    }
                    else {
                        var b = Instantiate(pointBulletPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        b.endSpeed = 2f + Mathf.Sin(10 * timer * Mathf.Deg2Rad);
                        b.direction = angle; //180f + angle;
                    }

                    if (timer % 32 <= 16) {
                        var b = Instantiate(riceBulletPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        b.direction = innerBulletAngle + 180f;// + angle;
                        var b2 = Instantiate(riceBulletPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        b2.direction = innerBulletAngle;// + angle;
                        innerBulletList.Add(b.GetComponent<State>());
                        innerBulletList.Add(b2.GetComponent<State>());
                        
                        
                        if (innerBulletAngle < 180f) {
                            pointLaserManager.SetState(LaserState.WarnLine);
                        }
                        if(innerBulletAngle < 120f) {
                            pointLaserManager.SetState(LaserState.Active);
                            if (timer % 64 == 0) {
                                var b3 = Instantiate(iceBulletPrefab, new Vector3(x, y, 0), Quaternion.identity);
                                b3.direction = Vector2.SignedAngle(Vector2.right, PlayerCtrl.Player.transform.position - new Vector3(x, y, 0));
                            }
                        }
                    }
                    

                }
            }
            
        }

        timer++;

    }
}
