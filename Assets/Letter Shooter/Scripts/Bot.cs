using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Bot : MonoBehaviour
{
    private Competitor competitor = null;
    private bool locked = false;
    [SerializeField] private int difficulty = 1;
    private static MainLevel levelManager = null;

    private void Awake()
    {
        competitor = GetComponent<Competitor>();
        if (!levelManager)
            levelManager = (MainLevel)LevelManager.Instance;
    }

    private void Update()
    {
        if (!locked && GameManager.Instance.State == GameManager.GameState.STARTED)
        {
            CheckState();
        }
    }

    private void CheckState()
    {
        if (competitor.State == Competitor.CompetitorState.AIMING && competitor.CurrentWordStack.State == WordStack.WordStackState.IDLE)
        {
            Lock();
            WordStack wordStack = competitor.CurrentWordStack;
            List<Cube> wrongLetterCubes = wordStack.WrongLetterCubes();
            Cube targetCube = wrongLetterCubes[Random.Range(0, wrongLetterCubes.Count)];
            float offsetX = Random.Range(-1f + 1 / difficulty, 1f + 1 / difficulty);
            float offsetY = Random.Range(-1f + 1 / difficulty, 1f + 1 / difficulty);
            Competitor targetCompetitor = levelManager.Player.Competitor;
            float x = Random.value;
            Vector3 target;
            if (levelManager.Player.Competitor.State != Competitor.CompetitorState.FINISHED && levelManager.Player.Competitor.State != Competitor.CompetitorState.CHARGING && levelManager.Player.Competitor.State != Competitor.CompetitorState.IDLE && x < 0.35f && targetCompetitor.transform.position.z > transform.position.z + 5)
            {
                print("shoot player");
                target = targetCompetitor.transform.position + Vector3.right * offsetX + Vector3.up * offsetY;
            }

            else target = targetCube.transform.position + Vector3.right * offsetX + Vector3.up * offsetY;
            competitor.Shoot(target);
        }
    }

    public void Lock()
    {
        locked = true;
        //print(transform.name + " Locked");
    }

    public void Unlock()
    {
        locked = false;
        //print(transform.name + " Unlocked");
    }
}
