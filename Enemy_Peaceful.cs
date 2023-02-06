using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

//플레이어에게 공격받아도 반격하지 않는 타입
//타겟이 항상 null이므로 Move 함수에서 타겟 관련 내용 제외
public class Enemy_Peaceful : Enemy
{
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        coll = GetComponent<BoxCollider>();
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        nav.speed = speed;
        nav.stoppingDistance = stopDistance;

        ranDirs = new Vector3[] { ranMoveOffset * Vector3.left, ranMoveOffset * Vector3.right, ranMoveOffset * Vector3.forward, ranMoveOffset * Vector3.back };
        attackTimeOffset = new WaitForSeconds(attackTime);

        type = EnemyType.Peaceful;
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
        //일정 시간마다 랜덤 이동
        curMove += Time.deltaTime;
        if (curMove >= ranMovMax)
        {
            nav.SetDestination(transform.position + ranDirs[Random.Range(0, 4)]);
            curMove = 0;
            if (animator.GetBool(AnimationVar.isMoving.ToString()) == false)
                animator.SetBool(AnimationVar.isMoving.ToString(), true);
            return;
        }

        if (Vector3.Distance(transform.position, nav.destination) <= nav.stoppingDistance)
            animator.SetBool(AnimationVar.isMoving.ToString(), false);
    }

    void DistanceCheck()
    {
        if (target == null) return;
        if (Vector3.Distance(transform.position, target.position) > searchDistance)
        {
            target = null;
            if (hpBarSet != null) hpBarSet.SetActive(false);
        }
    }
}