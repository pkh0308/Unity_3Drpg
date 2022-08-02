using UnityEngine;

public class GoodsManager
{
    //싱글톤 구현
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
    public enum Goods { Gold }

    public GoodsManager()
    {
        gold = PlayerPrefs.HasKey(Goods.Gold.ToString()) ? PlayerPrefs.GetInt(Goods.Gold.ToString()) : 0;
    }

    //획득한 골드를 더해서 최대값(int.MaxValue)을 넘어갈 경우 false 반환
    public bool GetGold(int amount)
    {
        int temp = gold + amount;
        if (temp > int.MaxValue) return false;

        gold = temp;
        UiManager.updateGold();
        return true;
    }

    //사용 후 골드가 0 미만이 될 경우 false 반환
    //호출 시 조건문안에 사용할 것(false 반환 시 실패 처리)
    public bool SpendGold(int price)
    {
        int temp = gold - price;
        if (temp < 0) return false;

        gold = temp;
        UiManager.updateGold();
        return true;
    }

    public void Save()
    {
        PlayerPrefs.SetInt(Goods.Gold.ToString(), gold);
    }
}
