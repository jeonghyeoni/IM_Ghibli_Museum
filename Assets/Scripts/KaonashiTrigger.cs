using UnityEngine;
using System.Collections; // Coroutine 사용을 위해 추가

public class KaonashiTrigger : MonoBehaviour
{
    [Header("연결할 오브젝트")]
    public Animator boxAnimator; // 상자 애니메이터
    public GameObject musicBoxPiece; // 상자 안의 오르골 조각
    
    [Header("UI 및 연출 설정")]
    public GameObject messagePanel;  // ✨ 추가: 띄울 안내 패널
    public float openDelay = 1.5f;   // ✨ 추가: 상자가 다 열릴 때까지 기다릴 시간

    [Header("발판 움직임 설정")]
    public Transform plateModel; 
    public float pressDepth = 0.05f; 
    public float moveSpeed = 5.0f;   

    [Header("효과음")]
    public AudioSource audioSource;
    public AudioClip pressSound;   // 발판 밟는 소리
    public AudioClip boxOpenSound; // ✨ 추가: 상자 열리는 소리

    // 내부 변수
    private bool isOpened = false; 
    private Vector3 initialLocalPos; 
    private Vector3 targetLocalPos;  

    void Start()
    {
        if (plateModel != null)
        {
            initialLocalPos = plateModel.localPosition;
            targetLocalPos = initialLocalPos;
        }
        
        // 시작할 때 아이템과 패널은 숨겨둡니다 (안전을 위해)
        if (musicBoxPiece != null) musicBoxPiece.SetActive(false);
        if (messagePanel != null) messagePanel.SetActive(false);
    }

    void Update()
    {
        if (plateModel != null)
        {
            plateModel.localPosition = Vector3.Lerp(plateModel.localPosition, targetLocalPos, Time.deltaTime * moveSpeed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Kaonashi"))
        {
            // 발판 내리기
            targetLocalPos = initialLocalPos - new Vector3(0, pressDepth, 0);

            // 발판 소리 재생
            if (audioSource != null && pressSound != null)
            {
                audioSource.PlayOneShot(pressSound);
            }

            // 상자 열기 (아직 안 열렸다면)
            if (!isOpened)
            {
                OpenBox();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Kaonashi"))
        {
            targetLocalPos = initialLocalPos;
        }
    }

    void OpenBox()
    {
        isOpened = true;
        Debug.Log("가오나시 도착! 상자 오픈!");

        // 1. 애니메이션 실행
        if (boxAnimator != null)
        {
            boxAnimator.SetTrigger("Open");
        }

        // 2. ✨ 추가: 상자 열리는 효과음 재생
        if (audioSource != null && boxOpenSound != null)
        {
            audioSource.PlayOneShot(boxOpenSound);
        }

        // 3. 지연 후 아이템/패널 등장 (코루틴 시작)
        StartCoroutine(ShowRewardRoutine());
    }

    // ✨ 추가: 애니메이션 시간만큼 기다렸다가 보상을 보여주는 코루틴
    IEnumerator ShowRewardRoutine()
    {
        // 상자 문이 열리는 시간만큼 대기 (Inspector에서 조절 가능)
        yield return new WaitForSeconds(openDelay);

        // 오르골 조각 등장
        if (musicBoxPiece != null)
        {
            musicBoxPiece.SetActive(true);
        }

        // 안내 패널 등장
        if (messagePanel != null)
        {
            messagePanel.SetActive(true);
        }
    }
}