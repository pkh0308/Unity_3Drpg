using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //플레이어 체크용
    public static Func<Player> getPlayer;
    Player GetPlayer() { return this; }

    float hAxis;
    float vAxis;
    bool wDown;
    bool mouseLeft;
    bool mouseLeftDown;
    bool isTargetMoving;
    bool isCollecting;
    bool isOverUi;
    bool inBuilding;
    int playerMask;

    [SerializeField] GameObject mainCamera;
    [SerializeField] GraphicRaycaster uiRaycaster;
    PointerEventData p_data;

    [SerializeField] Transform moveReference;
    public Vector3 targetPos;
    public GameObject target;
    [SerializeField] float speed;
    [SerializeField] float walkOffset;
    [SerializeField] Transform cameraReference;

    [SerializeField] Animator playerAnimator;
    [SerializeField] GameManager gameManager;
    [SerializeField] UiManager uiManager;
    [SerializeField] CursorManager cursorManger;

    enum AnimationVar { isRunning, isWalking, isCollecting, collectDone, doAttack, onDamaged, onDie, onRevive }
    enum Axis { Horizontal, Vertical }

    public bool onUi;
    public bool OnUi { get { return onUi; } }

    //전투 관련
    bool onCombat;
    bool onAttacked;
    bool isDied;
    public bool IsDied { get { return isDied; } }
    [SerializeField] int maxHp;
    public int MaxHp { get { return maxHp; } }
    int curHp;
    public int CurHp { get { return curHp; } }
    [SerializeField] int hpRecovery;
    [SerializeField] float hpRecoverySeconds;
    WaitForSeconds hpSeconds;
    [SerializeField] int maxSp;
    public int MaxSp { get { return maxSp; } }
    int curSp;
    public int CurSp { get { return curSp; } }
    [SerializeField] int spRecovery;
    [SerializeField] float spRecoverySeconds;
    WaitForSeconds spSeconds;
    [SerializeField] float attackTime;
    [SerializeField] int attackPower;
    WaitForSeconds attackTimeOffset;
    Coroutine damagedRoutine;
    
    void Awake()
    {
        playerMask = (-1) - (1 << LayerMask.NameToLayer(Tags.Player.ToString()));
        p_data = new PointerEventData(null);

        attackTimeOffset = new WaitForSeconds(attackTime);
        curHp = maxHp;
        curSp = maxSp;
        hpSeconds = new WaitForSeconds(hpRecoverySeconds);
        spSeconds = new WaitForSeconds(spRecoverySeconds);

        getPlayer = () => { return GetPlayer(); };
    }

    void Start()
    {
        uiManager.StsBar_HpUpdate(curHp, maxHp);
        uiManager.StsBar_SpUpdate(curSp, maxSp);//GoodsManager.Instance.GetGold(10000);
    }

    void Update()
    {
        if (isDied) return;
        InputCheck();
        if (gameManager.Pause) return;

        Move();
        TargetMove();
        Turn();
    }

    void InputCheck()
    {
        //유저 UI창 관련 인풋
        if (Input.GetKeyDown(KeyCode.I)) uiManager.ControlInventorySet();
        if (Input.GetKeyDown(KeyCode.Q)) uiManager.ControlQuestSet();
        if (Input.GetKeyDown(KeyCode.Escape)) uiManager.CloseWindows();

        //키보드 이동 관련 인풋
        hAxis = Input.GetAxisRaw(Axis.Horizontal.ToString());
        vAxis = Input.GetAxisRaw(Axis.Vertical.ToString());
        wDown = Input.GetKey(KeyCode.LeftShift);

        //마우스 인풋
        mouseLeft = Input.GetMouseButton(0);
        mouseLeftDown = Input.GetMouseButtonDown(0);
        isOverUi = EventSystem.current.IsPointerOverGameObject();

        if (gameManager.IsDraging) return;
        //마우스가 UI 위에 있을 경우
        //인벤토리나 상점일 경우 아이템 디스크립션 노출
        if (isOverUi)
        {
            onUi = true;

            p_data.position = Input.mousePosition;
            List<RaycastResult> list = new List<RaycastResult>();
            uiRaycaster.Raycast(p_data, list);

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].gameObject.CompareTag(Tags.UI) == false)
                    continue;

                if (list[i].gameObject.TryGetComponent(out ItemSlot slot))
                {
                    uiManager.ItemDescOn(slot.ItemId);
                    if (Input.GetMouseButtonUp(1)) gameManager.UseItem(slot.ItemId);
                }
                else if (list[i].gameObject.TryGetComponent(out ShopItem shop))
                    if (shop.Data != null)
                        uiManager.ShopDescOn(shop.Data.itemId);
            }
            return;
        }
        uiManager.ItemDescOff();
        uiManager.ShopDescOff();
        onUi = false;

        //마우스가 UI 위에 있지 않을 경우
        //Raycast로 플레이어를 제외한 타겟을 검출하고 타겟쪽으로 이동 및 커서 변경, 타겟에 따라 추가 행동(대화, 채집 등)
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
                    SetTargetPos(rayHit);
                    break;
                case Tags.Entrance:
                case Tags.StageDoor:
                    cursorManger.CursorChange((int)CursorManager.CursorIndexes.ENTRANCE);
                    SetTargetPos(rayHit);
                    break;
                case Tags.Enemy:
                    cursorManger.CursorChange((int)CursorManager.CursorIndexes.Enemy);
                    SetTargetPos(rayHit);
                    break;
                default:
                    cursorManger.CursorChange((int)CursorManager.CursorIndexes.DEFAULT);
                    break;
            }
        }
    }

    void Move()
    {
        if (isCollecting || onCombat || onAttacked) return;

        moveReference.position = wDown ? speed * walkOffset * new Vector3(hAxis, 0, vAxis).normalized
                                       : speed * new Vector3(hAxis, 0, vAxis).normalized;
        
        float angle = cameraReference.eulerAngles.y;
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
        if (onCombat) return;

        targetPos = new Vector3(target.x, transform.position.y, target.z);
        isTargetMoving = true;
    }

    //오브젝트 클릭 시 호출
    void SetTargetPos(RaycastHit rayHit)
    {
        if (isTargetMoving || !mouseLeftDown) return;
        if(rayHit.collider.gameObject.CompareTag(Tags.Collectable) || rayHit.collider.gameObject.CompareTag(Tags.Entrance))
            target = rayHit.collider.gameObject;

        target = rayHit.collider.gameObject;
        targetPos = new Vector3(rayHit.point.x, transform.position.y, rayHit.point.z);
        isTargetMoving = true;
    }

    void TargetMove()
    {
        if (onCombat) return;
        if (isTargetMoving == false || isCollecting) return;

        if (target == null)
        {
            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                isTargetMoving = false;
                return;
            }
        }
        else
        {
            if (target.transform.position != targetPos)
                targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);

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
                        if (curSp < collectLogic.SpCount) 
                        { 
                            uiManager.MidNotice(UiManager.NoticeType.NotEnoughSp); 
                            break; 
                        }
                        StartCoroutine(CollectItem(collectLogic.SpendTime, collectLogic.ItemId, collectLogic.ItemCount, collectLogic.SpCount));
                        collectLogic.StartCollect();
                        gameManager.ProgressStart(target.tag, collectLogic.SpendTime);
                        break;
                    case Tags.Entrance:
                        Entrance enterLogic = target.GetComponent<Entrance>();
                        transform.position = enterLogic.Enter();
                        mainCamera.SetActive(mainCamera.activeSelf == false);
                        inBuilding = (inBuilding == false);
                        break;
                    case Tags.StageDoor:
                        LoadingSceneManager.enterStage(target.GetComponent<StageDoor>().StageIdx);
                        break;
                    case Tags.Enemy:
                        StartCoroutine(Attack());
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

    IEnumerator CollectItem(float time, int id, int count, int spCount)
    {
        isCollecting = true;
        bool wasMaxSp = (curSp == maxSp);
        UpdateSp(spCount, false);
        //갱신 전에 Sp가 가득 찬 상태였다면 회복 코루틴 시작
        if (wasMaxSp) StartCoroutine(SpRecovery());

        playerAnimator.SetBool(AnimationVar.isCollecting.ToString(), true);

        yield return new WaitForSeconds(time);
        isCollecting = false;
        playerAnimator.SetBool(AnimationVar.isCollecting.ToString(), false);
        playerAnimator.SetTrigger(AnimationVar.collectDone.ToString());
        gameManager.GetItem(id, count);
    }

    //장애물에 걸릴 시 처리
    void OnCollisionEnter(Collision collision)
    {
        if (isTargetMoving == false) return;

        target = null;
        isTargetMoving = false;
    }

    //전투 관련
    IEnumerator Attack()
    {
        onCombat = true;
        playerAnimator.SetTrigger(AnimationVar.doAttack.ToString());
        //적 피격 로직 호출
        if (target.TryGetComponent<Enemy>(out Enemy enemy) == false)
            Debug.Log("It's not a enemy...");
        else
            enemy.OnDamaged(attackPower);

        yield return attackTimeOffset;
        onCombat = false;
    }

    public void OnDamaged(int dmg)
    {
        if(damagedRoutine != null) StopCoroutine(damagedRoutine);
        bool wasMaxHp = (curHp == maxHp); 

        if (UpdateHp(dmg, false) > 0)
            damagedRoutine = StartCoroutine(Damaged());
        else
        {
            StopAllCoroutines();
            StartCoroutine(Die());
        }
        //갱신 전에 Hp가 가득 찬 상태였다면 회복 코루틴 시작
        if (wasMaxHp) StartCoroutine(HpRecovery()); 
    }

    IEnumerator Damaged()
    {
        onAttacked = true;
        //playerAnimator.SetTrigger(AnimationVar.onDamaged.ToString());

        yield return new WaitForSeconds(1.0f);
        onAttacked = false;
    }

    IEnumerator Die()
    {
        cursorManger.CursorChange((int)CursorManager.CursorIndexes.DEFAULT);
        onCombat = false;
        isDied = true;
        target = null;
        isTargetMoving = false;
        playerAnimator.SetTrigger(AnimationVar.onDie.ToString());

        yield return new WaitForSeconds(1.0f);
        uiManager.SetDeadScreen();
    }

    public void ReviveAtStartPoint()
    {
        isDied = false;
        LoadingSceneManager.enterStage((int)LoadingSceneManager.SceneIndex.STAGE_1);
        transform.position = Vector3.zero;
        curHp = maxHp;
        curSp = maxSp;
        uiManager.StsBar_HpUpdate(curHp, maxHp);
        uiManager.StsBar_SpUpdate(curSp, maxSp);
        playerAnimator.SetTrigger(AnimationVar.onRevive.ToString());
        uiManager.SetDeadScreen();
    }

    public void ReViveNow()
    {
        isDied = false;
        curHp = maxHp / 10;
        curSp = maxSp / 10;
        uiManager.StsBar_HpUpdate(curHp, maxHp);
        uiManager.StsBar_SpUpdate(curSp, maxSp);
        playerAnimator.SetTrigger(AnimationVar.onRevive.ToString());
        uiManager.SetDeadScreen();
    }

    //Hp 및 Sp 갱신용 함수, 감소하는 경우에는 2번째 인자로 false 입력
    public int UpdateHp(int val, bool isPlus = true)
    {
        if (isPlus == false) val = -val; 

        if (curHp + val <= 0)
            curHp = 0;
        else if(curHp + val > maxHp)
            curHp = maxHp;
        else
            curHp += val;

        uiManager.StsBar_HpUpdate(curHp, maxHp);
        return curHp;
    }

    public int UpdateSp(int val, bool isPlus = true)
    {
        if (isPlus == false) val = -val;

        if (curSp + val <= 0)
            curSp = 0;
        else if (curSp + val > maxSp)
            curSp = maxSp;
        else
            curSp += val;

        uiManager.StsBar_SpUpdate(curSp, maxSp);
        return curSp;
    }
    
    IEnumerator HpRecovery()
    {
        while(curHp < maxHp)
        {
            yield return hpSeconds;
            UpdateHp(hpRecovery);
        }
    }

    IEnumerator SpRecovery()
    {
        while (curSp < maxSp)
        {
            yield return spSeconds;
            UpdateSp(spRecovery);
        }
    }
}
