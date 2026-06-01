using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [System.Serializable]
    public class LootItem
    {
        public GameObject item;   //掉落物
        [Range(0f, 1f)]
        public float weight;   //权重，决定这个物品被选中的概率，权重越大，被选中的概率越大
    }

    public LootItem[] lootItems;

    public void SpawnLoot()
    {
        float currentValue = Random.value;
        for(int i = 0;i< lootItems.Length; i++)
        {
            if (currentValue <= lootItems[i].weight)
            {
                GameObject obj = Instantiate(lootItems[i].item);
                obj.transform.position = transform.position+Vector3.up*2;  //把掉落物生成在这个物体的位置
                //break;       //加上break就一次只能掉落一个物品
            }
        }
    }
}
