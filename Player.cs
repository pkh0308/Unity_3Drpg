using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    float hAxis;
    float vAxis;
    bool wDown;
    bool mouseLeft;
    bool mouseLeftDown;
    public bool isTargetMoving;
    bool isCollecting;
    int mask;

    [SerializeField] Transform moveReference;
    Vector3 targetPos;
    public GameObject target;
    public float speed;
    public float walkOffset;
    public Transform cameraReference;

    public Animator playerAnimator;
    public GameManager gameManager;
    public UiManager uiManager;

    enum AnimationVar { isRunning, isWalking, isCollecting, collectDone }
    enum Tags { Player, Platform, Npc, Collectable }
    enum Axis { Horizontal, Vertical }

    void Awake()
    {
        mask = (-1) - (1 << LayerMask.NameToLayer(Tags.Player.ToString()));
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
        hAxis = Input.GetAxisRaw(Axis.Horizontal.ToString());
        vAxis = Input.GetAxisRaw(Axis.Vertical.ToString());
        wDown = Input.GetKey(KeyCode.LeftShift);

        if(Input.GetKeyDown(KeyCode.I))
        {
            gameManager.InventoryControll();
        }

        mouseLeft = Input.GetMouseButton(0);
        mouseLeftDown = Input.GetMouseButtonDown(0);
        if (!mouseLeft) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit rayHit, Mathf.Infinity, mask))
        {
            switch (rayHit.collider.tag)
            {
                case "Platform":
                    SetTargetPos(rayHit.point);
                    break;
                case "Npc":
                case "Collectable":
                    if (!isTargetMoving && mouseLeftDown)
                    {
                        target = rayHit.collider.gameObject;
                        SetTargetPos(rayHit.point);
                    }
                    break;
                default:
                    target = null;
                    break;
            }
        }
    }

    void Move()
    {
        if (isCollecting) return;

        float angle = cameraReference.eulerAngles.y;
        moveReference.position = wDown ? new Vector3(hAxis, 0, vAxis).normalized * speed * walkOffset
                                       : new Vector3(hAxis, 0, vAxis).normalized * speed;
        moveReference.RotateAround(Vector3.zero, Vector3.up, angle);

        transform.position += moveReference.position * Time.deltaTime;

        playerAnimator.SetBool(AnimationVar.isRunning.ToString(), moveReference.position != Vector3.zero);
        playerAnimator.SetBool(AnimationVar.isWalking.ToString(), wDown);
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
        if (!isTargetMoving || isCollecting) return;

        if(target == null)
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
                Turn();
                switch (target.tag)
                {
                    case "Npc":
                        target.GetComponent<Npc>().Turn(transform.position);
                        gameManager.StartConversation();
                        break;
                    case "Collectable":
                        ICollectable collectLogic = target.GetComponent<ICollectable>();
                        StartCoroutine(CollectItem(collectLogic.SpendTime));
                        collectLogic.StartCollect();
                        gameManager.ProgressStart(target.tag, collectLogic.SpendTime);
                        break;
                }
                target = null;
                isTargetMoving = false;
                return;
            }
        }

        float distanceDelta = wDown ? speed * walkOffset * Time.deltaTime : speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, distanceDelta);
        playerAnimator.SetBool(AnimationVar.isRunning.ToString(), isTargetMoving);
        playerAnimator.SetBool(AnimationVar.isWalking.ToString(), wDown);
    }

    IEnumerator CollectItem(float time)
    {
        isCollecting = true;
        playerAnimator.SetBool(AnimationVar.isCollecting.ToString(), true);

        yield return new WaitForSeconds(time);
        isCollecting = false;
        playerAnimator.SetBool(AnimationVar.isCollecting.ToString(), false);
        playerAnimator.SetTrigger(AnimationVar.collectDone.ToString());
    }
}
