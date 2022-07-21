using UnityEngine;
using System;

public class ItemManager : MonoBehaviour
{
    public static Func<int, bool> useItem;
    Player player;

    void Awake()
    {
        useItem = (a) => { return UseItem(a); };
        player = Player.getPlayer();
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
