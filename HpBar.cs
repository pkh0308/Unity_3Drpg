using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] Image hpBar;
    Image hpBarSet;

    private void Awake()
    {
        hpBarSet = GetComponent<Image>();
    }

    public void UpdatePos(Vector3 pos)
    {
        hpBarSet.rectTransform.position = pos;
    }

    public void UpdateScale(int curHp, int maxHp)
    {
        hpBar.rectTransform.localScale = new Vector3((float)curHp / maxHp, 1, 1);
    }
}
