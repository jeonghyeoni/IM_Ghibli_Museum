using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    [Header("둥둥 떠다니는 설정")]
    [Tooltip("위아래로 움직이는 높이 (얼마나 높게/낮게 갈지)")]
    public float amplitude = 0.1f; // 0.1m 정도면 살짝살짝 움직임

    [Tooltip("움직이는 속도 (얼마나 빨리 위아래로 왕복할지)")]
    public float frequency = 1.0f; // 1초에 한 번 사이클

    // 내부 변수
    private Vector3 startPos;
    private float randomOffset; // 두 마리가 똑같이 움직이면 로봇 같으니 랜덤 값 추가

    void Start()
    {
        // 1. 게임 시작 시 원래 위치를 기억합니다.
        startPos = transform.position;
        
        // 2. 햄스터랑 새가 서로 다른 타이밍에 움직이게 랜덤 시작점을 줍니다.
        randomOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    void Update()
    {
        // 3. 사인(Sin) 함수를 이용해 부드러운 파동을 만듭니다.
        // 공식: 원래위치 + (Sin(시간 * 속도 + 랜덤) * 높이)
        float newY = startPos.y + Mathf.Sin(Time.time * frequency + randomOffset) * amplitude;

        // 4. 새로운 Y값 적용 (X, Z는 원래 위치 유지)
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}