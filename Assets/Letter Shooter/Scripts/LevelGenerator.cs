using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private int wordStackCount = 3;
    [SerializeField] private float distanceBetweenWordStacks = 10;
    [SerializeField] private List<Word> words;
    [SerializeField] private GameObject wordStackPrefab = null;
    [SerializeField] private Transform[] platforms = null;
    [SerializeField] private Tribune tribune = null;

    private MainLevel levelManager;
    private bool key = false;

    private void Awake()
    {
        levelManager = (MainLevel)LevelManager.Instance;
        key = Random.Range(0, 2) != 0;
        print(key);
        Generate();
    }

    public void Generate()
    {
        Vector3[] startPoints = new Vector3[platforms.Length];
        float scale = (wordStackCount + 2) * distanceBetweenWordStacks;
        if (tribune)
            tribune.Generate(scale);
        for (int i = 0; i < startPoints.Length; i++)
        {
            startPoints[i] = platforms[i].position + platforms[i].forward * distanceBetweenWordStacks * wordStackCount;
            Vector3 platformScale = platforms[i].localScale;
            platformScale.z = scale;
            platforms[i].localScale = platformScale;
            if (i != platforms.Length / 2)
            {
                Transform competitor = Instantiate(levelManager.CompetitorPrefab, platforms[i].position + (wordStackCount + 1) * distanceBetweenWordStacks * platforms[i].forward, Quaternion.Euler(-platforms[i].forward)).transform;
                levelManager.Competitors.Add(competitor.GetComponent<Competitor>());
            }
            else
            {
                Transform player = Instantiate(levelManager.PlayerPrefab, platforms[i].position + (wordStackCount + 1) * distanceBetweenWordStacks * platforms[i].forward, Quaternion.Euler(-platforms[i].forward)).transform;
                FindObjectOfType<CameraFollow>().Target = player;
                levelManager.Player = player.GetComponent<Player>();
                levelManager.Competitors.Add(player.GetComponent<Competitor>());
            }
        }
        ShuffleWords();
        float positionOffset = 0;
        int keyWordStackIndex = Random.Range(0, wordStackCount) * 3 + 1;
        for (int i = 0; i < Mathf.Clamp(wordStackCount * 3, 0, words.Count); i++)
        {
            Vector3 pos = startPoints[i % 3];
            pos.z += positionOffset;
            CreateWordStack(pos, words[i], levelManager.Competitors[i % 3], key && i == keyWordStackIndex);
            if (i % 3 == 2)
                positionOffset += distanceBetweenWordStacks;
        }

    }

    private void CreateWordStack(Vector3 pos, Word word, Competitor competitor, bool key)
    {
        WordStack wordStack = Instantiate(wordStackPrefab, pos, wordStackPrefab.transform.rotation).GetComponent<WordStack>();
        wordStack.Initialize(word, competitor, key);
    }


    public void ShuffleWords()
    {
        for (int i = 0; i < words.Count; i++)
        {
            Word temp = words[i];
            int randomIndex = Random.Range(i, words.Count);
            words[i] = words[randomIndex];
            words[randomIndex] = temp;
        }
    }
}
