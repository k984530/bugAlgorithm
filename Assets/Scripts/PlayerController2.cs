using UnityEngine;


public class PlayerController2 : MonoBehaviour
{
    public Transform goal; // Goal 객체의 Transform 컴포넌트를 할당합니다.

    private bool isMoving = true; // 이동 상태를 나타내는 플래그 변수입니다.
    public Transform startpoint;
    private Rigidbody playerRigidbody;


    private ContactPoint con;
    private bool flag = false;
    private bool endFlag = true;
    private bool ComeBackFlag = false;
    private bool EnterFlag = false;
    private bool lineFlag = false;


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
        playerRigidbody = GetComponent<Rigidbody>();
    }

    //업데이트 예정 Bug 1과 구분하기 위함 
    public void OnEnable()
    {
        playerRigidbody.velocity = Vector3.zero;
        isMoving = true;
        flag = false;
        endFlag = true;
        ComeBackFlag = false;
        EnterFlag = false;
        lineFlag = false;
        count = 0;
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
            }
            else
            {
                Debug.LogWarning("Goal이 할당되지 않았습니다.");
            }
        }
        else
        {
            if (lineFlag && getDistancePointAndLine(startpoint.position, goal.position, transform.position) < 0.30)
            {
                isMoving = true;
                lineFlag = false;
            }
            else
            {

                if (min > (playerRigidbody.position.x - goal.position.x) * (playerRigidbody.position.x - goal.position.x) + (playerRigidbody.position.z - goal.position.z) * (playerRigidbody.position.z - goal.position.z))
                { // 그냥 벽을 짚고 가는 경우 

                    min = (playerRigidbody.position.x - goal.position.x) * (playerRigidbody.position.x - goal.position.x) + (playerRigidbody.position.z - goal.position.z) * (playerRigidbody.position.z - goal.position.z);
                    minPos = playerRigidbody.position;
                }
                if (flag) // 장애물의 꼭짓점을 만난 경우 
                {
                    perpendicularToXZPlane = new Vector3(-contactNormal.z - contactNormal.x, 0.0f, contactNormal.x - contactNormal.z);
                    playerRigidbody.rotation = Quaternion.LookRotation(con.point);
                    force = perpendicularToXZPlane.normalized * 1.5f * speed;
                    playerRigidbody.velocity = force;
                }
            }
        }
    }


    //장애물 인식
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall" && endFlag)
        {
            if (isMoving == true)
            {
                initPos = playerRigidbody.position;
                isMoving = false;
                EnterFlag = true;
                Invoke("enterCol", 0.2f);
            }
            count++;

            flag = false;
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
    // 스타트지점과 목표지점을 이은 선과의 거리를 추적함 
    float getDistancePointAndLine(Vector3 A, Vector3 B, Vector3 point)
    {
        Vector3 AB = B - A;
        return (Vector3.Cross(point - A, AB)).magnitude / AB.magnitude;
    }
    // 선을 만남
    void enterCol()
    {
        lineFlag = true;
    }

    // 벽을 짚고 감
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Wall" && endFlag)
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
                force = perpendicularToXZPlane.normalized * 3.0f * speed;

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
            if (count == 0)
            {
                playerRigidbody.velocity = Vector3.zero;
                flag = true;
            }
        }
    }
}
