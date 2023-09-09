using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    public Transform goal; // Goal 객체의 Transform 컴포넌트를 할당합니다.
    public float moveSpeed = 5f; // 이동 속도를 조절합니다.
    public float rotationSpeed = 45f; // 이동 각도 조절 가능

    private bool isMoving = true; // 이동 상태를 나타내는 플래그 변수입니다.
    private float initialY; // 초기 y 위치를 저장할 변수입니다.
    private Rigidbody playerRigidbody;

    public Color rayColor = Color.red; // 레이의 색상
    public float rayLength = 5.1f; // 레이의 길이

    Vector3 contactNormal;
    Vector3 perpendicularToXZPlane;
    Vector3 force;

    private void Start()
    {
        // 초기 y 위치를 저장합니다.
        initialY = transform.position.y;
        playerRigidbody = GetComponent<Rigidbody>();

    }

    private void Update()
    {
        if (isMoving)
        {
            if (goal != null)
            {
                // Goal 객체 방향을 향해 회전합니다.
                Vector3 direction = goal.position - transform.position;
                direction.y = 0f; // y 방향 이동을 제한합니다.
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = rotation;

                // Goal 객체 방향으로 이동합니다.
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

                // y 위치를 초기값으로 유지합니다.
                Vector3 newPosition = transform.position;
                newPosition.y = initialY;
                transform.position = newPosition;
            }
            else
            {
                Debug.LogWarning("Goal이 할당되지 않았습니다.");
            }
        }
        else
        {
            Vector3 rayOrigin = transform.position + new Vector3(-0.5f,0f,0f);
            Vector3 rayDirection = transform.forward;

            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength))
            {
                // 레이가 어떤 물체에 부딪혔다면, 부딪힌 지점에서 레이를 그립니다.
                //Debug.DrawRay(rayOrigin, rayDirection * hit.distance, rayColor);
                //if (hit.distance > 1.1f)
                //{
                //    playerRigidbody.velocity = new Vector3(transform.rotation.x, 0f, transform.rotation.z).normalized * 2f;
                //    Debug.Log("test");
                //}
            }
            else
            {
                //Debug.Log("a");
                ////playerRigidbody.velocity = Vector3.zero;
                //// 레이가 아무것도 부딪히지 않았다면, 원하는 길이만큼 레이를 그립니다.
                //Debug.DrawRay(rayOrigin, rayDirection * rayLength, rayColor);
                //playerRigidbody.velocity =new Vector3(transform.rotation.x,0f, transform.rotation.z).normalized * 2f;


            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Debug.Log("ttt");
            isMoving = false;
            foreach (ContactPoint contact in collision.contacts)
            {
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
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Debug.Log("aaa");
            playerRigidbody.velocity = Vector3.zero;


        }
    }
}
