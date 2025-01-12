using System.Linq;
using _Scripts;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Player;
using UnityEngine;



public class CircleSpawner : MonoBehaviour {
    public BulletMovement[] bulletTemplate;
    public BulletMovement[] bullets;
    public Transform posParent;
    public Vector3 center;
    public float radius;
    public int bulletAmount;
    
    [Header("扇形弹片的上下界角度与偏移量")]
    public float degreeOffset;
    public float degreeStart;
    public float degreeEnd;
    
    [Header("子弹方向偏移量")]
    public float dirOffset;

    public bool isSnip;
    
    public bool isSelfShoot;
    public int selfShootInterval;
    
    public int timer;

    public void SetParameters(Transform posParent,Vector3 center, float radius, int bulletAmount, 
                              float degreeOffset, float degreeStart, float degreeEnd,
                              float dirOffset, bool isSnip) {
        this.center = center;
        this.radius = radius;
        this.bulletAmount = bulletAmount;
        this.degreeOffset = degreeOffset;
        this.degreeStart = degreeStart;
        this.degreeEnd = degreeEnd;
        this.isSnip = isSnip;

        //Debug.Log("CircleSpawner SetParams " + this.gameObject.name);
        bullets = new BulletMovement[this.bulletAmount];
    }

    public BulletMovement[] Shoot() {
        if (posParent != null) center = posParent.position;

        // 获取玩家相对于发射中心的角度
        var dirPlayer = Vector2.SignedAngle(Vector2.right, PlayerCtrl.instance.transform.position - center);
        var halfRange = (degreeEnd - degreeStart) / 2;
        var midOffset = halfRange - dirPlayer;
        var newDegStart = degreeStart - midOffset;
        var newDegEnd = degreeEnd - midOffset;

        for (int i = 0; i < bulletAmount; i++) {
            float degree;
            if (isSnip) {
                // 只有在isSnip为true时才需要按自机狙方式瞄准玩家
                if(bulletAmount != 1)
                    degree = newDegStart + i * (newDegEnd - newDegStart) / (bulletAmount - 1);
                else {
                    degree = newDegStart + i * (newDegEnd - newDegStart);
                }
            }
            else {
                // 正常情况下按固定角度发射
                if (bulletAmount != 1) {
                    degree = degreeStart + i * (degreeEnd - degreeStart) / (bulletAmount - 1);
                }
                else {
                    degree = newDegStart + i * (newDegEnd - newDegStart);
                }
            }

            float x = center.x + radius * Mathf.Cos(degree * Mathf.Deg2Rad);
            float y = center.y + radius * Mathf.Sin(degree * Mathf.Deg2Rad);

            Vector3 position = new Vector3(x, y, i / 100f);
            var b = Instantiate(bulletTemplate[0], position, Quaternion.identity);
            b.transform.SetParent(GameManager.Manager.transform);
            b.direction = degree;
            bullets[i] = b;
        }

        return bullets.ToArray();
        //相当于传值而非引用
    }

    
    private void Awake() {
        bullets = new BulletMovement[bulletAmount];
    }
    
    void Update() {
        if(isSelfShoot && timer % selfShootInterval == 0) Shoot();
        timer++;
    }
}
