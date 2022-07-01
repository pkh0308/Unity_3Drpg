using UnityEngine;
using UnityEngine.AI;

//플레이어가 searchDistance 안에 들어올 경우 바로 타겟으로 설정
//Physics.OverlapSphere() 함수로 searchDistance 내부에 플레이어가 있는지 체크
public class Enemy_Aggressive : Enemy
{
    int playerMask;
    [SerializeField] float searchDistance;

    void Awake()
    {
        coll = GetComponent<BoxCollider>();
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        nav.speed = speed;
        nav.stoppingDistance = stopDistance;

        ranDirs = new Vector3[] { ranMoveOffset * Vector3.left, ranMoveOffset * Vector3.right, ranMoveOffset * Vector3.forward, ranMoveOffset * Vector3.back };
        attackTimeOffset = new WaitForSeconds(attackTime);
        curHp = maxHp;

        type = EnemyType.Aggressive;
        playerMask = (1 << LayerMask.NameToLayer(Tags.Player.ToString()));
    }

    void Update()
    {
        Move();
        DistanceCheck();
    }

    void Move()
    {
        if (onCombat || isDie) return;
        //타겟이 설정된 경우 navmesh로 추적
        if (target != null)
        {
            nav.SetDestination(target.position);

            if (Vector3.Distance(transform.position, target.position) < nav.stoppingDistance)
                StartCoroutine(Attack());
            else
                animator.SetBool(AnimationVar.isMoving.ToString(), true);
            return;
        }
        //타겟이 없을 경우 일정 시간마다 랜덤 이동
        curMove += Time.deltaTime;
        if (curMove >= ranMovMax)
        {
            nav.SetDestination(transform.position + ranDirs[Random.Range(0, 4)]);
            curMove = 0;
            animator.SetBool(AnimationVar.isMoving.ToString(), true);
            return;
        }

        if (animator.GetBool(AnimationVar.isMoving.ToString()) == true)
            animator.SetBool(AnimationVar.isMoving.ToString(), false);
    }

    void DistanceCheck()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, searchDistance, playerMask);
        if (colls.Length > 0)
            target = colls[0].gameObject.transform;
        else
            target = null;
    }
}
