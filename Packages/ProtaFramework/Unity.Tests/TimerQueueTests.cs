using System;
using System.Collections.Generic;
using NUnit.Framework;
using Prota.Unity;
using UnityEngine;

public class TimerQueueTests
{
    [Test]
    public void NewAndBasicTriggerOrder()
    {
        float t = 0f;
        var q = new TimerQueue();
        var ownerA = ScriptableObject.CreateInstance<TestTimerOwner>();
        var ownerB = ScriptableObject.CreateInstance<TestTimerOwner>();

        bool a = false, b = false;
        var ta = q.Add(new Timer(t, 0.1f, 0.1f, false, true, ownerA));
        ta.SetCallback(() => a = true);
        var tb = q.Add(new Timer(t, 0.2f, 0.2f, false, true, ownerB));
        tb.SetCallback(() => b = true);

        t = 0.15f;
        q.Update(t);
        Assert.True(a);
        Assert.False(b);
        Assert.False(ta.isAlive);
        Assert.True(tb.isAlive);

        t = 0.25f;
        q.Update(t);
        Assert.True(b);
        Assert.False(tb.isAlive);

        UnityEngine.Object.DestroyImmediate(ownerA);
        UnityEngine.Object.DestroyImmediate(ownerB);
    }

    [Test]
    public void SameNextTime_ExecutesInCreationOrder()
    {
        float t = 0f;
        var q = new TimerQueue();
        var ownerA = ScriptableObject.CreateInstance<TestTimerOwner>();
        var ownerB = ScriptableObject.CreateInstance<TestTimerOwner>();

        var fired = new List<string>();
        var ta = q.Add(new Timer(t, 0.1f, 0.1f, false, true, ownerA));
        ta.SetCallback(() => fired.Add("A"));
        var tb = q.Add(new Timer(t, 0.1f, 0.1f, false, true, ownerB));
        tb.SetCallback(() => fired.Add("B"));

        t = 0.11f;
        q.Update(t);
        Assert.AreEqual(2, fired.Count);
        Assert.AreEqual("A", fired[0]);
        Assert.AreEqual("B", fired[1]);

        t = 0.11f;
        q.Update(t);

        Assert.AreEqual(2, fired.Count);
        Assert.AreEqual("A", fired[0]);
        Assert.AreEqual("B", fired[1]);

        UnityEngine.Object.DestroyImmediate(ownerA);
        UnityEngine.Object.DestroyImmediate(ownerB);
    }

    [Test]
    public void RepeatTimer_ReenqueuesAndCanStop()
    {
        float t = 0f;
        var q = new TimerQueue();
        var owner = ScriptableObject.CreateInstance<TestTimerOwner>();

        int count = 0;
        var tr = q.Add(new Timer(t, 0.2f, 0.2f, true, true, owner));
        tr.SetCallback(() => count++);

        // first trigger at 0.2
        t = 0.2f;
        q.Update(t);
        Assert.AreEqual(1, count);
        Assert.True(tr.isAlive);

        // second trigger at 0.4
        t = 0.4f;
        q.Update(t);
        Assert.AreEqual(2, count);
        Assert.True(tr.isAlive);

        // stop repeating
        tr.Destroy();

        // advancing further should not trigger again
        t = 0.6f;
        q.Update(t);
        Assert.AreEqual(2, count);
        Assert.False(tr.isAlive);

        UnityEngine.Object.DestroyImmediate(owner);
    }

    [Test]
    public void GuardDead_PreventsExecution()
    {
        float t = 0f;
        var q = new TimerQueue();
        var owner = ScriptableObject.CreateInstance<TestTimerOwner>();

        bool fired = false;
        var timer = q.Add(new Timer(t, 0.1f, 0.1f, false, true, owner));
        timer.SetCallback(() => fired = true);

        // destroy owner before due
        UnityEngine.Object.DestroyImmediate(owner);
        t = 0.2f;
        q.Update(t);

        Assert.False(fired);
        Assert.False(timer.isAlive);
    }

    [Test]
    public void TryRemoveAndClear_Work()
    {
        float t = 0f;
        var q = new TimerQueue();
        var owner1 = ScriptableObject.CreateInstance<TestTimerOwner>();
        var owner2 = ScriptableObject.CreateInstance<TestTimerOwner>();

        bool fired1 = false, fired2 = false;
        var t1 = q.Add(new Timer(t, 0.1f, 0.1f, false, true, owner1));
        t1.SetCallback(() => fired1 = true);
        var t2 = q.Add(new Timer(t, 0.2f, 0.2f, false, true, owner2));
        t2.SetCallback(() => fired2 = true);

        t1.Destroy();
        Assert.False(t1.isAlive);

        // Clear removes remaining
        q.Clear();
        Assert.False(t2.isAlive);

        t = 1.0f;
        q.Update(t);
        Assert.False(fired1);
        Assert.False(fired2);

        UnityEngine.Object.DestroyImmediate(owner1);
        UnityEngine.Object.DestroyImmediate(owner2);
    }
}



