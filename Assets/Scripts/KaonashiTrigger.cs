using UnityEngine;

public class KaonashiTrigger : MonoBehaviour
{
    [Header("연결할 오브젝트")]
    public Animator boxAnimator; // 상자 애니메이터
    public GameObject musicBoxPiece; // 상자 안의 오르골 조각
    
    [Header("발판 움직임 설정")]
    public Transform plateModel; // 실제 눈에 보이는 발판 모델 (움직일 녀석)
    public float pressDepth = 0.05f; // 얼마나 깊이 눌릴지 (예: 0.05)
    public float moveSpeed = 5.0f;   // 움직이는 속도

    [Header("효과음")]
    public AudioSource audioSource;
    public AudioClip pressSound; // 밟았을 때 소리

    // 내부 변수
    private bool isOpened = false; // 상자 오픈 여부 확인용
    private Vector3 initialLocalPos; // 발판의 원래 위치
    private Vector3 targetLocalPos;  // 발판이 이동할 목표 위치

    void Start()
    {
        // 시작할 때 발판의 원래 위치를 기억해둡니다.
        if (plateModel != null)
        {
            initialLocalPos = plateModel.localPosition;
            targetLocalPos = initialLocalPos;
        }
    }

    void Update()
    {
        // 매 프레임 발판을 목표 위치로 부드럽게 이동시킵니다.
        if (plateModel != null)
        {
            plateModel.localPosition = Vector3.Lerp(plateModel.localPosition, targetLocalPos, Time.deltaTime * moveSpeed);
        }
    }

    // 가오나시가 밟았을 때 (내려가기)
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Kaonashi"))
        {
            // 1. 발판 목표 위치를 아래로 설정 (Y축으로 pressDepth만큼 뺌)
            targetLocalPos = initialLocalPos - new Vector3(0, pressDepth, 0);

            // 2. 효과음 재생 (누를 때마다 소리가 나게 하려면 여기 둠)
            if (audioSource != null && pressSound != null)
            {
                audioSource.PlayOneShot(pressSound);
            }

            // 3. 상자 열기 로직 (한 번만 실행됨)
            if (!isOpened)
            {
                OpenBox();
            }
        }
    }

    // 가오나시가 발판에서 나갔을 때 (올라오기)
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Kaonashi"))
        {
            // 발판 목표 위치를 다시 원래대로 설정
            targetLocalPos = initialLocalPos;
        }
    }

    void OpenBox()
    {
        isOpened = true;
        Debug.Log("가오나시 도착! 상자 오픈!");

        // 상자 열기 애니메이션 실행
        if (boxAnimator != null)
        {
            boxAnimator.SetTrigger("Open");
        }

        // 오르골 조각 활성화
        if (musicBoxPiece != null)
        {
            musicBoxPiece.SetActive(true);
            // 만약 SoundManager가 없다면 이 줄은 에러가 날 수 있으니 주석 처리하거나 확인 필요
            // SoundManager.Instance.PlayPieceSound(); 
        }
    }
}