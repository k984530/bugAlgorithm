using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tangent : MonoBehaviour
{
    public GameObject goal;
    private RaycastHit hit;
    private RaycastHit[] hits = new RaycastHit[360];
    private List<Vector3> Rs = new List<Vector3>();
    private Rigidbody playerRigidbody;
    private float minDistance;
    private int minIndex;
    private Vector3 minPoint;
    public float rayDis;

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
                    Vector3 p = new Vector3(-Vector3.Cross(hits[i].point, hits[i + 1].point).x, 0f, -Vector3.Cross(hits[i].point, hits[i + 1].point).z).normalized * 0.6f;
                    Debug.DrawRay(hits[i].point, p, Color.cyan, 0f, true);
                    //Rs.Add(hits[i].point);
                    Rs.Add(hits[i].point - transform.position + p);
                    //Rs.Add(((new Vector3(Vector3.Cross(hits[i].point, hits[i + 1].point).x, 0f, Vector3.Cross(hits[i].point, hits[i + 1].point).z).normalized * 0.5f) - hits[i].point).normalized);
                    Debug.DrawRay(transform.position, hits[i].point - transform.position + p, Color.gray, 0f);
                    Debug.DrawRay(transform.position, hits[i].point - transform.position, Color.yellow, 0f);

                }
                else if (hits[i + 1].point == Vector3.zero)
                {
                    Vector3 p = new Vector3(Vector3.Cross(hits[i].point, hits[i - 1].point).x, 0f, Vector3.Cross(hits[i].point, hits[i - 1].point).z).normalized * 0.6f;

                    Debug.DrawRay(hits[i].point, p, Color.black, 0f, true);
                    //Rs.Add(hits[i].point);
                    Rs.Add(hits[i].point - transform.position * 2 + p);
                    //Rs.Add(((new Vector3(Vector3.Cross(hits[i].point, hits[i + 1].point).x, 0f, Vector3.Cross(hits[i].point, hits[i + 1].point).z).normalized * 0.5f) - hits[i].point).normalized);
                    Debug.DrawRay(transform.position, hits[i].point - transform.position + p, Color.gray, 0f);
                    Debug.DrawRay(transform.position, hits[i].point - transform.position, Color.magenta, 0f);
                }
            }
        }

        foreach (Vector3 h in Rs)
        {
            if (Vector3.Distance(h, goal.transform.position) < minDistance)
            {
                minDistance = Vector3.Distance(h, goal.transform.position);
                minPoint = h;
            }
        }

        if (Vector3.Distance(transform.position, goal.transform.position) - 0.5f < minDistance)
        {
            Debug.Log("test");
            minDistance = Vector3.Distance(transform.position, goal.transform.position) - 0.5f;
            minPoint = goal.transform.position - transform.position;
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
        normalGo();
    }
}
