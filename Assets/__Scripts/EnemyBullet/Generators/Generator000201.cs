using _Scripts.EnemyBullet;
using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Player;
using _Scripts.Tools;
using Unity.VisualScripting;
using UnityEngine;
using Config = _Scripts.EnemyBullet.Config;
using Random = UnityEngine.Random;

public class Generator000201 : MonoBehaviour {
    public GameObject[] bulletList;
    public SpriteRenderer spriteRenderer;
    public Sprite spriteBounced;

    public Vector3 targetPos;
    public float targetDir;
    public float playerDir;
    public float targetSpeed;

    public bool hasCollided;
    public Vector3 collidePos;
    
    public bool hasArrived;

    public int[,] randArray;
    public int initRandDir;
    
    public int timer;
    public int destroyTimer;

    public int intervals;

    public int generated;

    void Start() {
        intervals = 10;
        randArray = new int[,]
        {
            {3, 7, 1, 9, 2, 8, 4, 6, 5, 0},
            {6, 9, 2, 4, 1, 0, 8, 7, 3, 5},
            {2, 8, 6, 1, 4, 7, 5, 0, 9, 3},
            {5, 2, 8, 3, 7, 4, 1, 9, 6, 0},
            {0, 5, 4, 6, 9, 3, 8, 2, 7, 1},
            {7, 3, 9, 6, 1, 0, 2, 8, 5, 4},
            {9, 1, 7, 8, 0, 3, 2, 6, 4, 5},
            {4, 6, 0, 7, 9, 2, 1, 8, 3, 5},
            {1, 4, 5, 2, 7, 8, 3, 0, 6, 9},
            {8, 2, 0, 3, 6, 5, 4, 9, 7, 1}
        };

        initRandDir = (int)(UnityEngine.Random.value * 10);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (!hasArrived) {
            //transform.position = transform.position.ApproachValue(targetPos, 32 * Vector3.one);
            //hasArrived = transform.position.Equal(targetPos, 0.05f);
            if(!hasCollided) transform.position += targetSpeed * Time.fixedDeltaTime * targetDir.Deg2Dir3();
            else transform.position += targetSpeed * Time.fixedDeltaTime * targetDir.Deg2Dir3();
            targetSpeed.ApproachRef(0f, 32f);
            hasArrived = targetSpeed.Equal(0f, 0.05f);

            if (!hasCollided) {
                if (transform.position.x >= 3.9f || transform.position.x <= -3.9f || 
                    transform.position.y >= 4.4f || transform.position.y <= -4.2f) {
                    collidePos = transform.position;
                    hasCollided = true;
                    spriteRenderer.sprite = spriteBounced;
                    targetSpeed += 1f;
                    if (transform.position.x >= 3.9f) targetDir -= (targetDir - 270f) * 2f;
                    if (transform.position.x <= -3.9f) targetDir += (270f - targetDir) * 2f;
                    if (transform.position.y >= 4.4f) targetDir = 360f - targetDir;
                }
            }
        }
        else if (generated <= 9) {
            if (timer % intervals == 0) {
                if (generated == 0)
                    playerDir = Vector2.SignedAngle(Vector2.right,
                        (PlayerCtrl.instance.transform.position - transform.position));
                var dir = (hasCollided ? playerDir : targetDir) + (float)360 / 10 * randArray[initRandDir, generated];
                var pos = transform.position + 0.35f * dir.Deg2Dir3();
                for (int i = 0; i < 14 - randArray[initRandDir, generated]; i++) {
                    //var k = randArray[initRandDir, generated] % 2;
                    var bullet = Instantiate(bulletList[hasCollided ? 1 : 0], pos, Quaternion.Euler(0, 0, dir));

                    var comp = bullet.GetComponent<DoubleSpeedApproach>();
                    comp.speed = 0;
                    comp.endSpeed = 0.3f + 0.2f * i;
                    comp.direction = dir;
                    comp.startFrame = 0;
                    comp.approachRate = 32f;
                }
                generated++;
            }
            timer++;
        }
        if (generated > 9) {
            destroyTimer++;
            if(destroyTimer >= 200)
                 Destroy(gameObject);
        }
    }

    private void OnDestroy() {
        var beginDir = 0f;
        for (int k = 0; k < 4; k++) {
            var pos = transform.position; //+ 1.2f * Random.value * (360 * Random.value).Deg2Dir3();
            var radius = 0.3f + k / 8f;
            beginDir += 0.618f * 360;//Random.value * 360;
            for (int i = 0; i < 12; i++) {
                var dir = beginDir + i * 360 / 12f;
                var innerPos = pos + radius * dir.Deg2Dir3();
                var b = Instantiate(bulletList[hasCollided ? 0 : 1], innerPos, Quaternion.Euler(0, 0, dir));
                b.GetComponent<Config>().type = BulletType.JadeR;
                
                var comp = b.GetComponent<DoubleSpeedApproach>();
                comp.speed = 0.2f;
                comp.endSpeed = 2f;
                comp.direction = dir;
                comp.startFrame = 240;
                comp.approachRate = 128f;
            }
        }
    }
}
