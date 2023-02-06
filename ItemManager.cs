using UnityEngine;
using System;

//사용 가능한 아이템들의 사용 시 로직 보관용 클래스
//사뇽 가능한 아이템 추가 시 UseItem 에 id 및 로직 추가 요망
public class ItemManager : MonoBehaviour
{
    public static Func<int, bool> useItem;
    Player player;

    void Awake()
    {
        useItem = (a) => { return UseItem(a); };
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    bool UseItem(int id)
    {
        switch(id)
        {
            case 70001:
                if (player.CurHp == player.MaxHp) return false;
                player.UpdateHp(50);
                return true;
            case 70011:
                if (player.CurSp == player.MaxSp) return false;
                player.UpdateSp(30);
                return true;
            default:
                return false;
        }
    }
}
