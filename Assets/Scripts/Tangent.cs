using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tangent : MonoBehaviour
{
    public GameObject goal;
    private RaycastHit hit;
    private RaycastHit[] hits;
    private Rigidbody playerRigidbody;

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    void shootRay()
    {
        for (int i = 0; i < 360; i++)
        {
            if (Physics.Raycast(transform.position, new Vector3(Mathf.Cos(i), 0f, Mathf.Sin(i)), out hits[i], 1f))
            {

            }
            else
            {

            }
        }

        if (Physics.Raycast(transform.position, (goal.transform.position - transform.position).normalized, out hit, 1f))
        {

        }
        else
        {

        }
    }

    void debugRay() {

        for (int i = 0; i < 360; i++)
        {
            Debug.DrawRay(transform.position, new Vector3(Mathf.Cos(i), 0f, Mathf.Sin(i)) , Color.white, 0f, true);
        }
        Debug.DrawRay(transform.position, (goal.transform.position - transform.position).normalized, Color.red, 0f, true);
    }
    private void FixedUpdate()
    {
        debugRay();
        normalGo();
    }

    void normalGo()
    {
        playerRigidbody.velocity = (goal.transform.position - transform.position).normalized * 3f;
        Debug.Log((goal.transform.position - transform.position).normalized);
    }
}
