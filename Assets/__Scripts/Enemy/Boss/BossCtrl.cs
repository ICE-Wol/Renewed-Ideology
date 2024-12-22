using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using _Scripts;
using _Scripts.Enemy;
using _Scripts.Item;
using _Scripts.Player;
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
    
    [Header("符卡计时器")]
    public int frameTimer;
    public float actualTimer;
    public bool isTimerActivated;

    public SCTimeInfoCtrl scTimeInfoCtrl;

    [Header("收取指示变量")] public bool hasBonus;
    public BonusBannerCtrl bonusBannerCtrl;
    public int bonusPoints;
    public int bonusPointsReducePerFrame = 10;
    
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
    
    public void StartTimer() {
        isTimerActivated = true;
    }

    public void RunTimer() {
        if (isTimerActivated) {
            frameTimer++;
            actualTimer += Time.deltaTime;
        }
    }

    public void EndTimer() {
        scTimeInfoCtrl.breakTime.text = (frameTimer / 60f).ToString("F2") + 's';
        scTimeInfoCtrl.actualTime.text = actualTimer.ToString("F2") + 's';
        scTimeInfoCtrl.AppearAll();
        frameTimer = 0;
        actualTimer = 0;
        isTimerActivated = false;
    }
    
    private void Awake() {
        scNumText.text = CountSpellCard().ToString();
        ChangeState(BossState.Interval);
    }

    private void OnEnable() {
        GameManager.Manager.curBoss = this;
    }
    
    private void ChangeState(BossState state) {
        switch (state) {
            case BossState.Interval:
                hasBonus = true;
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
                    ChangeState(BossState.Dead);
                } 
                break;
            case BossState.SpellCardAnnounce:
                StartTimer();
                bonusPoints = spellCardInfos[curScNumber].maxBonusPoints;
                scAnn.StartAnnouncing();
                spellCircle.gameObject.SetActive(true);
                spellCircle.ResetCircle();
                AudioManager.Manager.PlaySound(AudioNames.SeBossExplode);
                break;
           
            case BossState.SpellCard:
                damageable.inBattle = true;
                break;
            case BossState.NonSpellCard:
                damageable.inBattle = true;
                break;
            case BossState.SpellCardBreak:
                EndTimer();
                bonusBannerCtrl.ActivateBonusState(true,hasBonus,bonusPoints);
                if (hasBonus)
                    PlayerCtrl.Player.state.score += bonusPoints;
                spellCircle.SetState(SpellCircle.SpellCircleState.Shrink);
                scNum--;
                scNumText.text = scNum.ToString();
                AudioManager.Manager.PlaySound(AudioNames.SeShootTan);
                break;
            case BossState.Dead:
                //必定经过interval来到dead，所以不需要再次调用erase
                //GameManager.Manager.StartEraseBullets(PlayerCtrl.Player.transform.position);//transform.position);
                GameManager.Manager.reverseColorCtrl.StartReverseColorEffectAtCenter(transform.position);
                
                AudioManager.Manager.PlaySound(AudioNames.SeBossExplode);
                
                Damageable.damageableSet.Remove(damageable);
                
                DistortCtrl.Set(0f,0,Vector3.zero);
                
                bossArrowCtrl.isFunctioning = false;
                
                Timing.RunCoroutine(HealthToZero());
                Timing.RunCoroutine(TimeToZero());
                
                DestroyImmediate(gameObject);
                
                break;
            default: break;
        }
        //在change state里调用change state
        //会导致全局状态被递归后的状态覆盖
        //为避免屎山崩塌，故在此打补丁
        //事实上不是这里的问题，而是消弹圈是全局的问题
        //把boss的间隔调大一点就好了
        if (bossState == BossState.Dead) return;
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
                RunTimer();
                bonusPoints -= bonusPointsReducePerFrame;
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
                if (spellCardInfos[curScNumber].useDefaultItems) {
                    if (hasBonus)
                        item.itemSequence = new[]
                            { new ItemSpawnEntry(ItemType.LifeFrag, 1), new ItemSpawnEntry(ItemType.Power, 50) };
                    else {
                        item.itemSequence = new[] { new ItemSpawnEntry(ItemType.Power, 50) };
                    }
                }
                else {
                    if (hasBonus)
                        item.itemSequence = spellCardInfos[curScNumber].bonusItemSequence
                            .Concat(spellCardInfos[curScNumber].itemSequence).ToArray();
                    else
                        item.itemSequence = spellCardInfos[curScNumber].itemSequence;
                }

                item.CreateItem();
                Timing.KillCoroutines("Shoot");
                ChangeState(BossState.Interval);
                
                break;
            case BossState.NonSpellCard:
                animator.IsEnchanting = enchantMovementRef.isEnchanting;
                if (damageable.curHealth <= 0 || damageable.curTime <= 0) {
                    AudioManager.Manager.PlaySound(AudioNames.SeShootTan);
                    bonusBannerCtrl.ActivateBonusState(false,hasBonus,0);
                    DeactivateBulletGenerator();
                    Timing.KillCoroutines("Shoot");
                    item.setItemDropEnable = !(damageable.curHealth > 0.001f);

                    if (spellCardInfos[curScNumber].useDefaultItems) {
                        if(hasBonus) item.itemSequence = new[] { new ItemSpawnEntry(ItemType.BombFrag, 1) };
                        else {
                            item.itemSequence = new ItemSpawnEntry[] { };
                        }
                    }
                    else {
                        if(hasBonus) item.itemSequence = spellCardInfos[curScNumber].bonusItemSequence
                            .Concat(spellCardInfos[curScNumber].itemSequence).ToArray();
                        else item.itemSequence = spellCardInfos[curScNumber].itemSequence;
                    }

                    item.CreateItem();
                    ChangeState(BossState.Interval);
                }

                break;
            case BossState.Dead:
                break;
        }
    }

}
