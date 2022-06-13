using UnityEngine;

public class Entrance : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] GameObject building;
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject roomCamera;

    public Vector3 Enter()
    {
        building.SetActive(building.activeSelf == false);
        mainCamera.SetActive(mainCamera.activeSelf == false);
        roomCamera.SetActive(roomCamera.activeSelf == false);
        return target.position;
    }
}
