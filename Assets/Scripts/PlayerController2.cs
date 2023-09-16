using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class PlayerController2 : MonoBehaviour
{
    public Transform goal; // Goal 객체의 Transform 컴포넌트를 할당합니다.
    public float moveSpeed = 5f; // 이동 속도를 조절합니다.
    public float rotationSpeed = 45f; // 이동 각도 조절 가능

    private bool isMoving = true; // 이동 상태를 나타내는 플래그 변수입니다.
    private float initialY; // 초기 y 위치를 저장할 변수입니다.
    private Rigidbody playerRigidbody;
    private Vector3 StartingPoint;

    private ContactPoint con;
    private bool flag = false;
    private bool endFlag = true;
    private bool StartFlag = false;
    private bool ComeBackFlag = false;
    private bool EnterFlag = false;
    private bool LineFlag = false;

    public float speed = 1f;

    Vector3 contactNormal;
    Vector3 perpendicularToXZPlane;
    Vector3 force;
    private int count = 0;

    private Vector3 minPos;
    private Vector3 initPos;
    private double min = 10000;

    private void Start()
    {
        // 초기 y 위치를 저장합니다.
        initialY = transform.position.y;
        playerRigidbody = GetComponent<Rigidbody>();
        StartingPoint = playerRigidbody.transform.position;
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            if (goal != null)
            {
                Vector3 direction = (goal.position - transform.position).normalized * 1.5f * speed;
                playerRigidbody.rotation = Quaternion.LookRotation(goal.position);
                playerRigidbody.velocity = new Vector3(direction.x, 0, direction.z);
                print("gogogo");
            }
            else
            {
                Debug.LogWarning("Goal이 할당되지 않았습니다.");
            }
        }
        else
        {
            if (line())
            {
                playerRigidbody.velocity = Vector3.zero;
                endFlag = false;
                EnterFlag = false;
                isMoving = true;
                StartFlag = false;
                ComeBackFlag = false;
                flag = false;
                initPos = new Vector3(100f, 100f, 100f);
                min = 10000f;
                Invoke("lineend", 0.1f);
            }
            else
            {
                if (EnterFlag)
                {
                    if (min > (playerRigidbody.position.x - goal.position.x) * (playerRigidbody.position.x - goal.position.x) + (playerRigidbody.position.z - goal.position.z) * (playerRigidbody.position.z - goal.position.z))
                    {

                        min = (playerRigidbody.position.x - goal.position.x) * (playerRigidbody.position.x - goal.position.x) + (playerRigidbody.position.z - goal.position.z) * (playerRigidbody.position.z - goal.position.z);
                        minPos = playerRigidbody.position;
                    }
                    if (flag)
                    {
                        perpendicularToXZPlane = new Vector3(-contactNormal.z - contactNormal.x, 0.0f, contactNormal.x - contactNormal.z);
                        playerRigidbody.rotation = Quaternion.LookRotation(con.point);
                        force = perpendicularToXZPlane.normalized * 1.5f * speed;
                        playerRigidbody.velocity = force;
                    }

                    start();
                    comeback();
                    go();
                }
            }
        }

        if (!isMoving && !EnterFlag)
        {
            isMoving = true;
        }
    }

    void start()
    {
        if (!StartFlag && 1f < (initPos.x - playerRigidbody.position.x) * (initPos.x - playerRigidbody.position.x) + (initPos.z - playerRigidbody.position.z) * (initPos.z - playerRigidbody.position.z))
        {
            StartFlag = true;
            Debug.Log("Start");
        }
    }

    bool line()
    {
        //print(Mathf.Abs((goal.position.y - StartingPoint.y) / (goal.position.x - StartingPoint.x) - (goal.position.y - playerRigidbody.position.y) / (goal.position.x - playerRigidbody.position.x)));
        if (!isMoving && Mathf.Abs((goal.position.y - StartingPoint.y) / (goal.position.x - StartingPoint.x) - (goal.position.y - playerRigidbody.position.y) / (goal.position.x - playerRigidbody.position.x)) < 0.02f)
        {
            Ray ray = new Ray(playerRigidbody.position, goal.position);
            RaycastHit hitData;


            if (Physics.Raycast(ray, out hitData))
            {
                print(hitData.distance);
                if (hitData.distance > 1f)
                {
                    Debug.Log("line go");
                    LineFlag = true;
                    //lineend();
                }
            }
        }
        return LineFlag;
    }

    void lineend()
    {
        Debug.Log("lineend");
        Debug.Log(LineFlag);
        LineFlag = false;
        endFlag = true;
    }

    void comeback()
    {
        if (!ComeBackFlag && StartFlag && 0.1f > (initPos.x - playerRigidbody.position.x) * (initPos.x - playerRigidbody.position.x) + (initPos.z - playerRigidbody.position.z) * (initPos.z - playerRigidbody.position.z))
        {
            ComeBackFlag = true;
            Debug.Log("Comeback");
        }
    }

    void go()
    {
        if (ComeBackFlag && 0.01f > (minPos.x - playerRigidbody.position.x) * (minPos.x - playerRigidbody.position.x) + (minPos.z - playerRigidbody.position.z) * (minPos.z - playerRigidbody.position.z))
        {
            Debug.Log("Go");
            endFlag = false;
            EnterFlag = false;
            isMoving = true;
            StartFlag = false;
            ComeBackFlag = false;
            flag = false;
            initPos = new Vector3(100f, 100f, 100f);
            min = 10000f;
            Invoke("end", 0.1f);
        }
    }

    void end()
    {
        Debug.Log("end");
        endFlag = true;
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall" && endFlag)
        {
            if (isMoving == true)
            {
                initPos = playerRigidbody.position;
                EnterFlag = true;
                //Invoke("lineend", 0.1f);
            }
            count++;

            flag = false;
            if (ComeBackFlag != false)
            {
                isMoving = false;
            }
            foreach (ContactPoint contact in collision.contacts)
            {
                con = contact;
                contactNormal = contact.normal; // 법선 벡터

                // 이동할 거리를 결정합니다.

                perpendicularToXZPlane = new Vector3(-contactNormal.z, 0.0f, contactNormal.x);
                playerRigidbody.rotation = Quaternion.LookRotation(-contactNormal);
                // 오브젝트를 새로운 위치로 이동시킵니다.
                force = perpendicularToXZPlane.normalized * 3f * speed;

                playerRigidbody.velocity = force;
            }

        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Wall" && endFlag)
        {
            flag = false;
            isMoving = false;
            foreach (ContactPoint contact in collision.contacts)
            {
                con = contact;
                contactNormal = contact.normal; // 법선 벡터

                // 이동할 거리를 결정합니다.

                perpendicularToXZPlane = new Vector3(-contactNormal.z, 0.0f, contactNormal.x);
                playerRigidbody.rotation = Quaternion.LookRotation(-contactNormal);
                // 오브젝트를 새로운 위치로 이동시킵니다.
                force = perpendicularToXZPlane.normalized * 3.0f * speed;

                playerRigidbody.velocity = force;

                
            }

        }
    }


    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            count--;
            if (count == 0)
            {
                playerRigidbody.velocity = Vector3.zero;
                flag = true;
            }
        }
    }
}