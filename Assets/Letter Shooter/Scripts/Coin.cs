using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Coin : MonoBehaviour
{
    [SerializeField] private Transform mesh = null;
    [SerializeField] private GameObject collectEffectPrefab = null;
    private Rigidbody rb = null;
    private bool collected = false;
    private static MainLevel levelManager = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        transform.localScale = Vector3.zero;
        if (!levelManager)
            levelManager = (MainLevel)LevelManager.Instance;
        mesh.Rotate(Vector3.forward * Random.Range(0f, 180f));
    }
    void Start()
    {
        rb.AddForce(new Vector3(0, 2, Random.Range(-2f, 2f)), ForceMode.Impulse);
        transform.LeanScale(Vector3.one, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        mesh.Rotate(Vector3.forward * Time.deltaTime * 45);
    }

    public void Collect()
    {
        if (!collected)
        {
            collected = true;
            Instantiate(collectEffectPrefab, mesh.position, collectEffectPrefab.transform.rotation);
            transform.LeanScale(Vector3.zero, 0.3f);
            levelManager.AddGold(1);
        }
    }
}
