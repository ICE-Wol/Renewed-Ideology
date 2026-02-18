using System.Collections.Generic;
using NUnit.Framework;
using Prota;
using UnityEngine;
using System.Linq;

public class SerializableDictionaryTests
{
    [Test]
    public void SerializableHashMap_Add_StoresValue()
    {
        var dictionary = new SerializableHashMap<int, string>();
        dictionary.Add(1, "one");
        Assert.AreEqual("one", dictionary[1]);
    }

    [Test]
    public void SerializableHashMap_Remove_RemovesKey()
    {
        var dictionary = new SerializableHashMap<int, string>();
        dictionary.Add(1, "one");
        dictionary.Remove(1);
        Assert.IsFalse(dictionary.ContainsKey(1));
    }

    [Test]
    public void SerializableHashMap_Clear_RemovesAllItems()
    {
        var dictionary = new SerializableHashMap<int, string>();
        dictionary.Add(1, "one");
        dictionary.Clear();
        Assert.AreEqual(0, dictionary.Count);
    }

    [Test]
    public void SerializableHashMap_Count_ReturnsCorrectCount()
    {
        var dictionary = new SerializableHashMap<int, string>();
        dictionary.Add(1, "one");
        dictionary.Add(2, "two");
        Assert.AreEqual(2, dictionary.Count);
    }

    [Test]
    public void SerializableHashMap_ContainsKey_ReturnsCorrectValue()
    {
        var dictionary = new SerializableHashMap<int, string>();
        dictionary.Add(1, "one");
        dictionary.Add(2, "two");
        Assert.IsTrue(dictionary.ContainsKey(1));
        Assert.IsTrue(dictionary.ContainsKey(2));
        Assert.IsFalse(dictionary.ContainsKey(3));
    }

    [Test]
    public void SerializableHashMap_TryGetValue_ReturnsCorrectValue()
    {
        var dictionary = new SerializableHashMap<int, string>();
        dictionary.Add(1, "one");
        dictionary.Add(2, "two");
        
        Assert.IsTrue(dictionary.TryGetValue(1, out var one));
        Assert.AreEqual("one", one);
        
        Assert.IsTrue(dictionary.TryGetValue(2, out var two));
        Assert.AreEqual("two", two);
        
        Assert.IsFalse(dictionary.TryGetValue(3, out var three));
        Assert.IsNull(three);
    }

    [Test]
    public void SerializableHashMap_Enumerate_IteratesAllItems()
    {
        var dictionary = new SerializableHashMap<int, string>();
        dictionary.Add(1, "one");
        dictionary.Add(2, "two");
        
        var count = 0;
        foreach(var item in dictionary)
        {
            count++;
        }
        Assert.AreEqual(2, count);
        
        var array = dictionary.ToArray();
        Assert.AreNotEqual(array[0].Key, array[1].Key);
        Assert.IsTrue(array[0].Key == 1 || array[0].Key == 2);
        Assert.IsTrue(array[1].Key == 2 || array[1].Key == 1);
        Assert.AreNotEqual(array[0].Value, array[1].Value);
        Assert.IsTrue(array[0].Value == "one" || array[1].Value == "one");
        Assert.IsTrue(array[0].Value == "two" || array[1].Value == "two");
    }

    [Test]
    public void SerializableHashMap_LargeScaleOperations_MatchesStandardDictionary()
    {
        var standardDict = new Dictionary<string, string>();
        var serializableDict = new SerializableHashMap<string, string>();
        
        for(int i = 0; i < 1000; i++)
        {
            var k = UnityEngine.Random.Range(0, 100).ToString();
            var v = UnityEngine.Random.Range(0, 100).ToString();
            standardDict[k] = v;
            serializableDict[k] = v;
            
            for(int j = 0; j < 100; j++)
            {
                Assert.AreEqual(
                    standardDict.ContainsKey(j.ToString()), 
                    serializableDict.ContainsKey(j.ToString()),
                    $"Mismatch at iteration {i}, key {j}");
            }
        }
        
        for(int i = 0; i < 1000; i++)
        {
            if(i % 2 == 0) continue;
            var k = i.ToString();
            standardDict.Remove(k);
            serializableDict.Remove(k);
            
            for(int j = 0; j < 1000; j++)
            {
                Assert.AreEqual(
                    standardDict.ContainsKey(j.ToString()), 
                    serializableDict.ContainsKey(j.ToString()),
                    $"Mismatch after removal at iteration {i}, key {j}");
            }
            
            var z = UnityEngine.Random.Range(0, 2000);
            Assert.AreEqual(
                standardDict.ContainsKey(z.ToString()), 
                serializableDict.ContainsKey(z.ToString()),
                $"Mismatch at random check, key {z}");
        }
    }
}
