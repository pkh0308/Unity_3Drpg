using System;
using UnityEngine;

public class ImgContainer : MonoBehaviour
{
    [SerializeField] Sprite sprite_50001;
    [SerializeField] Sprite sprite_70001;
    [SerializeField] Sprite sprite_70011;

    public static Func<int, Sprite> getItemImg;

    void Awake()
    {
        getItemImg = (a) => { return GetItemImg(a); };
    }

    //입력받은 id 아이템의 스프라이트 이미지 반환
    public Sprite GetItemImg(int id)
    {
        switch(id)
        {
            case 50001:
                return sprite_50001;
            case 70001:
                return sprite_70001;
            case 70011:
                return sprite_70011;
            default:
                return null;
        }
    }
}
