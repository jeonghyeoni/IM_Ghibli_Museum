using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MusicBoxManager : MonoBehaviour
{
    [Header("오브젝트 교체 설정")]
    public GameObject interactiveObject;
    public GameObject completedObject;
    public Transform completedHandle;

    [Header("추가 회전 조각 설정")]
    [Tooltip("음악 나올 때 같이 돌릴 조각 (제자리에서 돕니다)")]
    public Transform extraRotatingPiece;

    // [삭제됨] Pivot 오브젝트는 필요 없습니다. 
    // Rotate 함수 자체가 Pivot 기준으로 돌기 때문입니다.

    [Header("소켓 설정")]
    public XRSocketInteractor[] sockets;

    [Header("오디오 설정")]
    public AudioSource audioSource;
    public AudioClip musicClip;

    [Header("애니메이션 설정")]
    public float playSpeed = 100.0f;

    private bool isActivated = false;

    [Header("환경 설정")]
    public DayNightManager dayNightManager;

    void Start()
    {
        if (completedObject != null) completedObject.SetActive(false);
        if (interactiveObject != null) interactiveObject.SetActive(true);

        foreach (var socket in sockets)
        {
            socket.selectEntered.AddListener(CheckSockets);
        }
    }

    private void CheckSockets(SelectEnterEventArgs args)
    {
        if (isActivated) return;

        int filledCount = 0;
        foreach (var socket in sockets)
        {
            if (socket.hasSelection) filledCount++;
        }

        if (filledCount == sockets.Length)
        {
            StartCoroutine(StartMusicBoxSequence());
        }
    }

    IEnumerator StartMusicBoxSequence()
    {
        isActivated = true;

        // 1. 소켓 조각 숨기기
        foreach (var socket in sockets)
        {
            if (socket.hasSelection)
            {
                var fragment = socket.firstInteractableSelected.transform.gameObject;
                if (fragment != null) fragment.SetActive(false);
            }
        }

        // 2. 오르골 본체 교체
        if (interactiveObject != null) interactiveObject.SetActive(false);
        if (completedObject != null) completedObject.SetActive(true);

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

        // 4. 회전 루프
        while (audioSource.isPlaying)
        {
            float rotateAmount = playSpeed * Time.deltaTime;

            // (A) 손잡이 회전
            if (completedHandle != null)
            {
                completedHandle.Rotate(-Vector3.forward * rotateAmount);
            }

            // (B) 추가 조각 제자리 회전 (Pivot 모드)
            if (extraRotatingPiece != null)
            {
                // Space.Self를 명시하면 '로컬 축' 기준으로 돕니다.
                // Vector3.right는 빨간색 화살표(X축)입니다.
                extraRotatingPiece.Rotate(Vector3.right * rotateAmount, Space.Self);
            }

            yield return null;
        }
    }
}