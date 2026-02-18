using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Prota;
using UnityEngine;

public class SerializableHashSetTests
{
    [Test]
    public void SerializableHashSet_Add_StoresValue()
    {
        var hashSet = new SerializableHashSet<int>();
        hashSet.Add(1);
        Assert.IsTrue(hashSet.Contains(1));
    }

    [Test]
    public void SerializableHashSet_Remove_RemovesItem()
    {
        var hashSet = new SerializableHashSet<int>();
        hashSet.Add(1);
        hashSet.Remove(1);
        Assert.IsFalse(hashSet.Contains(1));
    }

    [Test]
    public void SerializableHashSet_Clear_RemovesAllItems()
    {
        var hashSet = new SerializableHashSet<int>();
        hashSet.Add(1);
        hashSet.Add(2);
        hashSet.Clear();
        Assert.AreEqual(0, hashSet.Count);
    }

    [Test]
    public void SerializableHashSet_Count_ReturnsCorrectCount()
    {
        var hashSet = new SerializableHashSet<int>();
        hashSet.Add(1);
        hashSet.Add(2);
        hashSet.Add(3);
        Assert.AreEqual(3, hashSet.Count);
    }

    [Test]
    public void SerializableHashSet_Contains_ReturnsCorrectValue()
    {
        var hashSet = new SerializableHashSet<int>();
        hashSet.Add(1);
        hashSet.Add(2);
        Assert.IsTrue(hashSet.Contains(1));
        Assert.IsTrue(hashSet.Contains(2));
        Assert.IsFalse(hashSet.Contains(3));
    }

    [Test]
    public void SerializableHashSet_AddDuplicate_DoesNotIncreaseCount()
    {
        var hashSet = new SerializableHashSet<int>();
        hashSet.Add(1);
        hashSet.Add(1);
        Assert.AreEqual(1, hashSet.Count);
    }

    [Test]
    public void SerializableHashSet_Enumerate_IteratesAllItems()
    {
        var hashSet = new SerializableHashSet<int>();
        hashSet.Add(1);
        hashSet.Add(2);
        hashSet.Add(3);
        
        var count = 0;
        var items = new HashSet<int>();
        foreach(var item in hashSet)
        {
            count++;
            items.Add(item);
        }
        Assert.AreEqual(3, count);
        Assert.IsTrue(items.Contains(1));
        Assert.IsTrue(items.Contains(2));
        Assert.IsTrue(items.Contains(3));
    }

    [Test]
    public void SerializableHashSet_ToArray_ReturnsAllItems()
    {
        var hashSet = new SerializableHashSet<int>();
        hashSet.Add(1);
        hashSet.Add(2);
        hashSet.Add(3);
        
        var array = hashSet.ToArray();
        Assert.AreEqual(3, array.Length);
        Assert.IsTrue(array.Contains(1));
        Assert.IsTrue(array.Contains(2));
        Assert.IsTrue(array.Contains(3));
    }

    [Test]
    public void SerializableHashSet_LargeScaleOperations_MatchesStandardHashSet()
    {
        var standardSet = new HashSet<string>();
        var serializableSet = new SerializableHashSet<string>();
        
        for(int i = 0; i < 1000; i++)
        {
            var k = UnityEngine.Random.Range(0, 100).ToString();
            standardSet.Add(k);
            serializableSet.Add(k);
            
            for(int j = 0; j < 100; j++)
            {
                Assert.AreEqual(
                    standardSet.Contains(j.ToString()), 
                    serializableSet.Contains(j.ToString()),
                    $"Mismatch at iteration {i}, key {j}");
            }
        }
        
        for(int i = 0; i < 1000; i++)
        {
            if(i % 2 == 0) continue;
            var k = i.ToString();
            standardSet.Remove(k);
            serializableSet.Remove(k);
            
            for(int j = 0; j < 1000; j++)
            {
                Assert.AreEqual(
                    standardSet.Contains(j.ToString()), 
                    serializableSet.Contains(j.ToString()),
                    $"Mismatch after removal at iteration {i}, key {j}");
            }
            
            var z = UnityEngine.Random.Range(0, 2000);
            Assert.AreEqual(
                standardSet.Contains(z.ToString()), 
                serializableSet.Contains(z.ToString()),
                $"Mismatch at random check, key {z}");
        }
    }

    [Test]
    public void SerializableHashSet_UnionWith_WorksCorrectly()
    {
        var hashSet1 = new SerializableHashSet<int> { 1, 2, 3 };
        var hashSet2 = new HashSet<int> { 3, 4, 5 };
        hashSet1.UnionWith(hashSet2);
        
        Assert.AreEqual(5, hashSet1.Count);
        Assert.IsTrue(hashSet1.Contains(1));
        Assert.IsTrue(hashSet1.Contains(2));
        Assert.IsTrue(hashSet1.Contains(3));
        Assert.IsTrue(hashSet1.Contains(4));
        Assert.IsTrue(hashSet1.Contains(5));
    }

    [Test]
    public void SerializableHashSet_IntersectWith_WorksCorrectly()
    {
        var hashSet1 = new SerializableHashSet<int> { 1, 2, 3 };
        var hashSet2 = new HashSet<int> { 2, 3, 4 };
        hashSet1.IntersectWith(hashSet2);
        
        Assert.AreEqual(2, hashSet1.Count);
        Assert.IsFalse(hashSet1.Contains(1));
        Assert.IsTrue(hashSet1.Contains(2));
        Assert.IsTrue(hashSet1.Contains(3));
        Assert.IsFalse(hashSet1.Contains(4));
    }

    [Test]
    public void SerializableHashSet_ExceptWith_WorksCorrectly()
    {
        var hashSet1 = new SerializableHashSet<int> { 1, 2, 3 };
        var hashSet2 = new HashSet<int> { 2, 3 };
        hashSet1.ExceptWith(hashSet2);
        
        Assert.AreEqual(1, hashSet1.Count);
        Assert.IsTrue(hashSet1.Contains(1));
        Assert.IsFalse(hashSet1.Contains(2));
        Assert.IsFalse(hashSet1.Contains(3));
    }
}
