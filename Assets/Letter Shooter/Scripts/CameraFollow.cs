using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    [SerializeField] private float speed = 1;
    private bool follow = true;
    public Vector3 Offset = Vector3.zero;
    [SerializeField] private Transform finalShootPoint = null;

    public Transform Target
    {
        get
        {
            return target;
        }
        set
        {
            target = value;
            transform.position = Target.position + Offset;
        }
    }

    private void Awake()
    {
        //offset = transform.position - Target.position;

    }

    private void LateUpdate()
    {
        if (Target && follow)
            Follow();
    }

    private void Follow()
    {
        transform.position = Vector3.MoveTowards(transform.position, Target.position + Offset, Time.deltaTime * speed);
    }

    public void StopFollowing()
    {
        follow = false;
    }

    public void StartFollowing()
    {
        follow = true;
    }

    public LTDescr GetToFinalShootPoint()
    {
        StopFollowing();
        transform.LeanMove(finalShootPoint.position, 0.4f);
        return transform.LeanRotate(finalShootPoint.rotation.eulerAngles, 0.4f);
    }
}
