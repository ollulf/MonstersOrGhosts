using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Photon.Pun;

public class Flocking : MonoBehaviourPun
{
    private Transform target;
    [SerializeField] private float radius, nighbourDistance, maxDistance, rotationSpeed;
    [SerializeField] private Transform motherFlock;
    private List<Transform> flock;

    private Vector3 direction;

    private float speedX, speedY, speedZ;

    // Start is called before the first frame update
    void Start()
    {
        flock = new List<Transform>();
        speedX = Random.Range(0.5f, 10f);
        speedY = Random.Range(0.5f, 10f);
        speedZ = Random.Range(0.5f, 10f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        direction = target.position - transform.position;


        if (CheckTurning())
        {
            SetTurningAndSpeed();
        }
        else
        {
            if (Random.Range(0, 5) < 1)
            {
                ApplyRules();
            }
        }

        if(PlayerBaseDataHandler.FishPopulation.CheckMovement() || Vector3.Distance(target.position,transform.position) > maxDistance + 0.5f)
        {
            speedX = 11;
            speedY = 11;
            speedZ = 11;
        }

        Movement();
    }

    private void SetTurningAndSpeed()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed);

        speedX = Random.Range(1f, 5);
        speedZ = Random.Range(1f, 5);

        int Rand = Random.Range(0, 2);
        if (Rand == 1)
        {
            speedY = Random.Range(1f, 5);
        }
        else
        {
            speedY = Random.Range(-1f, -5);
        }
    }

    private bool CheckTurning()
    {
        return Vector3.Distance(transform.position, target.position) >= maxDistance;
    }

    private void Movement()
    {
        transform.Translate(speedX * Time.deltaTime, speedY * Time.deltaTime, speedZ * Time.deltaTime);
    }

    private void ApplyRules()
    {
        Vector3 vCentre = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;

        float gSpeedX = 0.5f;
        float gSpeedY = 0.5f;
        float gSpeedZ = 0.5f;

        float dist;

        int groupSize = 0;

        AddToFlock();

        foreach(Transform trans in flock)
        {
            if(trans != this.transform)
            {
                dist = Vector3.Distance(trans.position, this.transform.position);
                if(dist <= nighbourDistance)
                {
                    groupSize++;
                    if(dist < 1f)
                    {
                        vAvoid = vAvoid + (this.transform.position - trans.position);
                    }
                    Flocking anotherFlock = trans.GetComponent<Flocking>();
                    gSpeedX = gSpeedX + anotherFlock.speedX;
                    gSpeedY = gSpeedY + anotherFlock.speedY;
                    gSpeedZ = gSpeedZ + anotherFlock.speedZ;

                }
            }
            if(Vector3.Distance(target.position, this.transform.position) <= radius)
            {
                vAvoid = (this.transform.position - target.position).normalized;
            }
            vCentre += (trans.position + vAvoid).normalized;
        }

        if(groupSize > 0)
        {
            vCentre = vCentre / groupSize + (target.position - this.transform.position);
            speedX = gSpeedX / groupSize;
            speedY = gSpeedY / groupSize;
            speedZ = gSpeedZ / groupSize;
            Vector3 Direction = (vCentre + vAvoid).normalized - transform.position;
            if (Direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      Quaternion.LookRotation(Direction),
                                                      rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void AddToFlock()
    {
        foreach(Transform child in motherFlock)
        {
            if(!flock.Contains(child))
            {
                flock.Add(child);
                GlobalFlock.AddFishFlock(child);
            }
        }
    }

    public void SetMotherFlock(Transform newMother)
    {
        motherFlock = newMother;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void DestroyFlock()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    public void SetFlock(List<Transform> newList)
    {
        flock = new List<Transform>(newList);
    }
}
