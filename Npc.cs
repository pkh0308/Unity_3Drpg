using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
    GameObject target;

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    public void Turn(Vector3 pos)
    {
        transform.LookAt(pos);
    }
}
