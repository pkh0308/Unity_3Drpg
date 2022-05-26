using System;
using UnityEngine;

public class ImgContainer : MonoBehaviour
{
    [SerializeField] Sprite sprite_50001;
    [SerializeField] Sprite sprite_50002;
    [SerializeField] Sprite sprite_50003;

    public static Func<int, Sprite> getItemImg;

    void Awake()
    {
        getItemImg = (a) => { return GetItemImg(a); };
    }

    public Sprite GetItemImg(int id)
    {
        switch(id)
        {
            case 50001:
                return sprite_50001;
            default:
                return null;
        }
    }
}
