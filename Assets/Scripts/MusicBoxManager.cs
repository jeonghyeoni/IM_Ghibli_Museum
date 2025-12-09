using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections;
using TMPro; // UI 텍스트 제어를 위해 필수

public class MusicBoxManager : MonoBehaviour
{
    [Header("UI 설정 (패널)")]
    public TextMeshProUGUI titleText;  // 제목 (Complete the Music Box)
    public TextMeshProUGUI statusText; // 상태 (0/3 Found...)

    [Header("오브젝트 교체 설정")]
    public GameObject interactiveObject; // 조립 중인 오르골
    public GameObject completedObject;   // 완성된 오르골 (교체용)
    public Transform completedHandle;    // 회전할 손잡이

    [Header("추가 회전 조각 설정")]
    [Tooltip("음악 나올 때 같이 돌릴 조각 (제자리에서 돕니다)")]
    public Transform extraRotatingPiece;

    [Header("소켓 설정")]
    public XRSocketInteractor[] sockets;

    [Header("오디오 설정")]
    public AudioSource audioSource;
    public AudioClip musicClip;

    [Header("애니메이션 설정")]
    public float playSpeed = 100.0f;

    [Header("환경 설정")]
    public DayNightManager dayNightManager;

    private bool isActivated = false; // 오르골 완성 여부

    void Start()
    {
        // 1. 오브젝트 초기 상태 설정
        if (completedObject != null) completedObject.SetActive(false);
        if (interactiveObject != null) interactiveObject.SetActive(true);

        // 2. UI 초기 텍스트 설정
        if (titleText != null) titleText.text = "Complete the Music Box";
        UpdateUIAndCheck(false); // 시작하자마자 현재 상태(0/3) 표시

        // 3. 소켓 이벤트 연결 (끼울 때 & 뺄 때 모두 감지)
        foreach (var socket in sockets)
        {
            if (socket != null)
            {
                socket.selectEntered.AddListener(OnSocketChanged);
                socket.selectExited.AddListener(OnSocketChanged);
            }
        }
    }

    // 소켓 상태가 변할 때 호출됨
    private void OnSocketChanged(BaseInteractionEventArgs args)
    {
        // 이미 완성되어 작동 중이라면 더 이상 체크하지 않음
        if (isActivated) return;

        UpdateUIAndCheck(true);
    }

    // 개수를 세고 UI를 갱신하고, 다 찼으면 실행
    private void UpdateUIAndCheck(bool checkCompletion)
    {
        int filledCount = 0;
        int totalSockets = sockets.Length;

        // 현재 채워진 소켓 개수 세기
        foreach (var socket in sockets)
        {
            if (socket.hasSelection) filledCount++;
        }

        // UI 텍스트 업데이트
        if (statusText != null)
        {
            if (filledCount < totalSockets)
            {
                // 진행 중
                statusText.text = $"{filledCount}/{totalSockets} Music box pieces found!";
            }
            else
            {
                // 완성 메시지
                statusText.text = "All the pieces have been found.\nEnjoy your music box!\nThank you for playing!";
            }
        }

        // 다 채워졌고, 아직 작동 전이라면 시퀀스 시작
        if (checkCompletion && filledCount == totalSockets && !isActivated)
        {
            StartCoroutine(StartMusicBoxSequence());
        }
    }

    IEnumerator StartMusicBoxSequence()
    {
        isActivated = true;

        // 잠시 대기 (마지막 조각 끼우는 모션과 사운드가 겹치지 않게)
        yield return new WaitForSeconds(0.5f);

        // 1. 소켓에 꽂혀있는 조각들 숨기기 (완성품 모델로 교체할 거니까)
        foreach (var socket in sockets)
        {
            if (socket.hasSelection)
            {
                var fragment = socket.firstInteractableSelected.transform.gameObject;
                if (fragment != null) fragment.SetActive(false);
            }
        }

        // 2. 오르골 본체 교체 (조립형 -> 완성형)
        if (interactiveObject != null) interactiveObject.SetActive(false);
        if (completedObject != null) completedObject.SetActive(true);

        // 밤으로 전환
        if (dayNightManager != null)
        {
            dayNightManager.StartNightSequence();
        }

        // 3. 음악 재생
        if (audioSource != null && musicClip != null)
        {
            audioSource.clip = musicClip;
            audioSource.loop = false;
            audioSource.Play();
        }

        // 4. 회전 루프 (음악이 끝날 때까지)
        while (audioSource.isPlaying)
        {
            float rotateAmount = playSpeed * Time.deltaTime;

            // (A) 손잡이 회전
            if (completedHandle != null)
            {
                completedHandle.Rotate(-Vector3.forward * rotateAmount);
            }

            // (B) 추가 조각 제자리 회전
            if (extraRotatingPiece != null)
            {
                extraRotatingPiece.Rotate(Vector3.right * rotateAmount, Space.Self);
            }

            yield return null;
        }
    }
}