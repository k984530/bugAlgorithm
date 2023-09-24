using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class Tangent : MonoBehaviour
{
    public GameObject goal;
    private RaycastHit hit;
    private RaycastHit[] hitCheck = new RaycastHit[3];
    private RaycastHit[] hits = new RaycastHit[360];
    private List<Vector3> Rs = new List<Vector3>();
    private Rigidbody playerRigidbody;
    private float minDistance;
    private int count = 0;

    private Vector3 minPoint;
    public float rayDis;


    public Transform startpoint;

    private ContactPoint con;
    private bool Tanflag = true;
    private bool flag = false;
    private bool enterFlag = true;

    Vector3 contactNormal;
    Vector3 perpendicularToXZPlane;
    Vector3 force;

    private Vector3 initPos;
    private float flagDistance;
    private double min = 10000;


    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();

    }

    void shootRay()
    {
        Rs.Clear();

        hits = new RaycastHit[360];
        minDistance = 1000f;

        for (int i = 0; i < 360; i++)
        {
            Physics.Raycast(transform.position, new Vector3(Mathf.Cos(i * Mathf.Deg2Rad), 0f, Mathf.Sin(i * Mathf.Deg2Rad)), out hits[i], rayDis);
        }
        Physics.Raycast(transform.position, (goal.transform.position - transform.position).normalized, out hit, rayDis);

        for (int i = 1; i < 359; i++)
        {
            if (hits[i].point != Vector3.zero)
            {


                    if (hits[i - 1].point == Vector3.zero)
                    {
                        Vector3 p = new Vector3(-Vector3.Cross(hits[i].point, hits[i + 1].point).x, 0f, -Vector3.Cross(hits[i].point, hits[i + 1].point).z).normalized * 0.8f;
                        Debug.DrawRay(hits[i].point, p, Color.cyan, 0f, true);
                        Rs.Add(hits[i].point - transform.position + p);
                        Debug.DrawRay(transform.position, hits[i].point - transform.position + p, Color.gray, 0f);
                        Debug.DrawRay(transform.position, hits[i].point - transform.position, Color.yellow, 0f);

                    }
                    else if (hits[i + 1].point == Vector3.zero)
                    {
                        Vector3 p = new Vector3(Vector3.Cross(hits[i].point, hits[i - 1].point).x, 0f, Vector3.Cross(hits[i].point, hits[i - 1].point).z).normalized * 0.8f;

                        Debug.DrawRay(hits[i].point, p, Color.black, 0f, true);
                        Rs.Add(hits[i].point - transform.position * 2 + p);
                        Debug.DrawRay(transform.position, hits[i].point - transform.position + p, Color.gray, 0f);
                        Debug.DrawRay(transform.position, hits[i].point - transform.position, Color.magenta, 0f);
                    
                }
            }
        }

        foreach (Vector3 h in Rs)
        {
            if (Vector3.Distance(h, goal.transform.position) - 1f < minDistance)
            {
                minDistance = Vector3.Distance(h, goal.transform.position) -1f;
                minPoint = h;
            }
        }

        if (Vector3.Distance(transform.position, goal.transform.position) + 1f < minDistance)
        {
            if (!Physics.Raycast(transform.position, (goal.transform.position - transform.position).normalized + new Vector3(Mathf.Cos(Mathf.Deg2Rad), 0f, Mathf.Sin(Mathf.Deg2Rad)), out hit, rayDis)
                && !Physics.Raycast(transform.position, (goal.transform.position - transform.position).normalized - new Vector3(Mathf.Cos(Mathf.Deg2Rad), 0f, Mathf.Sin(Mathf.Deg2Rad)), out hit, rayDis)
                || Vector3.Distance(transform.position, goal.transform.position) < 2f)
            {
                Debug.Log("Check");
                minDistance = Vector3.Distance(transform.position, goal.transform.position) + 1f;
                minPoint = goal.transform.position - transform.position;
            }
            else
            {
                Tanflag = false;
                initPos = transform.position;
                flagDistance = Vector3.Distance(goal.transform.position, initPos);
            }
        }
    }

    void normalGo()
    {
        shootRay();
        playerRigidbody.velocity = new Vector3(minPoint.x, 0f, minPoint.z).normalized * 3f;
        Debug.DrawRay(transform.position, playerRigidbody.velocity, Color.red, 0f, true);

    }


    private void FixedUpdate()
    {
        if (Tanflag)
        {
            normalGo();
        }
        else
        {
            if (flag) // 장애물의 꼭짓점을 만난 경우 
            {
                perpendicularToXZPlane = new Vector3(-contactNormal.z - contactNormal.x, 0.0f, contactNormal.x - contactNormal.z);
                playerRigidbody.rotation = Quaternion.LookRotation(con.point);
                force = perpendicularToXZPlane.normalized * 1.5f;
                playerRigidbody.velocity = force;
            }
            else
            {
                if (!enterFlag)
                {
                    playerRigidbody.velocity = (goal.transform.position - transform.position).normalized * 2;
                }
            }
            if(Vector3.Distance(transform.position, goal.transform.position) < flagDistance)
            {
                Tanflag = true;
            }
        }
    }

    //장애물 인식
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Debug.Log(enterFlag);
            count++;
            Debug.Log(count);
            enterFlag = true;
            flag = false;
            foreach (ContactPoint contact in collision.contacts)
            {
                con = contact;
                contactNormal = contact.normal; // 법선 벡터

                // 이동할 거리를 결정합니다.

                perpendicularToXZPlane = new Vector3(-contactNormal.z, 0.0f, contactNormal.x);
                playerRigidbody.rotation = Quaternion.LookRotation(-contactNormal);
                // 오브젝트를 새로운 위치로 이동시킵니다.
                force = perpendicularToXZPlane.normalized * 3f;

                playerRigidbody.velocity = force;
            }

        }
    }


    // 벽을 짚고 감
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            flag = false;
            foreach (ContactPoint contact in collision.contacts)
            {
                con = contact;
                contactNormal = contact.normal; // 법선 벡터

                // 이동할 거리를 결정합니다.

                perpendicularToXZPlane = new Vector3(-contactNormal.z, 0.0f, contactNormal.x);
                playerRigidbody.rotation = Quaternion.LookRotation(-contactNormal);
                // 오브젝트를 새로운 위치로 이동시킵니다.
                force = perpendicularToXZPlane.normalized * 3.0f;

                playerRigidbody.velocity = force;


            }

        }
    }

    // 꼭짓점을 만난경우.
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            count--;
            Debug.Log(count);
            if (count == 0)
            {
                Debug.Log(enterFlag);
                enterFlag = false;
                playerRigidbody.velocity = Vector3.zero;
                flag = true;
            }
        }
    }
}
