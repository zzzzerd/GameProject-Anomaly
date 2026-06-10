using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂在结局记录面板上，打开时读取历史记录并动态生成列表
/// </summary>
public class EndingRecordPanel : MonoBehaviour
{
    [Header("列表容器")]
    public Transform content;

    [Header("单条记录 Prefab")]
    public EndingRecordItem recordItemPrefab;


    [Header("结局图片（按 EndingType 枚举顺序排列）")]
    // 顺序：Death, Warrior, Saint, AnomalySage, LostSoul, Farmer
    public Sprite[] endingSprites;

    private void Awake()
    {
        //游戏开始的时候记得隐藏，不然就出现之前的bug
        gameObject.SetActive(false);
    }

    //打开
    public void Open()
    {
        gameObject.SetActive(true);
        BuildList();
    }

    //关闭
    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void BuildList()
    {
        // 清空旧条目
        foreach (Transform child in content)
            Destroy(child.gameObject);

        var records = GameDataManager.Instance?.GetEndingRecords();
        if (records == null || records.Count == 0)
            return;

        // 最新的在最上面
        for (int i = records.Count - 1; i >= 0; i--)
        {
            var record = records[i];
            var item = Instantiate(recordItemPrefab, content);

            //获取图片
            var sprite = GetSprite(record.endingType);
            item.Setup(record, sprite);
        }
    }

    private Sprite GetSprite(EndingType type)
    {
        int index = (int)type;


        if (endingSprites != null && index < endingSprites.Length)
            return endingSprites[index];
        return null;
    }
}
