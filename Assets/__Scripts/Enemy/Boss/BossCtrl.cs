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
    public int maxScNumber;
    
    public BulletGenerator curAttackPattern;
    public List<SpellCardInfo> spellCardInfos;
    public BulletGenerator enchantMovementRef;
    
    [Tooltip("版面下方血条中央展示的剩余符卡数量 已废弃")]
    public int scNum;
    
    public Damageable damageable;
    public Movement movement;
    public ItemSpawner item;
    public BossAnimator animator;
    
    public SpellCircle spellCircle;
    public BossState bossState;
    
    [Header("符卡计时器")]
    public int frameTimer;
    public float actualTimer;
    public bool isTimerActivated;

    [Header("收取指示变量")] public bool hasBonus;
    public int bonusPoints;
    public int bonusPointsReducePerFrame = 10;
    
    /*
     * 所有的数据都应该聚集在一处
     * 因此符卡练习的数据应该从这里读取而非单独列一遍
     *
     * 此外，老版本将子弹生成器的预制体提前放在了场景中
     * 实际上不用这么做
     * 放在so里动态生成就好了
     */
    public void CreateBulletGenerator() {
        //attackPatternSet[curScNumber].SetActive(true);
        curAttackPattern = Instantiate(spellCardInfos[curScNumber].bulletGenerator,transform);
        enchantMovementRef = curAttackPattern;
        movement.stayFrames = enchantMovementRef.waveFrameInterval;
    }
    public void DestroyBulletGenerator() {
        //attackPatternSet[curScNumber].SetActive(false);
        Destroy(curAttackPattern.gameObject);
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
        SCTimeInfoCtrl.instance.breakTime.text = (frameTimer / 60f).ToString("F2") + 's';
        SCTimeInfoCtrl.instance.actualTime.text = actualTimer.ToString("F2") + 's';
        SCTimeInfoCtrl.instance.AppearAll();
        frameTimer = 0;
        actualTimer = 0;
        isTimerActivated = false;
    }
    
    private void Start() {
        ChangeState(BossState.Interval);
    }

    private void OnEnable() {
        BossManager.instance.curBoss = this;
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
                
                SpellAnnouncement.instance.ResetAnnounce();
                
                // if boss has remaining sc or ncs
                if (curScNumber + 1 < maxScNumber) {
                    curScNumber++;
                    
                    var curScInfo = spellCardInfos[curScNumber];
                    
                    //print("curScNumber: " + curScNumber + " spellCardInfosCount " + spellCardInfos.Count);
                    damageable.maxTime = curScInfo.useDefaultTime ? curScInfo.DefaultTime : curScInfo.maxTime;
                    damageable.maxHealth = curScInfo.useDefaultHealth ? curScInfo.DefaultHealth : curScInfo.maxHealth;

                    SpellAnnouncement.instance.scNameText.text = curScInfo.spellName + "「" +
                                                                 curScInfo.cardName + "」";
                        
                    if(curScInfo.hasInitPos) {
                        movement.hasInitPos = true;
                        movement.initPos = curScInfo.initPos;
                        movement.initPosFinished = false;
                        movement.hasFixedPos = curScInfo.hasFixedPos;
                    }
                    
                    Timing.RunCoroutine(HealthRecharge().CancelWith(gameObject));
                    Timing.RunCoroutine(TimeRecharge().CancelWith(gameObject));
                }
                else {
                    //如果没有剩余符卡，那么转到死亡状态
                    ChangeState(BossState.Dead);
                } 
                break;
            case BossState.SpellCardAnnounce:
                StartTimer();
                bonusPoints = spellCardInfos[curScNumber].maxBonusPoints;
                SpellAnnouncement.instance.StartAnnouncing();
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
                BonusBannerCtrl.instance.ActivateBonusState(true,hasBonus,bonusPoints);
                if (hasBonus)
                    PlayerCtrl.instance.state.score += bonusPoints;
                spellCircle.SetState(SpellCircle.SpellCircleState.Shrink);
                scNum--;
                //todo 改为调用左侧星星数量改变函数
                //scNumText.text = scNum.ToString();
                AudioManager.Manager.PlaySound(AudioNames.SeShootTan);
                break;
            case BossState.Dead:
                //必定经过interval来到dead，所以不需要再次调用erase
                //GameManager.Manager.StartEraseBullets(PlayerCtrl.Player.transform.position);//transform.position);
                GameManager.Manager.reverseColorCtrl.StartReverseColorEffectAtCenter(transform.position);
                
                AudioManager.Manager.PlaySound(AudioNames.SeBossExplode);
                
                Damageable.damageableSet.Remove(damageable);
                
                DistortCtrl.Set(0f,0,Vector3.zero);
                
                Timing.RunCoroutine(HealthToZero());
                Timing.RunCoroutine(TimeToZero());
                
                DestroyImmediate(gameObject);
                
                break;
        }
        //在change state里调用change state
        //会导致全局状态被递归后的状态覆盖
        //为避免屎山崩塌，故在此打补丁
        //事实上不是这里的问题，而是消弹圈是全局的问题(现已改成单独物体控制)
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
                    damageable.curTime >= damageable.maxTime
                    /* 改成物体控制之后就没办法直接判断消弹是否结束了
                     * 去掉这个条件也无关紧要
                    && GameManager.Manager.IsEraseFinished()*/) {
                    if (spellCardInfos[curScNumber].isSpellCard) {
                        ChangeState(BossState.SpellCardAnnounce);
                    }
                    else {
                        ChangeState(BossState.NonSpellCard);
                        CreateBulletGenerator();
                    }
                }
                
                break;
            case BossState.SpellCardAnnounce:
                if (SpellAnnouncement.instance.isAnnounceFinished) {
                    ChangeState(BossState.SpellCard);
                    CreateBulletGenerator();
                }

                break;
            case BossState.SpellCard:
                RunTimer();
                bonusPoints -= bonusPointsReducePerFrame;
                animator.IsEnchanting = enchantMovementRef.isEnchanting;
                if (damageable.curHealth <= 0 || damageable.curTime <= 0) {
                    ChangeState(BossState.SpellCardBreak);
                    item.setItemDropEnable = !(damageable.curHealth > 0.001f);
                    DestroyBulletGenerator();
                }

                break;
            case BossState.SpellCardBreak:
                animator.IsEnchanting = false;
                SpellAnnouncement.instance.SpellBreak();
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
                if(!PracticeManager.instance.spellPracticeStartInfo.isSpellPracticeMode)
                    item.CreateItem();
                Timing.KillCoroutines("Shoot");
                ChangeState(BossState.Interval);
                
                break;
            case BossState.NonSpellCard:
                if(enchantMovementRef != null) animator.IsEnchanting = enchantMovementRef.isEnchanting;
                if (damageable.curHealth <= 0 || damageable.curTime <= 0) {
                    AudioManager.Manager.PlaySound(AudioNames.SeShootTan);
                    BonusBannerCtrl.instance.ActivateBonusState(false,hasBonus,0);
                    DestroyBulletGenerator();
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

                    if(!PracticeManager.instance.spellPracticeStartInfo.isSpellPracticeMode)
                        item.CreateItem();
                    ChangeState(BossState.Interval);
                }

                break;
            case BossState.Dead:
                break;
        }
    }

}
