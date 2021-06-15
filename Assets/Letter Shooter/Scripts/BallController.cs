using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class BallController : MonoBehaviour
{
    /*private Camera cam = null;
    private Vector3 cursorPosition;
    private Vector3 latestMousePosition;
    private LineRenderer previewLine = null;
    private MainLevel levelManager;
    private bool locked = false;
    private Ball ball = null;
    private Transform previewSphere = null;

    [SerializeField] private float aimSpeed = 1;
    [SerializeField] private float ballSpeed = 1;
    [SerializeField] private GameObject previewSpherePrefab = null;
    [SerializeField] private GameObject ballPrefab = null;
    [SerializeField] private LayerMask previewLayerMask;
    [SerializeField] private Material previewSphereMaterial = null;
    [SerializeField] private Material previewLineMaterial = null;

    private float t
    {
        get
        {
            return 1 / ballSpeed;
        }
    }

    private void Awake()
    {
        cam = Camera.main;
        cursorPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        CreatePreviewLine();


        levelManager = (MainLevel)LevelManager.Instance;
        previewSphere = Instantiate(previewSpherePrefab, Vector3.zero, Quaternion.identity).transform;
        previewSphere.GetComponent<Renderer>().material = previewSphereMaterial;
        Initialize();
        Preview();
    }

    private void Update()
    {
        if (GameManager.Instance.State == GameManager.GameState.STARTED && !locked)
        {
            Aim();
            Preview();
        }
    }
    private void Aim()
    {
        if (Input.GetMouseButtonDown(0))
        {
            latestMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 deltaPositon = currentMousePosition - latestMousePosition;
            cursorPosition += deltaPositon * (aimSpeed / 10);
            latestMousePosition = currentMousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Shoot();
            cursorPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        }

    }

    private void Preview()
    {
        Ray ray = cam.ScreenPointToRay(cursorPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, previewLayerMask))
            previewSphere.position = hit.point;
        else
            previewSphere.position = ray.origin + ray.direction * 10;

        Vector3 force = CalculateForce();
        PlaceLine(force, 50);
    }
    private Vector3 CalculateForce()
    {
        Vector3 dif = previewSphere.position - levelManager.Player.BallStartPoint;
        Vector3 force = new Vector3(dif.x, 0, dif.z) / t;
        force.y = dif.y / t - Physics.gravity.y * t / 2;
        return force;
    }

    private void Shoot()
    {
        Lock();
        Ball ball = this.ball;
        ball.RB.isKinematic = false;
        ball.RB.AddForce(CalculateForce(), ForceMode.Impulse);
        LeanTween.delayedCall(t + 2, () => { ball.Disappear(0.4f); });
        LeanTween.delayedCall(1, Initialize);
    }

    private void PlaceLine(Vector3 force, int positionCount)
    {
        previewLine.positionCount = positionCount;
        float deltaTime = t / positionCount;
        Vector3[] positions = new Vector3[positionCount];
        positions[0] = levelManager.Player.BallStartPoint;
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

    private void Lock()
    {
        locked = true;
        previewSphere.gameObject.SetActive(!locked);
        previewLine.gameObject.SetActive(!locked);
    }

    private void Unlock()
    {
        locked = false;
        previewSphere.gameObject.SetActive(!locked);
        previewLine.gameObject.SetActive(!locked);
    }

    private void CreateBall()
    {
        ball = Instantiate(ballPrefab, levelManager.Player.BallStartPoint, ballPrefab.transform.rotation).GetComponent<Ball>();
    }

    private void Initialize()
    {
        CreateBall();
        Unlock();
    }*/
}
