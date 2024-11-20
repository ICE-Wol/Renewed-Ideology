using System.Collections;
using System.Collections.Generic;
using _Scripts.Enemy;
using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using UnityEngine;
using Config = _Scripts.EnemyBullet.Config;

public class EnemyBulletAPIs : MonoBehaviour {

    public SpriteRenderer spriteRenderer;
    public Config config;
    public State state;
    public DoubleSpeedApproach movement;

}
