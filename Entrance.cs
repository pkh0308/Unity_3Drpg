using UnityEngine;

public class Entrance : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] GameObject building;

    public Vector3 Enter()
    {
        building.SetActive(building.activeSelf == false);
        return target.position;
    }
}
