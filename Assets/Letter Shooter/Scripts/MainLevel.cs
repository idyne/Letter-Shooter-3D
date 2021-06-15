using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FateGames;

public class MainLevel : LevelManager
{

    public Player Player;
    private Camera cam;
    private WinText winText = null;
    private TapText tapText = null;
    private ChargeSlider chargeSlider = null;
    private CameraFollow cameraFollow = null;
    [SerializeField] private float playerStunTime = 2f;
    [SerializeField] private float competitorStunTime = 4f;
    public List<Competitor> Competitors = new List<Competitor>();
    [SerializeField] private Transform end;
    [SerializeField] private X20 x20 = null;
    [SerializeField] private GameObject cubeImpactEffectPrefab = null;
    [SerializeField] private GameObject stunnedEffectPrefab = null;
    [SerializeField] private Transform finishCameraPosition = null;
    [SerializeField] private GameObject playerPrefab = null;
    [SerializeField] private GameObject competitorPrefab = null;
    [SerializeField] private Transform finishLine = null;
    [SerializeField] private Text goldText = null;
    [SerializeField] private Text keyText = null;
    private int goldGained = 0;
    public int GoldMultiplier = 1;
    public int PlayerPlace = 1;

    public WinText WinText
    {
        get
        {
            return winText;
        }
    }
    public Transform FinishLine
    {
        get
        {
            return finishLine;
        }
    }
    public X20 X20
    {
        get
        {
            return x20;
        }
    }
    public TapText TapText
    {
        get
        {
            return tapText;
        }
    }
    public CameraFollow CameraFollow
    {
        get
        {
            return cameraFollow;
        }
    }
    public ChargeSlider ChargeSlider
    {
        get
        {
            return chargeSlider;
        }
    }
    public GameObject CompetitorPrefab
    {
        get
        {
            return competitorPrefab;
        }
    }
    public GameObject PlayerPrefab
    {
        get
        {
            return playerPrefab;
        }
    }
    public GameObject CubeImpactEffectPrefab
    {
        get
        {
            return cubeImpactEffectPrefab;
        }
    }

    public GameObject StunnedEffectPrefab
    {
        get
        {
            return stunnedEffectPrefab;
        }
    }


    public float CompetitorStunTime
    {
        get
        {
            return competitorStunTime;
        }
    }

    public float PlayerStunTime
    {
        get
        {
            return playerStunTime;
        }
    }

    public float EndPoint
    {
        get
        {
            return end.transform.position.z;
        }
    }
    private new void Awake()
    {
        base.Awake();
        cam = Camera.main;
        winText = FindObjectOfType<WinText>();
        tapText = FindObjectOfType<TapText>();
        chargeSlider = FindObjectOfType<ChargeSlider>();
        x20 = FindObjectOfType<X20>();
        cameraFollow = FindObjectOfType<CameraFollow>();
        goldText.text = GameManager.Instance.Gold.ToString();
        keyText.text = GameManager.Instance.Key.ToString();
    }
    public override void FinishLevel(bool success)
    {
        FindObjectOfType<CameraFollow>().StopFollowing();
        if (success)
        {
            AddGold(goldGained * (GoldMultiplier - 1));
        }

        cam.transform.LeanMove(finishCameraPosition.position, 1f);
        cam.transform.LeanRotate(finishCameraPosition.rotation.eulerAngles, 1f).setOnComplete(() =>
        {
            if (success)
            {
                Player.Confetties.SetActive(true);
            }

            LeanTween.delayedCall(0.55f, () => { GameManager.Instance.FinishLevel(success); });
        });
    }

    public override void StartLevel()
    {
        foreach (Competitor competitor in Competitors)
            competitor.NextStack();
    }

    public int AddGold(int gain)
    {
        goldGained += 1;
        GameManager.Instance.Gold += gain;
        goldText.text = GameManager.Instance.Gold.ToString();
        return GameManager.Instance.Gold;
    }

    public int AddKey(int gain)
    {
        GameManager.Instance.Key += gain;
        keyText.text = GameManager.Instance.Key.ToString();
        return GameManager.Instance.Key;
    }

}
