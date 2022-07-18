using UnityEngine;
using UnityEngine.AI;

//플레이어가 searchDistance 안에 들어올 경우 바로 타겟으로 설정
//Physics.OverlapSphere() 함수로 searchDistance 내부에 플레이어가 있는지 체크
public class Enemy_Aggressive : Enemy
{
    int playerMask;

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
        playerMask = (1 << LayerMask.NameToLayer(Tags.Player.ToString())); //OverlapSphere()용 마스크, player 레이어만 탐색
    }

    void Update()
    {
        if (isDie) return;
        Move();
        DistanceCheck();
        UpdateHpBar();
    }

    void Move()
    {
        if (onCombat || onAttacked) return;
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

        if (Vector3.Distance(transform.position, nav.destination) <= nav.stoppingDistance)
            animator.SetBool(AnimationVar.isMoving.ToString(), false);
    }

    void DistanceCheck()
    {
        if (playerDie) return;
        //타겟이 미설정된 경우 OverlapSphere()로 플레이어 탐지
        if (target == null)
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, searchDistance, playerMask);
            if (colls.Length > 0)
                target = colls[0].gameObject.transform;
            return;
        }
        // 타겟이 설정된 경우 타겟과의 거리 측정
        if (Vector3.Distance(transform.position, target.position) > searchDistance)
        {
            target = null;
            if (hpBarSet != null) hpBarSet.SetActive(false);
        }
            
    }
}
