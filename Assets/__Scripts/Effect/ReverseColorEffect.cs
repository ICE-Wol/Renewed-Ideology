using UnityEngine;

public class ReverseColorEffect : MonoBehaviour
{
    public MeshRenderer reverseColorEffectPrefab;
    public GameObject reverseColorEffectCamera;

    public Vector3 basePos;
    private readonly Vector3[] _posOffSetAndRadius = new Vector3[6] {
        new(0, 0, -1),
        new(1, 0, -1),
        new(-1, 0, -1),
        new(0, 1, -1),
        new(0, -1, -1),
        new(0, 0, -1),
    };
    private void InitEffect() {
        reverseColorEffectCamera.SetActive(true);
        for(var i = 0; i < 6; i++) {
            reverseColorEffectPrefab.material.SetVector("_Circle" + i, _posOffSetAndRadius[i] + basePos);
            TriggerEffect(i);
        }
    }
    
    private void TriggerEffect(int triggerNum) {
        _posOffSetAndRadius[triggerNum].z = 0f;
        reverseColorEffectPrefab.material.SetVector("_Circle" + triggerNum, _posOffSetAndRadius[triggerNum] + basePos);
    }

    
    private void DestroyEffect() {
        reverseColorEffectCamera.SetActive(false);
        //按位取反. mask = bitmask 
        Destroy(reverseColorEffectPrefab.material);
        Destroy(gameObject);
    }

    private void OnEnable() {
        Camera.main.cullingMask = LayerMask.GetMask("RevertGameScene");
    }

    private void OnDisable() {
        Camera.main.cullingMask = ~LayerMask.GetMask("RevertGameScene");
    }

    private void Start() {
        GetComponent<MeshRenderer>().sortingLayerName = "TopEffect";
        InitEffect();
    }

    private void Update() {
        for(int i = 0; i < 6; i++) {
            if (_posOffSetAndRadius[i].z >= 0) {
                switch (i) {
                        case 0:
                            if(_posOffSetAndRadius[i].z >= 0.05f)
                                _posOffSetAndRadius[i].z += 0.06f;
                            else 
                                _posOffSetAndRadius[i].z += 0.5f;
                            break;
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            if(_posOffSetAndRadius[i].z >= 0.05f)
                                _posOffSetAndRadius[i].z += 0.1f;
                            else 
                                _posOffSetAndRadius[i].z += 0.02f;

                            break;
                        case 5:
                            if(_posOffSetAndRadius[i].z >= 0.05f)
                                _posOffSetAndRadius[i].z += 0.08f;
                            else 
                                _posOffSetAndRadius[i].z += 0.02f;

                            break;
                }
                
                reverseColorEffectPrefab.material.SetVector("_Circle" + i, _posOffSetAndRadius[i] + basePos);
            }
        }


        bool stopFlag = true;
        for (int i = 0; i < 6; i++) {
            if (_posOffSetAndRadius[i].z <= 15f) {
                stopFlag = false;
                break;
            }
        }
        if (stopFlag) DestroyEffect();

    }
}
