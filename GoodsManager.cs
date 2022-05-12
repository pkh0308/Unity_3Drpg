using UnityEngine;

public class GoodsManager
{
    private static GoodsManager instance;
    public static GoodsManager Instance
    {
        get
        {
            if (instance == null)
                instance = new GoodsManager();

            return instance;
        }
    }

    int gold;
    public int Gold { get { return gold; } }
    public GoodsManager()
    {
        gold = PlayerPrefs.HasKey("Gold") ? PlayerPrefs.GetInt("Gold") : 0;
    }

    public bool GetGold(int amount)
    {
        int temp = gold + amount;
        if (temp > int.MaxValue) return false;

        gold = temp;
        return true;
    }

    public bool Purchase(int price)
    {
        int temp = gold - price;
        if (temp < 0) return false;

        gold = temp;
        return true;
    }

    public void Save()
    {
        PlayerPrefs.SetInt("Gold", gold);
    }
}
