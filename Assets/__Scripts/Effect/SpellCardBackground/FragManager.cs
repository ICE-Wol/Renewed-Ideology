using _Scripts.Tools;
using UnityEngine;

public class FragManager : MonoBehaviour {
    public enum SpellBreakEffect {
        FloatUp,
        CircularBreak,
        Absorb,
    }

    public SpellBreakEffect spellBreakMethod;
    public float edgeLength = 1f;

    public SpellBreakCamera cam;
    public SpriteRenderer pointPrefab;
    public FragCtrl FragPrefab;
    public Transform fragFatherObject;
    public Material fragMaterial;
    public GameObject SCBGObject;
    public Renderer spellBgFull;
    
    private FragCtrl[,] _frags;
    private SpriteRenderer[,] _points;
    private Vector3[,] _position;

    private int _timer = 0;
    private bool _breakFlag = false;
    private bool _initFlag = false;

    private readonly int[] _randomOrder = {
        0, 14, 67, 2, 48, 50, 71, 56, 11, 82, 40, 33, 62, 96, 25, 1, 84, 57, 55, 69, 97, 21, 49, 5, 23, 72, 31, 73, 44,
        27,
        85, 13, 80, 16, 74, 61, 34, 18, 59, 0, 46, 87, 3, 76, 93, 89, 15, 6, 92, 43, 12, 39, 42, 70, 26, 95, 36, 99,
        28, 86, 68, 4, 9, 10, 88, 38, 81, 22, 75, 41, 7, 35, 51, 94, 29, 19, 37, 64, 8, 17, 58, 98, 60, 65, 24, 20, 90,
        53, 30, 79, 78, 63, 66, 45, 32, 52, 77, 54, 47, 83, 91
    };
    
    private void GenerateMarkPointForTest() {
        _points = new SpriteRenderer[11, 11];
        for (int i = 0; i < 11; i++) {
            for (int j = 0; j < 11; j++) {
                var pos = transform.position + edgeLength * i * Vector3.right + edgeLength * j * Vector3.up;
                _points[i, j] = Instantiate(pointPrefab, pos, Quaternion.Euler(0f, 0f, 0f));
            }
        }
    }

    public Vector2 screenSize;
    public Vector2Int unitNum;
    
    public void InitFrag() {
        if (_initFlag) return;
            /*screenSize = new Vector2(1920, 1080);
        unitNum.x = (int)Mathf.Floor(screenSize.x / 100);
        unitNum.y = (int)Mathf.Floor(screenSize.y / 100);*/
//       19 * 10

        screenSize = new Vector2(1080, 1080);
        unitNum.x = (int)Mathf.Floor(screenSize.x / 100);
        unitNum.y = (int)Mathf.Floor(screenSize.y / 100);
        //10 * 10
        
        _frags = new FragCtrl[unitNum.x, unitNum.y];
        _position = new Vector3[unitNum.x + 1, unitNum.y + 1];

        for (int i = 0; i < unitNum.x + 1; i++) {
            for (int j = 0; j < unitNum.y + 1; j++) {
                _position[i, j] = edgeLength * i * Vector3.right + edgeLength * j * Vector3.up;
                //端点位置
            }
        }

        for (int i = 0; i < unitNum.x; i++) {
            for (int j = 0; j < unitNum.y; j++) {
                var pos = (_position[i, j] + _position[i + 1, j] + _position[i + 1, j + 1] + _position[i, j + 1]) / 4f;
                //根据四个端点位置计算中心位置
                pos = pos.SetZ(4f);
                _frags[i, j] = Instantiate(FragPrefab, fragFatherObject);
                _frags[i, j].transform.localPosition = pos;
                
                Vector2 basePoint = new Vector2(i / 10f, j / 10f);
                //uv坐标的左下角
                Vector2[] uv = new Vector2[4];

                uv[0] = basePoint;
                uv[2] = basePoint + Vector2.up / 10f;
                uv[1] = basePoint + Vector2.right / 10f;
                uv[3] = basePoint + Vector2.right / 10f + Vector2.up / 10f;
                //uv坐标的四个点
                
                _frags[i, j].gameObject.GetComponent<MeshFilter>().mesh.uv = uv;
                _frags[i, j].meshRenderer.material = fragMaterial;
                _frags[i, j].spellBreakMethod = spellBreakMethod;
                //_frags[i, j].testText.text = i + "+" + j;
                
            }
        }

        _initFlag = true;
    }
    
    /// <summary>
    /// 激活碎片组的显示,并且将破碎标记置为true
    /// </summary>
    public void EnableFrag() {
        fragFatherObject.gameObject.SetActive(true);
        _breakFlag = true;
    }

    public void ResetFrag() {
        _breakFlag = false;
        for (int i = 0; i < unitNum.x; i++) {
            for (int j = 0; j < unitNum.y; j++) {
                var pos = (_position[i, j] + _position[i + 1, j] + _position[i + 1, j + 1] + _position[i, j + 1]) / 4f;
                pos = pos.SetZ(4f);
                _frags[i, j].transform.localPosition = pos;
                _frags[i, j].transform.rotation = Quaternion.Euler(0, 0, 0);
                _frags[i, j].transform.localScale = Vector3.one;
                _frags[i, j].ResetSelf();
            }
        }
    }

    public void DestroyFrag() {
        for (int i = 0; i < unitNum.x; i++) {
            for (int j = 0; j < unitNum.y; j++) {
               Destroy(_frags[i, j].gameObject);
            }
        }
    }

    public void DisableFrag() {
        _breakFlag = false;
        for (int i = 0; i < 10; i++) {
            for (int j = 0; j < 10; j++) {
                Debug.Log(_frags[i,j]);
                _frags[i, j].transform.localPosition = _position[i, j];
                _frags[i, j].transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        fragFatherObject.gameObject.SetActive(false);
        _timer = 0;
    }

    public void FloatFrag() {
        for (int i = 0; i <= 3; i++) {
            int num = _randomOrder[_timer + 25 * i];
            int x = num % 10;
            int y = num / 10;
            //Debug.Log("fragsxy" + x + " " + y + " " + (_frags != null));
            _frags[x, y].StartFloat();
            Debug.Break();
            //if (100 + num < 190) {
            //    _frags[x + 9, y].StartFloat();
            //}
        }
        spellBgFull.gameObject.SetActive(false);
    }
    private void Update() {
        if (_breakFlag && !_initFlag) {
            InitFrag();
            if (spellBreakMethod != SpellBreakEffect.FloatUp) {
                for (int i = 0; i < 10; i++) {
                    for (int j = 0; j < 10; j++) {
                        _frags[i, j].InitSelf();
                    }
                }
            }
            _initFlag = true;
        }
        
        if (_breakFlag) {
            switch (spellBreakMethod) {
                case SpellBreakEffect.FloatUp:
                    if (_timer < 26) {
                        FloatFrag();
                    } else if (_timer > 180) {
                        //DisableFrag();
                        Destroy(gameObject);
                    }
                    break;
                case SpellBreakEffect.CircularBreak:
                    for (int i = 0; i < 10; i++) {
                        for (int j = 0; j < 10; j++) {
                            if (_timer / 2 > _frags[i, j].number) {
                                _frags[i, j].trigger = true;
                            }
                        }
                    }
                    break;
                case SpellBreakEffect.Absorb:
                    for (int i = 0; i < 10; i++) {
                        for (int j = 0; j < 10; j++) {
                            if (_timer > _frags[i, j].number) {
                                _frags[i, j].trigger = true;
                            }
                        }
                    }
                    break;
            }
            _timer++;
        }
    }
}

