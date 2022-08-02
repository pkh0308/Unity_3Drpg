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

    //현재 카메라의 y축 회전각의 sin, cos값 계산
    void CalSinCos()
    {
        sinRes = Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad);
        cosRes = Mathf.Cos(transform.eulerAngles.y * Mathf.Deg2Rad);
    }

    //마우스 휠 드래그를 입력받아 minWheel ~ maxWheel 내에서  wheel값 변경
    //위에서 계산한 sin, cos 값에 wheel 값을 곱해서 offset 설정 후 카메라 포지션에 더해줌(줌 인 시 살짝 내려가도록 하기 위해 y축 값은 -0.5f)
    void Move()
    {
        wheel += Input.GetAxisRaw("Mouse ScrollWheel");
        wheel = Mathf.Clamp(wheel, minWheel, maxWheel);
        expandOffset = new Vector3(sinRes, -0.5f, cosRes) * wheel;
        transform.position = target.position + offset.position + expandOffset;
    }

    //z축 회전을 막기 위해 z축 회전각을 0으로 고정하는 함수
    void Limit()
    {
        angle = transform.eulerAngles;
        if (angle.z != 0)
            transform.rotation = Quaternion.Euler(angle.x, angle.y, 0);
    }

    //CameraDrag에서 계산한 x축, y축 이동값으로 카메라 회전
    //좌우 드래그한 정도로 플레이어를 기준으로 y축 회전
    //상하 드래그한 정도로 플레이어를 기준으로 x축, y축 회전(sin, cos 값 사용)
    //카메라와 플레이어 간 거리 offset도 동일하게 회전시켜 시점 유지
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
