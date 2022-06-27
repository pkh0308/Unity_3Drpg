using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Peaceful : MonoBehaviour, IEnemy
{
    [SerializeField] int enemyId;
    public int EnemyId { get { return enemyId; } }
    [SerializeField] int maxHp;
    public int MaxHp { get { return maxHp; } }
    int curHp;
    public int CurHp { get { return curHp; } }
    [SerializeField] int attackPower;
    public int AttackPower { get { return attackPower; } }

    bool onCombat;
    [SerializeField] Transform target;
    [SerializeField] float speed;
    [SerializeField] float stopDistance;
    [SerializeField] float ranMovMax;
    float curMove;
    Vector3[] ranDirs;

    [SerializeField] float attackTime;
    WaitForSeconds attackTimeOffset;

    NavMeshAgent nav;
    Rigidbody rigid;
    Animator animator;

    public enum EnemyType { Peaceful, Normal, Aggressive }
    enum AnimationVar { isMoving, doAttack, onDie}
    public EnemyType type;

    public void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        nav.speed = speed;
        nav.stoppingDistance = stopDistance;

        ranDirs = new Vector3[] { Vector3.left, Vector3.right, Vector3.forward, Vector3.back};
        attackTimeOffset = new WaitForSeconds(attackTime);
    }

    public void Initialize(int maxHp)
    {
        this.maxHp = maxHp;
        curHp = maxHp;
    }

    void Update()
    {
        Move();
    }

    public void Move()
    {
        if (onCombat) return;
        //타겟이 설정된 경우 navmesh로 추적
        if (target != null)
        {
            nav.SetDestination(target.position);

            if (Vector3.Distance(transform.position, target.position) < nav.stoppingDistance) 
                StartCoroutine(Attack());
            if(animator.GetBool(AnimationVar.isMoving.ToString()) == false) 
                animator.SetBool(AnimationVar.isMoving.ToString(), true);
            return;
        }
        //타겟이 없을 경우 일정 시간마다 랜덤 이동
        curMove += Time.deltaTime;
        if(curMove >= ranMovMax)
        {
            nav.SetDestination(transform.position + ranDirs[Random.Range(0, 4)]);
            curMove = 0;
            if (animator.GetBool(AnimationVar.isMoving.ToString()) == false)
                animator.SetBool(AnimationVar.isMoving.ToString(), true);
            return;
        }

        if (animator.GetBool(AnimationVar.isMoving.ToString()) == true)
            animator.SetBool(AnimationVar.isMoving.ToString(), false);
    }

    IEnumerator Attack()
    {
        onCombat = true;
        //공격 애니메이션
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
        if(curHp - dmg > 0)
        {
            curHp -= dmg;
            //피격 애니메이션
        }
        else
        {
            curHp = 0;
            OnDie();
        }
    }

    public void OnDie()
    {
        StopAllCoroutines();
        onCombat = false;
        target = null;

        //사망 애니메이션
        animator.SetTrigger(AnimationVar.onDie.ToString());
    }
}
