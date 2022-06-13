using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform target;
    public Transform offset;
    Vector3 expandOffset;

    float wheel;
    public float maxWheel;
    public float minWheel;

    Vector3 angle;
    float cosRes;
    float sinRes;

    void Start()
    {
        transform.position = target.position + offset.position;
    }

    void Update()
    {
        CalSinCos();
        Move();
        Limit();
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
        transform.position = target.position + offset.position + expandOffset;
    }

    void Limit()
    {
        angle = transform.eulerAngles;
        if (angle.z != 0)
            transform.rotation = Quaternion.Euler(angle.x, angle.y, 0);
    }

    public void Turn(float x, float y)
    {
        transform.RotateAround(target.position, Vector3.up, x);
        offset.RotateAround(Vector3.zero, Vector3.up, x);

        transform.RotateAround(target.position, Vector3.right, -y * cosRes);
        transform.RotateAround(target.position, Vector3.forward, y * sinRes);
        if (angle.x > 60) { transform.rotation = Quaternion.Euler(59.5f, angle.y, 0);  return; }
        if (angle.x < 10) { transform.rotation = Quaternion.Euler(10.5f, angle.y, 0);  return; }

        offset.RotateAround(Vector3.zero, Vector3.right, -y * cosRes);
        offset.RotateAround(Vector3.zero, Vector3.forward, y * sinRes);
    }
}
