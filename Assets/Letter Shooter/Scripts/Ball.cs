using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
public class Ball : MonoBehaviour
{
    private static MainLevel levelManager = null;
    private Rigidbody rb = null;
    private bool active = true;
    private bool rotate = true;
    public Competitor Owner = null;
    [SerializeField] private GameObject impactEffect = null;
    [SerializeField] private Transform followPoint = null;
    [SerializeField] private Transform mesh = null;


    public Transform FollowPoint
    {
        get
        {
            return followPoint;
        }
    }

    public Rigidbody RB
    {
        get
        {
            return rb;
        }
    }

    public bool Active
    {
        get
        {
            return active;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        if (!levelManager)
            levelManager = (MainLevel)LevelManager.Instance;
    }


    private void Update()
    {
        if (rotate)
            mesh.Rotate(new Vector3(15, 20, 30) * Time.deltaTime);
    }
    public void Disappear(float t)
    {
        GetComponent<TrailRenderer>().enabled = false;
        Collider collider = GetComponent<Collider>();
        if (collider)
            collider.enabled = false;
        if (Owner.State == Competitor.CompetitorState.STUNNED)
        {
            active = false;
            gameObject.SetActive(false);
        }
        else
            transform.LeanScale(Vector3.zero, t).setOnComplete(() =>
            {
                active = false;
                gameObject.SetActive(false);
            });

    }

    public void Throw(Vector3 force, float t, bool final)
    {
        rb.isKinematic = true;
        rb.isKinematic = false;
        rb.AddForce(force, ForceMode.Impulse);
        if (final)
            LeanTween.delayedCall(t, () => { rb.isKinematic = true; });
    }

    public Vector3 CalculateForce(Vector3 to, float t)
    {
        Vector3 dif = to - transform.position;
        Vector3 force = new Vector3(dif.x, 0, dif.z) / t;
        force.y = dif.y / t - Physics.gravity.y * t / 2;
        return force;
    }

    private void ThrowToCompetitor(Competitor competitor)
    {
        Throw(CalculateForce(competitor.HeadPoint, competitor.t), competitor.t, false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;
        if (Owner.State == Competitor.CompetitorState.STUNNED)
        {
            active = false;
            Disappear(0.1f);
        }

        if (active)
        {
            Cube cube = other.GetComponent<Cube>();
            Competitor competitor = other.GetComponentInParent<Competitor>();
            if (cube)
            {
                if (cube.stack.State == WordStack.WordStackState.IDLE && cube.stack == cube.stack.Competitor.CurrentWordStack)
                {
                    if (cube.Type == Cube.CubeType.WRONG || cube.Type == Cube.CubeType.KEY)
                    {
                        cube.stack.Pop(cube.Index);
                        Disappear(0.1f);
                    }
                    else
                    {
                        Destroy(Instantiate(impactEffect, transform.position, impactEffect.transform.rotation), 0.5f);
                        ThrowToCompetitor(cube.stack.Competitor);
                        collider.enabled = true;
                    }
                }
                else
                {
                    Disappear(0.1f);
                }
            }
            else if (competitor)
            {
                Destroy(Instantiate(impactEffect, transform.position, impactEffect.transform.rotation), 0.5f);
                competitor.Stun();
                Disappear(0.1f);
            }
            else
            {
                Disappear(0.1f);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (active)
        {
            if (levelManager.Player.Competitor.State != Competitor.CompetitorState.IDLE)
                Disappear(0.1f);
            else
                rotate = false;
        }

    }


}
