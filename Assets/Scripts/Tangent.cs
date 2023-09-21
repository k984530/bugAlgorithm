using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tangent : MonoBehaviour
{
    public GameObject goal;
    private RaycastHit hit;
    private RaycastHit[] hits = new RaycastHit[360];
    private List<RaycastHit> Rs = new List<RaycastHit>();
    private Rigidbody playerRigidbody;
    private float minDistance;
    private int minIndex;
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
            Physics.Raycast(transform.position, new Vector3(Mathf.Cos(i * Mathf.Deg2Rad), 0f, Mathf.Sin(i * Mathf.Deg2Rad) ) , out hits[i], rayDis);
        }
        Physics.Raycast(transform.position, (goal.transform.position - transform.position).normalized, out hit, rayDis);

        for (int i = 1; i<359; i++)
        {
            if (hits[i].point != Vector3.zero)
            {
                if (hits[i - 1].point == Vector3.zero)
                {
                    Rs.Add(hits[i]);
                } else if(hits[i + 1].point == Vector3.zero){
                    Rs.Add(hits[i]);
                }
            }
        }


        if (hit.distance > 0.1f)
        {
            if (Vector3.Distance(transform.position, goal.transform.position) - hit.distance < minDistance)
            {
                minIndex = 360;
            }
        } else
        {
            if (Vector3.Distance(transform.position, goal.transform.position) - rayDis < minDistance)
            {
                minIndex = 360;
            }
        }
    }

    void normalGo()
    {
        shootRay();
        if(minIndex == 360)
        {
            playerRigidbody.velocity = (goal.transform.position - transform.position).normalized * 3f;
        }
        else
        {
            playerRigidbody.velocity = new Vector3(Mathf.Cos(minIndex * Mathf.Deg2Rad), 0f, Mathf.Sin(minIndex * Mathf.Deg2Rad)).normalized * 3f;
        }
    }

    void debugRay() {
        Debug.DrawRay(transform.position, (goal.transform.position - transform.position).normalized * rayDis, Color.red, 0f, true);
        Debug.DrawRay(transform.position, new Vector3(Mathf.Cos(minIndex * Mathf.Deg2Rad), 0f, Mathf.Sin(minIndex * Mathf.Deg2Rad)) * rayDis, Color.green, 0f, true);
        foreach (RaycastHit h in Rs) {
            Debug.DrawRay(transform.position,h.point - transform.position,Color.blue,0f);
                }

    }
    private void FixedUpdate()
    {
        normalGo();
        debugRay();
    }
}
