using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    float hAxis;
    float vAxis;
    bool wDown;
    bool mouseLeftDown;
    bool isTargetMoving;
    int mask;

    [SerializeField] Transform moveReference;
    Vector3 targetPos;
    public GameObject targetNpc;
    public float speed;
    public float walkOffset;
    public Transform cameraReference;

    public Animator playerAnimator;
    public GameManager gameManager;
    public UiManager uiManager;

    private void Awake()
    {
        mask = (-1) - (1 << LayerMask.NameToLayer("Player"));
    }

    void Update()
    {
        if (gameManager.Pause) return;

        InputCheck();
        Move();
        TargetMove();
        Turn();
    }

    void InputCheck()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetKey(KeyCode.LeftShift);

        if(Input.GetKeyDown(KeyCode.I))
        {
            gameManager.InventoryControll();
        }

        mouseLeftDown = Input.GetMouseButton(0);
        if (!mouseLeftDown) return;
        if (EventSystem.current.IsPointerOverGameObject()) Debug.Log("no");

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit rayHit, Mathf.Infinity, mask))
        {
            switch (rayHit.collider.tag)
            {
                case "Platform":
                    SetTargetPos(rayHit.point);
                    break;
                case "Npc":
                    if (!isTargetMoving) targetNpc = rayHit.collider.gameObject;
                    SetTargetPos(rayHit.point);
                    break;
                default:
                    targetNpc = null;
                    break;
            }
        }
    }

    void Move()
    {
        float angle = cameraReference.eulerAngles.y;
        moveReference.position = wDown ? new Vector3(hAxis, 0, vAxis).normalized * speed * walkOffset
                                       : new Vector3(hAxis, 0, vAxis).normalized * speed;
        moveReference.RotateAround(Vector3.zero, Vector3.up, angle);

        transform.position += moveReference.position * Time.deltaTime;

        playerAnimator.SetBool("isRunning", moveReference.position != Vector3.zero);
        playerAnimator.SetBool("isWalking", wDown);
    }

    void Turn()
    {
        if (isTargetMoving)
            transform.LookAt(targetPos);
        else
            transform.LookAt(transform.position + moveReference.position);
    }

    void SetTargetPos(Vector3 target)
    {
        targetPos = new Vector3(target.x, transform.position.y, target.z);
        isTargetMoving = true;
    }

    void TargetMove()
    {
        if (!isTargetMoving) return;

        if(targetNpc == null)
        {
            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                isTargetMoving = false;
                return;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, targetPos) < 1.2f)
            {
                transform.LookAt(targetNpc.transform.position);
                targetNpc.GetComponent<Npc>().Turn(transform.position);
                gameManager.StartConversation();
                targetNpc = null;
                isTargetMoving = false;
                return;
            }
        }

        float distanceDelta = wDown ? speed * walkOffset * Time.deltaTime : speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, distanceDelta);
        playerAnimator.SetBool("isRunning", isTargetMoving);
        playerAnimator.SetBool("isWalking", wDown);
    }

}
