using UnityEngine;

public class KaonashiFollowAI : MonoBehaviour
{
    [Header("추적 대상")]
    public Transform targetChihiro; // 치히로 오브젝트 넣기

    [Header("움직임 설정")]
    public float moveSpeed = 2.0f;       // 따라가는 속도
    public float stopDistance = 1.5f;    // 치히로와 유지할 거리 (너무 붙지 않게)
    public float rotationSpeed = 5.0f;   // 회전 속도
    public float smoothTime = 0.5f;      // 지연 시간 (클수록 반응이 느리고 부드러움)

    private Vector3 currentVelocity; // SmoothDamp용 변수

    void Update()
    {
        if (targetChihiro == null) return;

        // 1. 거리 계산
        float distance = Vector3.Distance(transform.position, targetChihiro.position);

        // 2. 멈춤 거리보다 멀리 있을 때만 이동
        if (distance > stopDistance)
        {
            // 목표 지점: 치히로의 위치 (y축 높이는 가오나시 현재 높이 유지)
            Vector3 targetPos = targetChihiro.position;
            targetPos.y = transform.position.y; 

            // 부드럽게 이동 (SmoothDamp가 딜레이 효과를 줌)
            transform.position = Vector3.SmoothDamp(
                transform.position, 
                targetPos, 
                ref currentVelocity, 
                smoothTime, 
                moveSpeed
            );

            // 3. 치히로 바라보기 (부드럽게 회전)
            Vector3 direction = (targetPos - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
}