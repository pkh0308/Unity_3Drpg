using UnityEngine;
using UnityEngine.AI;

//플레이어에게 공격받는 순간 타겟으로 잡고 따라옴
//타겟이 searchDistance보다 멀어질 경우 타겟 해제
public class Enemy_Normal : Enemy
{
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

        type = EnemyType.Normal;
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
            //if (animator.GetBool(AnimationVar.isMoving.ToString()) == false)
                animator.SetBool(AnimationVar.isMoving.ToString(), true);
            return;
        }
        //타겟이 없을 경우 일정 시간마다 랜덤 이동
        curMove += Time.deltaTime;
        if (curMove >= ranMovMax)
        {
            nav.SetDestination(transform.position + ranDirs[Random.Range(0, 4)]);
            curMove = 0;
            //if (animator.GetBool(AnimationVar.isMoving.ToString()) == false)
                animator.SetBool(AnimationVar.isMoving.ToString(), true);
            return;
        }

        //if (animator.GetBool(AnimationVar.isMoving.ToString()) == true)
            animator.SetBool(AnimationVar.isMoving.ToString(), false);
    }

    void DistanceCheck()
    {
        if (target == null) return;
        if (Vector3.Distance(transform.position, target.position) > searchDistance)
            target = null;
    }
}
