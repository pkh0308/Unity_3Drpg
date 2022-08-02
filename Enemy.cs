using System.Collections;
using UnityEngine;
using UnityEngine.AI;

//Enemy들의 기본 로직(이동, 공격 및 피격, 사망) 상속용 클래스
//이벤트 주기 함수(Awake, Update 등)는 상속 받은 클래스에서 선언
//EnemyType에 따라 달라지는 함수들은 상속 받은 클래스에서 선언
public class Enemy : MonoBehaviour
{
    //기본 스테이터스 정보
    [SerializeField] protected int enemyId;
    public int EnemyId { get { return enemyId; } }
    protected int maxHp;
    public int MaxHp { get { return maxHp; } }
    protected int curHp;
    protected int attackPower;
    public int AttackPower { get { return attackPower; } }
    [SerializeField] protected float attackTime;
    protected WaitForSeconds attackTimeOffset;
    protected string enemyName;
    public string EnemyName { get { return enemyName; } }

    //전투 관련 트리거
    protected bool onCombat;
    protected bool onAttacked;
    protected bool isDie;
    public bool IsDie { get { return isDie; } }

    //UI 관련
    protected GameObject hpBarSet;
    protected HpBar hpBar;
    [SerializeField] protected Vector3 hpBarOffset;

    protected Transform target;
    protected Player player;
    [SerializeField] protected float speed;
    [SerializeField] protected float stopDistance;
    [SerializeField] protected float ranMovMax;
    protected float curMove;
    protected Vector3[] ranDirs;
    [SerializeField] protected float ranMoveOffset;
    [SerializeField] protected float searchDistance;
    [SerializeField] protected float disappearTime;

    protected NavMeshAgent nav;
    protected Animator animator;
    protected BoxCollider coll;

    public enum EnemyType { Peaceful, Normal, Aggressive }
    protected enum AnimationVar { isMoving, doAttack, onDamaged, onDie }
    protected EnemyType type;
    public EnemyType Type { get { return type; } }

    //ObjectManager에서 최초 생성 시 호출
    //최대체력 및 공격력, 이름을 설정
    public void Initialize(EnemyData data)
    {
        maxHp = data.maxHp;
        curHp = maxHp;
        attackPower = data.attackPower;
        enemyName = data.enemyName;
    }

    //애니메이션 설정 후 플레이어의 피격 로직 호출
    //플레이어 사망 시 전투 관련 변수들 초기화 및 체력바 미노출
    protected IEnumerator Attack()
    {
        onCombat = true;
        transform.LookAt(target.transform.position);
        animator.SetBool(AnimationVar.isMoving.ToString(), false);
        animator.SetTrigger(AnimationVar.doAttack.ToString());

        //플레이어 피격 로직 호출
        if (target.TryGetComponent<Player>(out Player p) == false)
        {
            Debug.Log("It's not a player...");
            yield break;
        }
        player = p;
        player.OnDamaged(attackPower);
        if (player.IsDied)
        {
            target = null;
            onCombat = false;
            onAttacked = false;
            hpBarSet.SetActive(false);
            yield break;
        }
        yield return attackTimeOffset;
        onCombat = false;
    }

    //타겟이 없는 상태에서 피격 시 타겟으로 플레이어 설정(Normal 타입용)
    //체력이 0이 되는 경우 사망 로직 호출
    public void OnDamaged(int dmg)
    {
        if (target == null)
            target = Player.getPlayer().gameObject.transform;

        if (curHp - dmg > 0)
        {
            curHp -= dmg;
            StartCoroutine(Damaged());
            SetHpBar();
        }
        else
        {
            curHp = 0;
            OnDie();
        }
    }

    protected IEnumerator Damaged()
    {
        onAttacked = true;

        yield return new WaitForSeconds(1.0f);
        onAttacked = false;
    }

    //체력바를 노출한 적이 없으면 ObjectManager에서 할당받아 세팅
    //현재 자신의 스크린상의 좌표를 WorldToScreenPoint 로 받은 뒤, hpBarOffset 을 더하여 위치 조정
    protected void SetHpBar()
    {
        if (hpBar == null)
        {
            hpBarSet = ObjectManager.makeObj(ObjectNames.hpBar);
            hpBar = hpBarSet.GetComponent<HpBar>();
        }
        else
            hpBarSet.SetActive(true);

        hpBar.UpdatePos(Camera.main.WorldToScreenPoint(transform.position), hpBarOffset);
        hpBar.UpdateScale(curHp, maxHp);
    }

    //(현재 체력/최대 체력) 값을 체력바의 x축 스케일로 설정하여 체력 표시  
    protected void UpdateHpBar()
    {
        if (hpBar == null) return;
        if (!hpBarSet.activeSelf) return;
        
        hpBar.UpdatePos(Camera.main.WorldToScreenPoint(transform.position), hpBarOffset);
        hpBar.UpdateScale(curHp, maxHp);
    }

    //모든 코루틴 취소 및 isDie 값을 true로 설정하여 다른 동작 정지
    //기타 변수들을 초기화 및 킬 퀘스트 검사
    protected void OnDie()
    {
        StopAllCoroutines();
        isDie = true;
        coll.enabled = false;
        onCombat = false;
        onAttacked = false;
        target = null;
        if(hpBarSet != null) hpBarSet.SetActive(false);
        QuestManager.Instance.UpdateKillQuest(enemyId);

        StartCoroutine(Die());
    }

    protected IEnumerator Die()
    {
        animator.SetTrigger(AnimationVar.onDie.ToString());

        yield return new WaitForSeconds(disappearTime);
        gameObject.SetActive(false);
    }
}
