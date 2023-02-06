using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleManager : MonoBehaviour
{
    [SerializeField] TMP_InputField playerName;

    public void Btn_GameStart()
    {
        NetworkManager.Inst.MakeRoom(playerName.text);
    }
}