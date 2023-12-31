using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    public Transform startpoint;
    public Transform goal; // Goal 객체의 Transform 컴포넌트를 할당합니다.

    private bool isMoving = true; // 이동 상태를 나타내는 플래그 변수입니다.
    private float initialY; // 초기 y 위치를 저장할 변수입니다.
    private Rigidbody playerRigidbody;

    private ContactPoint con;
    private bool flag = false;
    private bool endFlag = true;
    private bool StartFlag = false;
    private bool ComeBackFlag = false;
    private bool EnterFlag = false;

    public float speed =1f;

    Vector3 contactNormal;
    Vector3 perpendicularToXZPlane;
    Vector3 force;
    private int count = 0;

    private Vector3 minPos;
    private Vector3 initPos;
    private double min = 10000;

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();

    }

    public void OnEnable()
    {
        playerRigidbody.velocity = Vector3.zero;
        isMoving = true;
        flag = false;
        endFlag = true;
        ComeBackFlag = false;
        EnterFlag = false;
        count = 0;
    }

    private void FixedUpdate()
    {
        
        if (isMoving)
        {
            if (goal != null)
            {
                // Goal 객체 방향을 향해 회전합니다.
                Vector3 direction = (goal.position - transform.position).normalized * 1.5f * speed;
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = rotation;

                playerRigidbody.velocity = new Vector3(direction.x, 0, direction.z);
            }
            else
            {
                Debug.LogWarning("Goal이 할당되지 않았습니다.");
            }
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

        if(!isMoving && !EnterFlag)
        {
            isMoving = true;
        }
    }
    
    //장애물을 만남
    void start()
    {
        if(!StartFlag && 1f < (initPos.x - playerRigidbody.position.x) * (initPos.x - playerRigidbody.position.x) + (initPos.z - playerRigidbody.position.z) * (initPos.z - playerRigidbody.position.z))
        {
            StartFlag = true;
            Debug.Log("Start");
        }
    }
    
    //장애물을 만나고 한 바퀴 둘러봄
    void comeback()
    {
        if (!ComeBackFlag && StartFlag && 0.1f > (initPos.x - playerRigidbody.position.x) * (initPos.x - playerRigidbody.position.x) + (initPos.z - playerRigidbody.position.z) * (initPos.z - playerRigidbody.position.z))
        {
            ComeBackFlag = true;
            Debug.Log("Comeback");
        }
    }

    // 목적지와 가장 가까운 지점에서 탈출함
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

    // 장애물에서 떠나고 떠난 장애물을 인식하지 않기 위함
    void end()
    {
        Debug.Log("end");
        endFlag = true;
    }

    // 장애물을 만났을 때 실행
    void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject);
        if (collision.gameObject.tag == "Wall" && endFlag)
        {
            if(isMoving == true)
            {
                initPos = playerRigidbody.position;
                EnterFlag = true;
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
                force = perpendicularToXZPlane.normalized * 2.0f * speed;

                playerRigidbody.velocity = force;
            }

        }
    }

    // 장애물을 만나고 있는 /
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
                force = perpendicularToXZPlane.normalized * 2.0f * speed;

                playerRigidbody.velocity = force;
            }

        }
    }

    // 장애물을 돌다가 벽을 못 짚은 경우
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
