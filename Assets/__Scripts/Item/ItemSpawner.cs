using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Enemy;
using _Scripts.Tools;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Scripts.Item {
    [Serializable]
    public struct ItemSpawnEntry {
        public ItemType type;
        public int num;

        public ItemSpawnEntry(ItemType t, int n) {
            type = t;
            num = n;
        }
    }
    public class ItemSpawner : MonoBehaviour {
        public ItemSpawnEntry[] itemSequence;
        public bool hasSpecialItem;
        public bool setItemDropEnable;
        
        void Awake() {
            //GetComponent<Damageable>().OnDead += CreateItem;
        }
        
        bool CheckSpecialItem() {
            var length = itemSequence.Length;
            hasSpecialItem = false;
            for (int i = 0; i < length; i++) {
                if ((int)itemSequence[i].type >= 5) {
                    hasSpecialItem = true;
                    return true;
                }
            }
            return false;
        }

        public void RefreshItem(ItemSpawnEntry[] arr) {
            CheckSpecialItem();
            itemSequence = arr;
        }

        //public static int cnt;
        public void CreateItem() {
            //cnt++;
            //print(cnt);
            if (!setItemDropEnable) return;
            CheckSpecialItem();
            if (hasSpecialItem) {
                AudioManager.Manager.PlaySound(Random.value > 0.5f ? AudioNames.SeBonus0 : AudioNames.SeBonus1);
            }
            var length = itemSequence.Length;
            if (length == 1 && itemSequence[0].num == 1) {
                if (hasSpecialItem) {
                    Instantiate(GameManager.Manager.items[(int)(itemSequence[0].type - 4)],
                        transform.position, transform.rotation);
                }
                else {
                    var item = Instantiate(GameManager.Manager.items[0],
                        transform.position, transform.rotation);
                    item.SetType(itemSequence[0].type);
                }
            }
            else if(!hasSpecialItem) {
                for (int i = 0; i < length; i++) {
                    for (int j = 0; j < itemSequence[i].num; j++) {
                        if (length < 5) {
                            var itemSpawnPos = transform.position + Calc.GetRandomVectorCircle
                                (0, 360, 1f);
                            var item = Instantiate(GameManager.Manager.items[0],
                                itemSpawnPos, transform.rotation);
                            item.SetType(itemSequence[i].type);
                            item.transform.position.SetZ(j * 0.1f);
                        }
                    }
                }
            }
            else {
                for (int i = 0; i < length; i++) {
                    for (int j = 0; j < itemSequence[i].num; j++) {
                        if ((int)itemSequence[i].type < 5) {
                            var itemSpawnPos = transform.position + Calc.GetRandomVectorRing
                                (0, 360, 0.8f, 1.3f);
                            var item = Instantiate(GameManager.Manager.items[0], 
                                itemSpawnPos, transform.rotation);
                            item.SetType(itemSequence[i].type);
                            item.transform.position.SetZ(j * 0.1f);
                            item.transform.parent = GameManager.Manager.itemSortingGroup;
                        }
                        else {
                            var item = Instantiate(GameManager.Manager.items[(int)(itemSequence[i].type - 4)],
                                transform.position, transform.rotation);
                            item.transform.parent = GameManager.Manager.itemSortingGroup;
                        }
                    }
                }
            }
        }
    }

}
