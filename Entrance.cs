using UnityEngine;

public class Entrance : MonoBehaviour
{
    [SerializeField] Vector3 targetPos;

    public Vector3 GetPos()
    {
        return targetPos;
    }
}
