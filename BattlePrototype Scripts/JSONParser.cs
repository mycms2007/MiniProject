using UnityEngine;

public class JSONParser : MonoBehaviour
{
    public MonsterData[] monsters;

    void Start()
    {
        monsters = ParseJSON("monsters");
    }

    public MonsterData[] ParseJSON(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);
        MonsterDataList dataList = JsonUtility.FromJson<MonsterDataList>(jsonFile.text);

        MonsterData[] result = new MonsterData[dataList.monsters.Length];

        for (int i = 0; i < dataList.monsters.Length; i++)
        {
            MonsterData data = ScriptableObject.CreateInstance<MonsterData>();
            data.monsterName = dataList.monsters[i].monsterName;
            data.hp = dataList.monsters[i].hp;
            data.attack = dataList.monsters[i].attack;
            data.defense = dataList.monsters[i].defense;
            data.dropItem = dataList.monsters[i].dropItem;
            data.dropRate = dataList.monsters[i].dropRate;
            result[i] = data;
        }

        return result;
    }
}

[System.Serializable]
public class MonsterDataRaw
{
    public string monsterName;
    public int hp;
    public int attack;
    public int defense;
    public string dropItem;
    public float dropRate;
}

[System.Serializable]
public class MonsterDataList
{
    public MonsterDataRaw[] monsters;
}
