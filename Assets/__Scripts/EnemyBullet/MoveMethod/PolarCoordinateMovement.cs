using _Scripts.Enemy;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using UnityEngine;

public class PolarCoordinateMovement : BulletMovement
{
    [Header("基本参数")]
    public Vector3 center;
    public float radius;
    public float angle;
    
    [Header("半径参数")]
    public float radSpd;
    public float tarRadSpd;
    public float radSpdRate;
    
    [Header("角度参数")]
    public float angSpd;
    public float tarAngSpd;
    public float angSpdRate;
    
    public override void Movement(Transform transform) {
        radSpd.ApproachRef(tarRadSpd, radSpdRate);
        angSpd.ApproachRef(tarAngSpd, angSpdRate);
        radius += radSpd * Time.fixedDeltaTime;
        angle += angSpd * Time.fixedDeltaTime;
        var newPos = center + radius * angle.Deg2Dir3();
        direction = Calc.GetDirection(transform.position, newPos);
        transform.position = newPos;
    }
}
