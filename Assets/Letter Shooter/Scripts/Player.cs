using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Player : MonoBehaviour
{
    private Competitor competitor = null;
    private Camera cam = null;
    private Vector3 cursorPosition;
    private Vector3 latestMousePosition;
    private LineRenderer previewLine = null;
    private MainLevel levelManager;
    private Transform previewSphere = null;
    private float chargeValue = 0;
    private Animator animator = null;
    
    private bool finalShootMade = false;
    [SerializeField] private float aimSpeed = 1;
    [SerializeField] private float chargeTime = 3;
    [SerializeField] private float chargeDelta = 0.7f;
    [SerializeField] private GameObject previewSpherePrefab = null;
    [SerializeField] private LayerMask previewLayerMask;
    [SerializeField] private Material previewSphereMaterial = null;
    [SerializeField] private Material previewLineMaterial = null;
    [SerializeField] private GameObject confetties = null;

    public GameObject Confetties
    {
        get
        {
            return confetties;
        }
    }

    public Competitor Competitor
    {
        get
        {
            return competitor;
        }
    }

    private void Awake()
    {
        competitor = GetComponent<Competitor>();
        cam = Camera.main;
        cursorPosition = new Vector3(Screen.width / 1.7f, Screen.height / 2.8f, 0);
        CreatePreviewLine();

        animator = GetComponent<Animator>();
        levelManager = (MainLevel)LevelManager.Instance;
        previewSphere = Instantiate(previewSpherePrefab, Vector3.zero, Quaternion.identity).transform;
        previewSphere.GetComponent<Renderer>().material = previewSphereMaterial;
        //Preview();
    }

    private void Update()
    {
        

        if (GameManager.Instance.State == GameManager.GameState.STARTED && competitor.State == Competitor.CompetitorState.AIMING)
        {
            Aim();
            Preview();
        }
        else if (GameManager.Instance.State == GameManager.GameState.STARTED && competitor.State == Competitor.CompetitorState.CHARGING)
        {
            if (chargeTime > 0)
            {
                Charge();
                chargeTime -= Time.deltaTime;
            }
            else if (!finalShootMade)
                FinalShoot();
        }
    }
    private void Aim()
    {
        if (Input.GetMouseButtonDown(0))
        {
            latestMousePosition = Input.mousePosition;
            //latestMousePosition.y = Mathf.Clamp(latestMousePosition.y, Screen.height / 3.0f, Screen.height);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 currentMousePosition = Input.mousePosition;
            //currentMousePosition.y = Mathf.Clamp(currentMousePosition.y, Screen.height / 3.0f, Screen.height);
            Vector3 deltaPositon = currentMousePosition - latestMousePosition;
            cursorPosition += deltaPositon * (aimSpeed / 10);
            cursorPosition.y = Mathf.Clamp(cursorPosition.y, Screen.height / 2.8f, Screen.height);
            latestMousePosition = currentMousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            competitor.Shoot(previewSphere.position);
            cursorPosition = new Vector3(Screen.width / 1.7f, Screen.height / 2.8f, 0);
        }

    }

    private void Preview()
    {
        Ray ray = cam.ScreenPointToRay(cursorPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, previewLayerMask))
            previewSphere.position = hit.point;
        else
            previewSphere.position = ray.origin + ray.direction * 10;
        Vector3 force = competitor.Ball.CalculateForce(previewSphere.position, competitor.t);
        PlaceLine(force, 50);
    }
    private void PlaceLine(Vector3 force, int positionCount)
    {
        previewLine.positionCount = positionCount;
        float deltaTime = competitor.t / positionCount;
        Vector3[] positions = new Vector3[positionCount];
        positions[0] = competitor.BallStartPoint;
        for (int i = 1; i < positionCount; i++)
        {
            Vector3 newPos = positions[i - 1];
            newPos += (new Vector3(force.x, 0, force.z)) * deltaTime;
            force.y += Physics.gravity.y * deltaTime;
            newPos.y += (force.y * deltaTime);
            positions[i] = newPos;
        }
        previewLine.SetPositions(positions);
    }
    private void CreatePreviewLine()
    {
        previewLine = new GameObject("Preview Line").AddComponent<LineRenderer>();
        previewLine.material = previewLineMaterial;
        previewLine.startWidth = 0.1f;
        previewLine.endWidth = 0.1f;
    }

    public void HidePreview()
    {
        previewSphere.gameObject.SetActive(false);
        previewLine.gameObject.SetActive(false);
        previewSphere.position = new Vector3(0, 1000, 0);
        previewLine.positionCount = 0;
    }

    public void ShowPreview()
    {
        previewSphere.gameObject.SetActive(true);
        previewLine.gameObject.SetActive(true);
    }

    private void Charge()
    {
        if (chargeValue > 0)
            chargeValue = Mathf.Clamp(chargeValue - Time.deltaTime, 0, 1);
        if (Input.GetMouseButtonDown(0))
        {
            levelManager.TapText.PopUp();
            chargeValue = Mathf.Clamp(chargeValue + chargeDelta, 0, 1);
        }
        levelManager.ChargeSlider.value = chargeValue;
        if (chargeValue == 1f)
            chargeTime = 0;
        if (chargeValue > 0)
            animator.Play("Charge", 0, chargeValue);
        animator.SetFloat("CHARGE_VALUE", chargeValue);
    }

    private void FinalShoot()
    {
        finalShootMade = true;
        print("Final Shoot");
        levelManager.ChargeSlider.Hide();
        levelManager.TapText.Disappear();
        animator.SetTrigger("FINAL_SHOOT");
        LeanTween.delayedCall(0.2f, () =>
        {
            int index = Mathf.Clamp((int)(chargeValue * 15), 1, 15);
            levelManager.GoldMultiplier = index;
            competitor.Shoot(levelManager.X20.GetPosition(index));
            competitor.State = Competitor.CompetitorState.IDLE;
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        Coin coin = other.transform.GetComponent<Coin>();
        if (coin)
        {
            coin.Collect();
        }
    }

}
