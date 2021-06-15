using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkySpotlight : MonoBehaviour
{
    [SerializeField] private Transform head = null;
    [SerializeField] private float angle = 20;
    [SerializeField] private float time = 5;
    private void Start()
    {
        StartMove();
    }

    private void StartMove()
    {
        head.localRotation = Quaternion.identity;
        head.RotateAround(head.position, head.forward, -angle);
        head.LeanRotateAround(head.forward, angle * 2, time).setEaseInOutQuart().setLoopPingPong();
    }
}
