using System;
using _Scripts.EnemyBullet;
using UnityEngine;

[Serializable]
public class BulletRuntimeState
{
    public bool isAlive = true;
    public int id;
    public int layer;
    public int parentID;
    public BulletType type;
    public float checkRadius;
    public Vector2 position;

    public float speed;
    public float direction;
    public float rotation;
    public Color color;
    public float alpha;

    public Sprite sprite;

    public Vector2 scale;

    /// <summary>
    /// 是否被中途消除
    /// </summary>
    public bool isErased;
}