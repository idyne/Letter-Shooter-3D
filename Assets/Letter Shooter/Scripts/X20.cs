using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class X20 : MonoBehaviour
{
    [SerializeField] private Transform[] positions = null;

    public Vector3 GetPosition(int index)
    {
        return positions[index - 1].position;
    }
}
