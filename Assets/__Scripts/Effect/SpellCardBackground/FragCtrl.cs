using System;
using _Scripts.Tools;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class FragCtrl : MonoBehaviour {
    public MeshRenderer meshRenderer;
    public TMP_Text testText;
    public FragManager.SpellBreakEffect spellBreakMethod;
    
    /// <summary>
    /// flag that indicates whether the fragment is breaking
    /// true refer to breaking
    /// </summary>
    private bool _breakFlag;

    private Vector3 _target;
    private Vector3 _rot;

    private float _scale;

    private float _speed;
    private float _finalSpeed;

    private float _alpha = 1f;
    
    //SpellBreakEffect.CircularBreak=========
    private Vector2 _dir;
    private float _sqrDis;
    public bool trigger;

    public int number;
    //=======================================
    //Absorb======================
    public Vector3 playerPos;
    //===========================


    private static FragCtrl record;
    private void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
        //record = this;
    }

    public void StartFloat() {
        _breakFlag = true;
        _target = new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));
        _rot = Vector3.zero;
        _speed = 0f;
        _scale = 1f;
        _finalSpeed = Random.Range(0.03f, 0.13f);
    }
    
    public void ResetSelf() {
        _breakFlag = false;
        InitSelf();
    }

    public void InitSelf() {
        StartFloat();
        _alpha = 1f;
        trigger = false;
        var vec = transform.position - _Scripts.Player.PlayerCtrl.instance.transform.position;
        _sqrDis = vec.sqrMagnitude;
        _dir = vec.normalized;
        if (spellBreakMethod == FragManager.SpellBreakEffect.Absorb) {
            _dir *= -1;
        }
        number = (int)Mathf.Floor(_sqrDis);
        //testText.text = _sqrDis.ToString();
    }

    private void Update() {
        if (_breakFlag) {
            meshRenderer.material.color = meshRenderer.material.color.SetAlpha(_alpha); 
            _alpha -= 0.015f;
            switch (spellBreakMethod) {
                case FragManager.SpellBreakEffect.FloatUp:
                    _speed.ApproachRef(_finalSpeed, 128f);
                    _rot.ApproachRef( _target, 128f * Vector3.one);
                    _scale.ApproachRef(0f, 256f);
                    transform.position += _speed * Vector3.up;
                    //if(this == record) Debug.LogError("Update:SpellBreak " + transform.position);
                    transform.rotation = Quaternion.Euler(_rot);
                    transform.localScale = _scale * Vector3.one;
                    break;
                
                case FragManager.SpellBreakEffect.CircularBreak:
                    if (trigger) {
                        _speed.ApproachRef(_finalSpeed, 128f);
                        _rot.ApproachRef( _target, 128f * Vector3.one);
                        _scale.ApproachRef(0f, 32f);
                        
                        if(_scale.Equal(0.1f)) Destroy(gameObject);
                        
                        transform.position += _speed * (Vector3)_dir;
                        transform.rotation = Quaternion.Euler(_rot);
                        transform.localScale = _scale * Vector3.one;
                    }
                    break;
                case FragManager.SpellBreakEffect.Absorb:
                    if (trigger) {
                        _speed.ApproachRef(_finalSpeed, 128f);
                        _rot.ApproachRef( _target, 128f * Vector3.one);
                        _scale.ApproachRef(0f, 16f);
                        
                        if(_scale.Equal(0.2f)) Destroy(gameObject);
                        
                        playerPos = _Scripts.Player.PlayerCtrl.instance.transform.position;
                        //transform.position = transform.position.ApproachValue(playerPos, 64f * Vector3.one);
                        var dir = (playerPos - transform.position).normalized;
                        transform.position += _speed * dir;
                        transform.rotation = Quaternion.Euler(_rot);
                        transform.localScale = _scale * Vector3.one;
                    }
                    break;
            }
            
            
        }
    }
}
