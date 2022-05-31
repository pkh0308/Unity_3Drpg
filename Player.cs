using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    float hAxis;
    float vAxis;
    bool wDown;
    bool mouseLeft;
    bool mouseLeftDown;
    bool isTargetMoving;
    bool isCollecting;
    bool isOverUi;
    int playerMask;

    [SerializeField] GraphicRaycaster uiRaycaster;
    PointerEventData p_data;

    [SerializeField] Transform moveReference;
    Vector3 targetPos;
    GameObject target;
    public float speed;
    public float walkOffset;
    public Transform cameraReference;

    public Animator playerAnimator;
    public GameManager gameManager;
    public UiManager uiManager;
    public CursorManager cursorManger;

    enum AnimationVar { isRunning, isWalking, isCollecting, collectDone }
    enum Axis { Horizontal, Vertical }

    void Awake()
    {
        playerMask = (-1) - (1 << LayerMask.NameToLayer(Tags.Player.ToString()));
        p_data = new PointerEventData(null);
    }

    void Update()
    {
        InputCheck();
        if (gameManager.Pause) return;

        Move();
        TargetMove();
        Turn();
    }

    void InputCheck()
    {
        hAxis = Input.GetAxisRaw(Axis.Horizontal.ToString());
        vAxis = Input.GetAxisRaw(Axis.Vertical.ToString());
        wDown = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown(KeyCode.I)) uiManager.ControlInventorySet();
        if (Input.GetKeyDown(KeyCode.Q)) uiManager.ControlQuestSet();

        mouseLeft = Input.GetMouseButton(0);
        mouseLeftDown = Input.GetMouseButtonDown(0);
        isOverUi = EventSystem.current.IsPointerOverGameObject();

        if (gameManager.IsDraging) return;
        if (isOverUi)
        {
            p_data.position = Input.mousePosition;
            List<RaycastResult> list = new List<RaycastResult>();
            uiRaycaster.Raycast(p_data, list);

            if (list.Count > 0)
            {
                if (list[0].gameObject.TryGetComponent(out ItemSlot slot))
                    uiManager.ItemDescOn(slot.ItemId);
                else if (list[0].gameObject.TryGetComponent(out ShopItem shop))
                    if(shop.Data != null) 
                        uiManager.ShopDescOn(shop.Data.itemId);
            }
            return;
        }
        uiManager.ItemDescOff();
        uiManager.ShopDescOff();

        if (gameManager.Pause) return;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit rayHit, Mathf.Infinity, playerMask))
        {
            switch (rayHit.collider.tag)
            {
                case Tags.Platform:
                    cursorManger.CursorChange((int)CursorManager.CursorIndexes.DEFAULT);
                    if (mouseLeft) SetTargetPos(rayHit.point);
                    break;
                case Tags.Npc:
                    cursorManger.CursorChange((int)CursorManager.CursorIndexes.CONV);
                    SetTargetPos(rayHit);
                    break;
                case Tags.Collectable:
                    cursorManger.CursorChange((int)CursorManager.CursorIndexes.COLLECT);
                    if (!isTargetMoving && mouseLeftDown)
                    {
                        target = rayHit.collider.gameObject;
                        SetTargetPos(rayHit);
                    }
                    break;
                case Tags.Entrance:
                    cursorManger.CursorChange((int)CursorManager.CursorIndexes.ENTRANCE);
                    if (!isTargetMoving && mouseLeftDown)
                    {
                        target = rayHit.collider.gameObject;
                        SetTargetPos(rayHit);
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

    //마우스 이동 시 호출
    void SetTargetPos(Vector3 target)
    {
        targetPos = new Vector3(target.x, transform.position.y, target.z);
        isTargetMoving = true;
    }

    //오브젝트 클릭 시 호출
    void SetTargetPos(RaycastHit rayHit)
    {
        if (isTargetMoving || !mouseLeftDown) return;
        
        target = rayHit.collider.gameObject;
        targetPos = new Vector3(rayHit.point.x, transform.position.y, rayHit.point.z);
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
            if (Vector3.Distance(transform.position, targetPos) < 1.0f)
            {
                Turn();
                switch (target.tag)
                {
                    case Tags.Npc:
                        Npc npcLogic = target.GetComponent<Npc>();
                        npcLogic.Turn(transform.position);
                        gameManager.Conv_Start(npcLogic.NpcName, npcLogic.NpcId, npcLogic.HasShop);
                        break;
                    case Tags.Collectable:
                        ICollectable collectLogic = target.GetComponent<ICollectable>();
                        StartCoroutine(CollectItem(collectLogic.SpendTime, collectLogic.ItemId, collectLogic.ItemCount));
                        collectLogic.StartCollect();
                        gameManager.ProgressStart(target.tag, collectLogic.SpendTime);
                        break;
                    case Tags.Entrance:
                        Entrance enterLogic = target.GetComponent<Entrance>();
                        transform.position = enterLogic.GetPos();
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

    IEnumerator CollectItem(float time, int id, int count)
    {
        isCollecting = true;
        playerAnimator.SetBool(AnimationVar.isCollecting.ToString(), true);

        yield return new WaitForSeconds(time);
        isCollecting = false;
        playerAnimator.SetBool(AnimationVar.isCollecting.ToString(), false);
        playerAnimator.SetTrigger(AnimationVar.collectDone.ToString());
        gameManager.GetItem(id, count);
    }
}
