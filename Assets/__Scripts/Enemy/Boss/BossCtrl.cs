using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.Enemy;
using _Scripts.Item;
using _Scripts.Tools;
using MEC;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;


public enum BossState{
    SpellCard,
    SpellCardAnnounce,
    SpellCardBreak,
    NonSpellCard,
    Interval,
    Dead,
}

public class BossCtrl : MonoBehaviour {
    public int curScNumber = -1;
    public int maxScNumber = 3;
    public GameObject[] attackPatternSet;
    public SCInfo[] spellCardInfos;
    public BulletGenerator enchantMovementRef;
    public TMP_Text scNameText;
    public TMP_Text scNumText;
    [Tooltip("版面下方血条中央展示的剩余符卡数量")]
    public int scNum;

    public ReverseColorEffect defeatedEffect;
    public Damageable damageable;
    public Movement movement;
    public ItemSpawner item;
    public BossAnimator animator;

    public SpellAnnouncement scAnn;
    //public SpellCircle spellCircleTemplate;
    public SpellCircle spellCircle;
    public BossState bossState;
    
    public EnemyArrowCtrl bossArrowCtrl;
    
    public void ActivateBulletGenerator() {
        attackPatternSet[curScNumber].SetActive(true);
    }
    public void DeactivateBulletGenerator() {
        attackPatternSet[curScNumber].SetActive(false);
       }

    public IEnumerator<float> HealthRecharge() {
        var maxHealth = damageable.maxHealth;
        var curHealth = damageable.curHealth;
        //damageable.SetInvincibleTime(180);
        while (curHealth < maxHealth) {
            damageable.SetInvincibleTime(10);
            curHealth = curHealth.ApproachValue(maxHealth, 20f,100f);
            damageable.curHealth = curHealth;
            var time = Time.frameCount;
            yield return Timing.WaitForOneFrame;
        }
        damageable.SetInvincibleTime(120);
    }

    public IEnumerator<float> HealthToZero() {
        var curHealth = damageable.curHealth;
        while (curHealth > 0) {
            curHealth = curHealth.ApproachValue(0, 20f,100f);
            damageable.curHealth = curHealth;
            yield return Timing.WaitForOneFrame;
        }
    }
    
    public IEnumerator<float> TimeRecharge() {
        var maxTime = damageable.maxTime;
        var curTime = damageable.curTime;
        while (curTime < maxTime) {
            curTime = curTime.ApproachValue(maxTime, 20f,0.1f);
            damageable.curTime = curTime;
            yield return Timing.WaitForOneFrame;
        }
    }
    
    public IEnumerator<float> TimeToZero() {
        var curTime = damageable.curTime;
        while (curTime > 0f) {
            curTime = curTime.ApproachValue(0f, 20f,0.1f);
            damageable.curTime = curTime;
            yield return Timing.WaitForOneFrame;
        }
    }

    public int CountSpellCard() {
        int num = 0;
        foreach (var sc in spellCardInfos) {
            if (sc.isSpellCard) num++;
        }
        scNum = num;
        return num;
    }
    private void Awake() {
        scNumText.text = CountSpellCard().ToString();
        ChangeState(BossState.Interval);
    }
    
    private void ChangeState(BossState state) {
        switch (state) {
            case BossState.Interval:
                damageable.inBattle = false;
                GameManager.Manager.StartEraseBullets(transform.position);
                
                var list = BulletGenerator.subBulletGenerators;
                foreach (var sub in list) Destroy(sub);
                list.Clear();
                
                scAnn.ResetAnnounce();
                
                // if boss has remaining sc or ncs
                if (curScNumber + 1 < maxScNumber) {
                    curScNumber++;
                    
                    damageable.maxTime = spellCardInfos[curScNumber].maxTime;
                    damageable.maxHealth = spellCardInfos[curScNumber].maxHealth;

                    scNameText.text = spellCardInfos[curScNumber].spellName + "「" +
                                      spellCardInfos[curScNumber].cardName + "」";
                        //spellCardInfos[curScNumber].spellCardName;
                    
                    item.RefreshItem(spellCardInfos[curScNumber].itemSequence);
                    
                    enchantMovementRef = attackPatternSet[curScNumber].GetComponent<BulletGenerator>();
                    movement.stayFrames = enchantMovementRef.waveFrameInterval;
                    if(spellCardInfos[curScNumber].hasInitPos) {
                        movement.hasInitPos = true;
                        movement.initPos = spellCardInfos[curScNumber].initPos;
                        movement.initPosFinished = false;
                        movement.hasFixedPos = spellCardInfos[curScNumber].hasFixedPos;
                    }
                    
                    Timing.RunCoroutine(HealthRecharge().CancelWith(gameObject));
                    Timing.RunCoroutine(TimeRecharge().CancelWith(gameObject));
                }//let boss die
                else {
                    //var effect = Instantiate(defeatedEffect, new Vector3(0, 0, -5), Quaternion.identity);
                    //effect.basePos = transform.position;
                    //Timing.RunCoroutine(Rest(30));
                    ChangeState(BossState.Dead);
                } 
                break;
            case BossState.SpellCardAnnounce:
                scAnn.StartAnnouncing();
                spellCircle.gameObject.SetActive(true);
                spellCircle.ResetCircle();
                break;
           
            case BossState.SpellCard:
                damageable.inBattle = true;
                break;
            case BossState.NonSpellCard:
                damageable.inBattle = true;
                break;
            case BossState.SpellCardBreak:
                spellCircle.SetState(SpellCircle.SpellCircleState.Shrink);
                scNum--;
                scNumText.text = scNum.ToString();
                break;
            case BossState.Dead:
                GameManager.Manager.StartEraseBullets(transform.position);
                GameManager.Manager.reverseColorCtrl.StartReverseColorEffectAtCenter(transform.position);
                
                Damageable.damageableSet.Remove(damageable);
                
                DistortCtrl.Set(0f,0,Vector3.zero);
                
                bossArrowCtrl.isFunctioning = false;
                
                Timing.RunCoroutine(HealthToZero());
                Timing.RunCoroutine(TimeToZero());
                
                Destroy(gameObject);
                
                
                break;
            default: break;
        }

        bossState = state;
    }
    private void Update() {
        if (bossState != BossState.Dead) {
            DistortCtrl.Set(0.8f, 400, transform.position);
        }

        switch (bossState) {
            case BossState.Interval:
                
                if(spellCardInfos[curScNumber].hasInitPos)
                    if (!movement.initPosFinished) 
                        return;
                if (damageable.curHealth >= damageable.maxHealth  &&
                    damageable.curTime >= damageable.maxTime &&
                    GameManager.Manager.IsEraseFinished()) {
                    if (spellCardInfos[curScNumber].isSpellCard) {
                        ChangeState(BossState.SpellCardAnnounce);
                    }
                    else {
                        ChangeState(BossState.NonSpellCard);
                        ActivateBulletGenerator();
                    }
                }
                
                break;
            case BossState.SpellCardAnnounce:
                if (scAnn.isAnnounceFinished) {
                    ChangeState(BossState.SpellCard);
                    ActivateBulletGenerator();
                }

                break;
            case BossState.SpellCard:
                animator.IsEnchanting = enchantMovementRef.isEnchanting;
                if (damageable.curHealth <= 0 || damageable.curTime <= 0) {
                    ChangeState(BossState.SpellCardBreak);
                    item.setItemDropEnable = !(damageable.curHealth > 0.001f);
                    DeactivateBulletGenerator();
                }

                break;
            case BossState.SpellCardBreak:
                animator.IsEnchanting = false;
                scAnn.SpellBreak();
                item.CreateItem();
                Timing.KillCoroutines("Shoot");
                ChangeState(BossState.Interval);
                
                break;
            case BossState.NonSpellCard:
                animator.IsEnchanting = enchantMovementRef.isEnchanting;
                if (damageable.curHealth <= 0 || damageable.curTime <= 0) {
                    DeactivateBulletGenerator();
                    Timing.KillCoroutines("Shoot");
                    item.setItemDropEnable = !(damageable.curHealth > 0.001f);
                    item.CreateItem();
                    ChangeState(BossState.Interval);
                }

                break;
            case BossState.Dead:
                break;
        }
    }

}
