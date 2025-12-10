using UnityEngine;
using System.Collections; 

public class KaonashiTrigger : MonoBehaviour
{
    [Header("연결할 오브젝트")]
    public Animator boxAnimator; // 상자 애니메이터
    public GameObject musicBoxPiece; // 상자 안의 오르골 조각
    
    [Header("UI 패널 설정")]
    [Tooltip("예: 오르골 부품 발견 패널")]
    public GameObject itemPanel; 

    [Tooltip("예: 잘했습니다! 변장 해제 안내 패널")]
    public PanelPopupBehavior guidePanel; // ✨ 추가: 두 번째 패널

    [Header("연출 설정")]
    public float openDelay = 1.5f;   // 상자가 다 열릴 때까지 기다릴 시간

    [Header("발판 움직임 설정")]
    public Transform plateModel; 
    public float pressDepth = 0.05f; 
    public float moveSpeed = 5.0f;   

    [Header("효과음")]
    public AudioSource audioSource;
    public AudioClip pressSound;   // 발판 밟는 소리
    public AudioClip boxOpenSound; // 상자 열리는 소리

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
        
        // 시작할 때 아이템은 숨겨둡니다
        if (musicBoxPiece != null) musicBoxPiece.SetActive(false);
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

        // 2. 상자 열리는 효과음 재생
        if (audioSource != null && boxOpenSound != null)
        {
            audioSource.PlayOneShot(boxOpenSound);
        }

        // 3. 지연 후 아이템/패널 2개 등장 (코루틴 시작)
        StartCoroutine(ShowRewardRoutine());
    }

    IEnumerator ShowRewardRoutine()
    {
        // 상자 문이 열리는 시간만큼 대기
        yield return new WaitForSeconds(openDelay);

        // 오르골 조각 등장
        if (musicBoxPiece != null)
        {
            musicBoxPiece.SetActive(true);
        }

        // ✨ 패널 1: 아이템 발견 (You found a Music Box Part!)
        if (itemPanel != null)
        {
            itemPanel.SetActive(true);
        }

        // ✨ 패널 2: 가이드 안내 (Well Done! Remove Disguise...)
        if (guidePanel != null)
        {
            guidePanel.ShowPanel();
        }
    }
}