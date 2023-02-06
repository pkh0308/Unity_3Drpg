using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

//플레이어에게 공격받는 순간 타겟으로 잡고 따라옴
//타겟이 searchDistance보다 멀어질 경우 타겟 해제
public class Enemy_Normal : Enemy
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

        type = EnemyType.Normal;
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
        if (onCombat) return;

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
        if (target == null) return;
        if (player != null && player.IsDied) return;

        if (Vector3.Distance(transform.position, target.position) > searchDistance)
        {
            target = null;
            player = null;
            if (hpBarSet != null) hpBarSet.SetActive(false);
        }
    }
}