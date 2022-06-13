using UnityEngine;

public class RoomCamera : MonoBehaviour
{
    //회전 방지, 쫓아가는 기능만 제공
    Vector3 originPos;
    Vector3 expandOffset;

    float wheel;
    public float maxWheel;
    public float minWheel;

    float cosRes;
    float sinRes;

    void Start()
    {
        originPos = transform.position;
    }

    void Update()
    {
        CalSinCos();
        Move();
    }

    void CalSinCos()
    {
        sinRes = Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad);
        cosRes = Mathf.Cos(transform.eulerAngles.y * Mathf.Deg2Rad);
    }

    void Move()
    {
        wheel += Input.GetAxisRaw("Mouse ScrollWheel");
        wheel = Mathf.Clamp(wheel, minWheel, maxWheel);
        expandOffset = new Vector3(sinRes, -0.5f, cosRes) * wheel;
        transform.position = originPos + expandOffset;
    }
}
