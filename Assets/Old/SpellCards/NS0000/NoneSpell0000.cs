using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Tools;
using UnityEngine;


public class NoneSpell0000 : MonoBehaviour {
    public UniformLinear bullet;
    public GameObject setterPrefab;
    
    public GameObject[] setterObjects;
    public Vector3[] setterPoints;
    public int setterNum;
    public float setterRadius;
    public float setterStartDegree;

    public bool reverseTag;
    
    public float radius;
    public float degree;
    public float curDegree;
    
    public int timer;

    private void Start() {
        degree = 0;
        timer = 0;

        setterPoints = new Vector3[setterNum];
        for (int i = 0; i < setterNum; i++) {
            setterPoints[i] = setterRadius * (360f / setterNum * i + setterStartDegree).Deg2Dir3();
        }

        setterObjects = new GameObject[setterNum];
        for (int i = 0; i < setterNum; i++) {
            var setter = Instantiate(setterPrefab, setterPoints[i], Quaternion.identity);
            setter.transform.parent = this.transform;
            setterObjects[i] = setter;
        }
    }
    private void Update() {
        for (int i = 0; i < setterNum; i++) {
            if(timer % 10 == 0 && timer <= 360) {
                degree += 10f * (reverseTag ? -1 : 1);
                var pos = setterPoints[i] + radius * degree.Deg2Dir3();
                var rot = degree + 30f * (reverseTag ? -1 : 1);
                var bullet = Instantiate(this.bullet, pos, Quaternion.Euler(0, 0, rot));
                bullet.transform.parent = setterObjects[i].transform;
                bullet.GetComponent<UniformLinear>().direction = rot;
                bullet.transform.rotation = Quaternion.Euler(0, 0, rot);
            }
        }

        if (timer == 360) {
            degree = 0;
        }

        for (int i = 0; i < setterNum; i++) {
            if (timer > 360) {
                curDegree.ApproachRef(degree, 32f);
                degree += 0.1f * (reverseTag ? -1 : 1);
                //setterObjects[i].transform.position =
                //    setterRadius * (degree + 360f / setterNum * i + setterStartDegree).Deg2Dir3();
                foreach (Transform child in setterObjects[i].transform) {
                    var b = child.GetComponent<UniformLinear>();
                    b.speed = -1;
                }
            }
        }


        timer++;
    }
}
