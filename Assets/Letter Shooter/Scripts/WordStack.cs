using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class WordStack : MonoBehaviour
{
    private Stack<char> stack;
    private Cube[] cubes;
    private int numberOfWrongLetters = 0;
    private Competitor competitor = null;
    private bool active = true;
    private int wordLength = 0;
    [SerializeField] private GameObject cubePrefab = null;
    [SerializeField] private Material unlockedMaterial = null;
    [SerializeField] private GameObject disappearingEffectPrefab = null;
    [SerializeField] private Material keyCubeMaterial = null;
    [SerializeField] private GameObject coinPrefab = null;
    private WordStackState state = WordStackState.IDLE;
    private static MainLevel levelManager = null;

    public int WordLength
    {
        get
        {
            return wordLength;
        }
    }
    public WordStackState State
    {
        get
        {
            return state;
        }
    }

    public Competitor Competitor
    {
        get
        {
            return competitor;
        }
    }

    public Cube[] Cubes
    {
        get
        {
            return (Cube[])cubes.Clone();
        }
    }

    private void Awake()
    {
        stack = new Stack<char>();
        if (!levelManager)
            levelManager = (MainLevel)LevelManager.Instance;
    }
    public void Initialize(Word word, Competitor competitor, bool key)
    {
        this.competitor = competitor;
        competitor.WordStackQueue.Enqueue(this);
        wordLength = word.word.Length;
        numberOfWrongLetters = Random.Range(1, wordLength < 5 ? 2 : 3) + (key ? 1 : 0);
        List<int> wrongLetterIndexes = new List<int>();
        for (int i = 0; i < numberOfWrongLetters; i++)
        {
            int index = Random.Range(1, wordLength + numberOfWrongLetters - 1);
            while (wrongLetterIndexes.Contains(index))
                index = Random.Range(1, wordLength + numberOfWrongLetters - 1);
            wrongLetterIndexes.Add(index);
        }


        int offset = 0;
        for (int i = 0; i < wordLength + numberOfWrongLetters; i++)
        {
            char letter;
            if (wrongLetterIndexes.Contains(i))
            {
                letter = RandomLetter(word.word);
                offset--;
            }
            else
                letter = word.word[i + offset];
            stack.Push(letter);
        }
        CreateCubes();
        int keyCubeIndex = wrongLetterIndexes[Random.Range(0, wrongLetterIndexes.Count)];
        foreach (int index in wrongLetterIndexes)
        {
            if (key && index == keyCubeIndex)
                cubes[wordLength + numberOfWrongLetters - index - 1].Type = Cube.CubeType.KEY;
            else
                cubes[wordLength + numberOfWrongLetters - index - 1].Type = Cube.CubeType.WRONG;
        }

    }

    private void CreateCubes()
    {
        cubes = new Cube[stack.Count];
        float y = 0f;
        int i = 0;
        while (stack.Count > 0)
        {
            Cube cube = Instantiate(cubePrefab, transform.position + Vector3.up * y, cubePrefab.transform.rotation, transform).GetComponent<Cube>();
            char letter = stack.Pop();
            cube.SetLetter(letter);
            cube.stack = this;
            cubes[i] = cube;
            cube.Index = i;
            y++;
            i++;
        }
    }

    // RETURNS true if the stack is wrong letter free
    public bool Pop(int index)
    {
        bool result = false;
        if (numberOfWrongLetters > 0)
        {
            state = WordStackState.POPPING;
            ParticleSystemRenderer impactEffect = Instantiate(levelManager.CubeImpactEffectPrefab, cubes[index].transform.position, levelManager.CubeImpactEffectPrefab.transform.rotation).GetComponent<ParticleSystemRenderer>();
            if (cubes[index].Type == Cube.CubeType.KEY)
            {
                impactEffect.material = keyCubeMaterial;
                levelManager.AddKey(1);
            }
            Destroy(impactEffect.gameObject, 1f);
            numberOfWrongLetters--;
            cubes[index].gameObject.SetActive(false);
            for (int i = index + 1; i < cubes.Length; i++)
            {
                Cube cube = cubes[i];
                cube.transform.LeanMoveY(cube.transform.position.y - 1, 0.44f).setEaseInQuad().setOnComplete(() =>
                {

                    if (numberOfWrongLetters == 0)
                    {
                        result = true;
                        WordStackEffect wordStackEffect = Instantiate(disappearingEffectPrefab, transform.position, disappearingEffectPrefab.transform.rotation).GetComponent<WordStackEffect>();
                        wordStackEffect.Appear(wordLength);
                        LeanTween.delayedCall(1f, Disappear);
                    }
                    else
                        state = WordStackState.IDLE;
                });
            }


        }
        return result;
    }

    public bool Active
    {
        get
        {
            return active;
        }
    }

    public List<Cube> WrongLetterCubes()
    {
        List<Cube> result = new List<Cube>();
        for (int i = 0; i < cubes.Length; i++)
        {
            Cube cube = cubes[i];
            if (cube.gameObject.activeSelf && cube.Type == Cube.CubeType.WRONG)
                result.Add(cube);
        }
        return result;
    }

    private void Disappear()
    {
        if (state != WordStackState.DISAPPEARING)
        {
            state = WordStackState.DISAPPEARING;
            if (competitor.GetComponent<Player>())
                for (int i = 0; i < wordLength; i++)
                {
                    Instantiate(coinPrefab, transform.position, coinPrefab.transform.rotation);
                }
            transform.LeanScale(Vector3.zero, 0.1f).setOnComplete(() => { active = false; });
        }
    }

    private static char RandomLetter(string excludes)
    {
        char result = RandomLetter();
        while (excludes.Contains(result.ToString()))
            result = RandomLetter();
        return result;
    }
    private static char RandomLetter()
    {
        return ((char)Random.Range(65, 91));
    }

    public void Unlock()
    {
        foreach (Cube cube in cubes)
        {
            if (cube.Type != Cube.CubeType.KEY)
                cube.gameObject.LeanColor(unlockedMaterial.color, 0.3f);
        }
    }

    public enum WordStackState { DISAPPEARING, IDLE, POPPING }
}
