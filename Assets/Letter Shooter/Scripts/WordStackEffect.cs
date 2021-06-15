using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordStackEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particleSystems = null;

    public void Appear(float height)
    {
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            ParticleSystem.ShapeModule shape = particleSystem.shape;
            Vector3 scale = shape.scale;
            scale.z = height;
            shape.scale = scale;
            Vector3 pos = transform.position;
            pos.y = height / 2;
            transform.position = pos;
        }
    }
}
