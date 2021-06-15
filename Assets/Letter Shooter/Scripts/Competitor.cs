using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Competitor : MonoBehaviour
{
    private Ball ball = null;
    private static MainLevel levelManager = null;
    private Player player = null;
    private CompetitorState state = CompetitorState.WAITING;
    private Queue<WordStack> wordStackQueue = null;
    private WordStack currentWordStack = null;
    private float remainingStunTime = 0;
    private Animator animator = null;
    private LTDescr moveTween = null;
    private bool finish = false;
    [SerializeField] private float ballSpeed = 1;
    [SerializeField] private GameObject ballPrefab = null;
    [SerializeField] private float speed = 1;
    [SerializeField] private Transform head = null;
    [SerializeField] private LayerMask cubeLayerMask;
    [SerializeField] private Transform ballStartPoint = null;
    [SerializeField] private GameObject stick = null;

    public CompetitorState State
    {
        get
        {
            return state;
        }
        set
        {
            //print(transform.name + " : " + state + " => " + value);
            animator.SetBool("IDLE", false);
            animator.SetBool("DANCING", false);
            animator.SetBool("MOVING", false);
            animator.SetBool("STUNNED", false);
            state = value;
            if (state == CompetitorState.AIMING)
                animator.SetBool("IDLE", true);
            else if (state == CompetitorState.FINISHED)
            {
                if (player && levelManager.PlayerPlace < 3 || !player)
                    animator.SetBool("DANCING", true);
                else if (player && levelManager.PlayerPlace > 2)
                    animator.SetBool("FAIL", true);
            }

            else if (state == CompetitorState.MOVING)
                animator.SetBool("MOVING", true);
            else if (state == CompetitorState.STUNNED)
                animator.SetBool("STUNNED", true);
            else if (state == CompetitorState.WAITING)
                animator.SetBool("IDLE", true);
            else if (state == CompetitorState.CHARGING)
            {
                animator.SetTrigger("CHARGE");
                levelManager.CameraFollow.GetToFinalShootPoint().setOnComplete(() =>
                {
                    levelManager.ChargeSlider.Show();
                    Initialize();
                    levelManager.TapText.PopUp();
                });
            }
            if (player)
            {
                if (state == CompetitorState.FINISHED)
                    levelManager.FinishLevel(levelManager.PlayerPlace < 3);
                if (state == CompetitorState.AIMING)
                    player.ShowPreview();
                else
                    player.HidePreview();
            }
            switch (state)
            {
                case CompetitorState.AIMING:
                    Initialize();
                    break;
                case CompetitorState.FINISHED:
                    if (stick)
                        stick.SetActive(false);
                    break;
            }
            Bot bot = GetComponent<Bot>();
            if (bot && state == CompetitorState.AIMING)
            {
                bot.Unlock();
            }
        }
    }

    public WordStack CurrentWordStack
    {
        get
        {
            return currentWordStack;
        }
    }

    public Queue<WordStack> WordStackQueue
    {
        get
        {
            return wordStackQueue;
        }
    }

    public Vector3 HeadPoint
    {
        get
        {
            return head.transform.position;
        }
    }
    public Vector3 BallStartPoint
    {
        get
        {
            return ballStartPoint.position;
        }
    }
    public float t
    {
        get
        {
            return 1 / ballSpeed;
        }
    }
    public Ball Ball
    {
        get
        {
            return ball;
        }
    }
    public float BallSpeed
    {
        get
        {
            return ballSpeed;
        }
    }
    public float Speed
    {
        get
        {
            return speed;
        }
    }

    private void Awake()
    {
        if (!levelManager)
            levelManager = (MainLevel)LevelManager.Instance;
        player = GetComponent<Player>();
        wordStackQueue = new Queue<WordStack>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!finish && transform.position.z >= levelManager.FinishLine.position.z)
        {
            finish = true;
            if (!player)
                levelManager.PlayerPlace++;
            else
            {
                levelManager.WinText.Show(levelManager.PlayerPlace, levelManager.PlayerPlace < 3);
            }
        }
        if (GameManager.Instance.State == GameManager.GameState.STARTED)
            UpdateState();
    }

    public void Stun()
    {
        if(state != CompetitorState.CHARGING || state != CompetitorState.FINISHED || state != CompetitorState.IDLE)
        {
            if (state == CompetitorState.MOVING)
                moveTween.pause();
            State = CompetitorState.STUNNED;
            if (ball)
                ball.Disappear(0.1f);
            remainingStunTime = player ? levelManager.PlayerStunTime : levelManager.CompetitorStunTime;
            GameObject effect = Instantiate(levelManager.StunnedEffectPrefab, head.position, levelManager.StunnedEffectPrefab.transform.rotation, head.transform);
            effect.transform.localPosition = levelManager.StunnedEffectPrefab.transform.localPosition;
            Destroy(effect, remainingStunTime);
        }
    }

    public void NextStack()
    {
        if (wordStackQueue.Count > 0)
        {
            currentWordStack = wordStackQueue.Dequeue();
            currentWordStack.Unlock();
        }
        else
            currentWordStack = null;
        MoveForward();
    }

    public void MoveForward()
    {
        State = CompetitorState.MOVING;
        Vector3 to = transform.position;
        if (currentWordStack)
            to.z = currentWordStack.transform.position.z - 7;
        else
            to.z = levelManager.EndPoint;
        float distance = Vector3.Distance(transform.position, to);
        float t = distance / speed;
        Move(to, t, currentWordStack == null);
    }

    private void Move()
    {
        if (moveTween != null)
            moveTween.resume();
    }

    private void Move(Vector3 to, float t, bool finished)
    {
        moveTween = transform.LeanMove(to, t).setOnComplete(() =>
         {
             if (finished)
             {
                 if (player)
                     State = levelManager.PlayerPlace < 3 ? CompetitorState.CHARGING : CompetitorState.FINISHED;
                 else
                     State = CompetitorState.FINISHED;
             }
             else
                 State = CompetitorState.AIMING;
         });
    }

    public void UpdateState()
    {
        if (State == CompetitorState.WAITING && !ball.Active)
        {
            if (currentWordStack.Active && currentWordStack.State == WordStack.WordStackState.IDLE)
                State = CompetitorState.AIMING;
            else if (!currentWordStack.Active)
                NextStack();
        }
        else if (State == CompetitorState.STUNNED)
        {
            if (remainingStunTime <= 0)
            {
                if (moveTween != null && moveTween.ratioPassed >= 1)
                    State = CompetitorState.WAITING;
                else
                    Move();
            }
            remainingStunTime -= Time.deltaTime;
        }
    }

    public void Shoot(Vector3 targetPosition)
    {
        if (State == CompetitorState.AIMING)
        {
            if (currentWordStack.Active)
            {
                State = CompetitorState.WAITING;
                animator.SetTrigger("SHOOT");
                Ball ball = this.ball;
                Vector3 force = ball.CalculateForce(targetPosition, t);
                LeanTween.delayedCall(0.60f, () =>
                {
                    ball.Throw(force, t, false);
                    LeanTween.delayedCall(t * 5.1f, () => { if (ball.gameObject.activeSelf) ball.Disappear(0.1f); });
                });
            }
        }
        else if (State == CompetitorState.CHARGING)
        {
            Ball ball = this.ball;
            Vector3 force = ball.CalculateForce(targetPosition, 3);
            levelManager.CameraFollow.Offset = ball.FollowPoint.position - ball.transform.position;
            levelManager.CameraFollow.Target = ball.transform;
            levelManager.CameraFollow.StartFollowing();
            levelManager.CameraFollow.transform.LeanRotate(ball.FollowPoint.rotation.eulerAngles, 0.2f);
            ball.Throw(force, 3, true);
            LeanTween.delayedCall(3.5f, () => { levelManager.Player.Competitor.State = Competitor.CompetitorState.FINISHED; });
        }
    }



    private void CreateBall()
    {
        ball = Instantiate(ballPrefab, BallStartPoint, ballPrefab.transform.rotation).GetComponent<Ball>();
        ball.Owner = this;
    }

    private void Initialize()
    {
        CreateBall();
    }

    public enum CompetitorState { AIMING, WAITING, STUNNED, MOVING, FINISHED, CHARGING, IDLE }
}
