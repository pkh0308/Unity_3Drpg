using System.Collections;
using UnityEngine;
using UnityEngine.AI;

//Enemy들의 기본 로직(이동, 공격 및 피격, 사망) 상속용 클래스
//이벤트 주기 함수(Awake, Update 등)는 상속 받은 클래스에서 선언
//상속 후 EnemyType에 따라 달라지는 부분만 new 키워드로 재선언
public class Enemy : MonoBehaviour
{
    [SerializeField] protected int enemyId;
    public int EnemyId { get { return enemyId; } }
    [SerializeField] protected int maxHp;
    public int MaxHp { get { return maxHp; } }
    protected int curHp;
    [SerializeField] protected int attackPower;
    public int AttackPower { get { return attackPower; } }
    [SerializeField] protected float attackTime;
    protected WaitForSeconds attackTimeOffset;

    protected bool onCombat;
    protected bool isDie;
    public bool IsDie { get { return isDie; } }

    protected Transform target;
    [SerializeField] protected float speed;
    [SerializeField] protected float stopDistance;
    [SerializeField] protected float ranMovMax;
    protected float curMove;
    protected Vector3[] ranDirs;
    [SerializeField] protected float ranMoveOffset;

    protected NavMeshAgent nav;
    protected Animator animator;
    protected BoxCollider coll;

    public enum EnemyType { Peaceful, Normal, Aggressive }
    protected enum AnimationVar { isMoving, doAttack, onDamaged, onDie }
    protected EnemyType type;
    public EnemyType Type { get { return type; } }

    protected void Initialize()
    {
        coll = GetComponent<BoxCollider>();
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        nav.speed = speed;
        nav.stoppingDistance = stopDistance;

        ranDirs = new Vector3[] { ranMoveOffset * Vector3.left, ranMoveOffset * Vector3.right, ranMoveOffset * Vector3.forward, ranMoveOffset * Vector3.back };
        attackTimeOffset = new WaitForSeconds(attackTime);
        curHp = maxHp;
    }

    protected IEnumerator Attack()
    {
        onCombat = true;
        animator.SetBool(AnimationVar.isMoving.ToString(), false);
        animator.SetTrigger(AnimationVar.doAttack.ToString());

        //플레이어 피격 로직 호출
        if (target.TryGetComponent<Player>(out Player player) == false)
            Debug.Log("It's not a player...");
        else
            player.OnDamaged(attackPower);

        yield return attackTimeOffset;
        onCombat = false;
    }

    public void OnDamaged(int dmg)
    {
        if (type == EnemyType.Normal && target == null)
            target = Player.getPlayer().gameObject.transform;

        if (curHp - dmg > 0)
        {
            curHp -= dmg;
            StartCoroutine(Damaged());
        }
        else
        {
            curHp = 0;
            OnDie();
        }
    }

    protected IEnumerator Damaged()
    {
        onCombat = true;
        animator.SetTrigger(AnimationVar.onDamaged.ToString());

        yield return new WaitForSeconds(1.0f);
        onCombat = false;
    }

    protected void OnDie()
    {
        StopAllCoroutines();
        isDie = true;
        coll.enabled = false;
        onCombat = false;
        target = null;
        StartCoroutine(Die());
    }

    protected IEnumerator Die()
    {
        animator.SetTrigger(AnimationVar.onDie.ToString());

        yield return new WaitForSeconds(3.0f);
        gameObject.SetActive(false);
    }
}
