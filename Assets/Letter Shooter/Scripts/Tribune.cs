using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tribune : MonoBehaviour
{
    [SerializeField] private GameObject spectatorPrefab = null;
    [SerializeField] private float minDistanceBetweenSpectators = 1;
    [SerializeField] private float maxDistanceBetweenSpectators = 5;
    [SerializeField] private Transform[] tribunes = null;
    [SerializeField] private Transform[] startPoints = null;

    public void Generate(float scale)
    {
        for (int i = 0; i < tribunes.Length; i++)
        {
            Vector3 tribuneScale = tribunes[i].localScale;
            tribuneScale.y = scale;
            tribunes[i].localScale = tribuneScale;
        }
        for (int i = 0; i < startPoints.Length; i++)
        {
            GenerateSpectators(startPoints[i], -Vector3.forward, scale);
        }
    }

    public void GenerateSpectators(Transform startPoint, Vector3 direction, float maxLength)
    {
        float offset = 0;
        while (Mathf.Abs(offset) < maxLength)
        {
            Transform spectator = Instantiate(spectatorPrefab, startPoint.position + direction * offset, spectatorPrefab.transform.rotation).transform;
            spectator.rotation = startPoint.rotation;
            offset += Random.Range(minDistanceBetweenSpectators, maxDistanceBetweenSpectators);
        }
    }
}
