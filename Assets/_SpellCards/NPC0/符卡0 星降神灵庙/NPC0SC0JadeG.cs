using System;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Player;
using _Scripts.Tools;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using Config = _Scripts.EnemyBullet.Config;
using Random = UnityEngine.Random;

public class NPC0SC0JadeG : MonoBehaviour
{
    public DoubleSpeedApproach dotPrefab;
    public DoubleSpeedApproach dotPrefab2;
    
    //public List<DoubleSpeedApproach> bullets;

    public int bulletWays;
    public int currentWays;
    public int spawnInterval;
    
    [FormerlySerializedAs("genOffsetPos")] public Vector2 tarPos;
    public int timer;

    private void Shoot() {
        timer++;
        //transform.rotation = Quaternion.Euler(0, 0, 60f * Mathf.Sin(timer * Mathf.Deg2Rad));
        //transform.rotation = Quaternion.Euler(0, 0, timer * 10f);
        //transform.rotation = Quaternion.Euler(0, 0, timer * 5f);
        //transform.rotation = Quaternion.Euler(0, 0, timer * 3f);
        transform.rotation = Quaternion.Euler(0, 0, timer * 2f);
        if (timer % spawnInterval == 0 && timer <= 200) {
            for (int i = 0; i < bulletWays; i++) {
                if(i / currentWays % currentWays != 0) continue;
                DoubleSpeedApproach bullet = Instantiate(dotPrefab, transform);
                bullet.direction = 360f / bulletWays * i + timer + 90f;
                bullet.transform.position = transform.position + (0.5f + 0.01f * timer) * bullet.direction.Deg2Dir3();
                bullet.GetComponent<NPC0SC0Dot>().centerPoint = transform.position;
                
                bullet = Instantiate(dotPrefab2, transform);
                bullet.direction = 360f / bulletWays * i - timer + 90f;
                bullet.transform.position = transform.position + (0.5f - 0.01f * timer) * bullet.direction.Deg2Dir3();
                bullet.GetComponent<NPC0SC0Dot>().centerPoint = transform.position;
                bullet.GetComponent<NPC0SC0Dot>().isClockwise = true;
                //bullets.Add(bullet);
            }
        }
    }

    private void Start() {
        tarPos = 0.1f * (360 * Random.value).Deg2Dir3();
    }

    private void Update() {
        transform.position = transform.position.ApproachValue(tarPos, 16f);
        if(transform.position.Equal(tarPos,0.1f)) Shoot();
    }
}
