using System;
using System.Collections.Generic;
using NUnit.Framework;
using Prota.Unity;
using UnityEngine;

public class TimingWheelTests
{
    

    [Test]
    public void BasicTriggerOrder()
    {
        var wheel = new TimingWheel("test", 0.1, 10);

        var fired = new bool[5];
        double t = 0.0;
        var owners = new TestTimerOwner[5];
        for(int i = 0; i < 5; i++)
        {
            owners[i] = ScriptableObject.CreateInstance<TestTimerOwner>();
        }

        // 4 个定时器, 落入不同槽位(0.1, 0.2, 0.3, 0.4)
        for(int i = 1; i <= 4; i++)
        {
            int idx = i;
            var delay = 0.1f * i;
            var timer = new Timer((float)t, delay, delay, false, true, owners[idx]);
            timer.SetCallback(() => fired[idx] = true);
            wheel.AddTimer(timer);
        }

        // 推进至 0.11 -> 仅处理槽位 0
        t += 0.11;
        wheel.Update(t);
        Assert.True(fired[1]);
        Assert.False(fired[2]);
        Assert.False(fired[3]);
        Assert.False(fired[4]);

        // 推进至 0.22 -> 触发 0.1, 0.2 槽位定时器
        t += 0.11;
        wheel.Update(t);
        Assert.True(fired[1]);
        Assert.True(fired[2]);
        Assert.False(fired[3]);
        Assert.False(fired[4]);

        // 再推进 0.3 -> 应触发剩余全部槽位定时器
        t += 0.3;
        wheel.Update(t);
        Assert.True(fired[1]);
        Assert.True(fired[2]);
        Assert.True(fired[3]);
        Assert.True(fired[4]);

        Assert.AreEqual(0, wheel.GetActiveCount());

        for(int i = 0; i < 5; i++)
        {
            if(owners[i] != null)
                UnityEngine.Object.DestroyImmediate(owners[i]);
        }
    }

    [Test]
    public void RepeatTimer_Reenqueues()
    {
        var wheel = new TimingWheel("test", 0.1, 10);
        double t = 0.0;
        var owner = ScriptableObject.CreateInstance<TestTimerOwner>();

        var executedCount = 0;
        Prota.Unity.Timer repeat = new((float)t, 0.2f, 0.2f, true, true, owner);
        repeat.SetCallback(() => executedCount++);
        wheel.AddTimer(repeat);
        Assert.AreEqual(1, wheel.GetActiveCount());
		
		t = 0.101;
		wheel.Update(t);
		Assert.AreEqual(0, executedCount);
		Assert.AreEqual(1, wheel.GetActiveCount());

        // 边界 0.2 时触发第一次（时间轮按桶边界触发）
        t = 0.201;
        wheel.Update(t);
        Assert.AreEqual(1, executedCount);
        Assert.AreEqual(1, wheel.GetActiveCount());

        // 第二次在 0.4 边界触发
        t = 0.401;
        wheel.Update(t);
        Assert.AreEqual(2, executedCount);
        Assert.AreEqual(1, wheel.GetActiveCount());

        // 第三次在 0.6 边界触发
        t = 0.601;
        wheel.Update(t);
        Assert.AreEqual(3, executedCount);
        Assert.AreEqual(1, wheel.GetActiveCount());

        // 标记禁用后不再触发
		repeat.Destroy();
        t = 0.801;
        wheel.Update(t);
        Assert.AreEqual(3, executedCount);
        Assert.AreEqual(0, wheel.GetActiveCount());
		
		t = 1.001;
		wheel.Update(t);
		Assert.AreEqual(3, executedCount);
		Assert.AreEqual(0, wheel.GetActiveCount());

        UnityEngine.Object.DestroyImmediate(owner);
    }

    [Test]
    public void OwnerDead_IsNotExecuted()
    {
        var wheel = new TimingWheel("test", 0.1, 10);
        var owner = ScriptableObject.CreateInstance<TestTimerOwner>();
        double t = 0.0;

        var executed = false;
        var timer = new Timer((float)t, 0.1f, 0.1f, false, true, owner);
        timer.SetCallback(() => executed = true);
        wheel.AddTimer(timer);
        Assert.AreEqual(1, wheel.GetActiveCount());

        // 所有者死亡
        UnityEngine.Object.DestroyImmediate(owner);
        t = 0.2;
        wheel.Update(t);

        Assert.False(executed);
        Assert.AreEqual(0, wheel.GetActiveCount());
    }
}



