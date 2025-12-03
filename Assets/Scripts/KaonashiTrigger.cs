using UnityEngine;

public class KaonashiTrigger : MonoBehaviour
{
    [Header("연결할 오브젝트")]
    public Animator boxAnimator; // 상자 애니메이터
    public GameObject musicBoxPiece; // 상자 안의 오르골 조각
    
    [Header("효과음 (선택)")]
    public AudioSource audioSource;
    public AudioClip boxOpenSound;

    private bool isOpened = false; // 이미 열렸는지 확인

    // 무언가가 발판(Trigger)에 들어왔을 때 실행
    void OnTriggerEnter(Collider other)
    {
        // 1. 이미 열렸으면 무시
        if (isOpened) return;

        // 2. 들어온 녀석이 "Kaonashi"인지 확인
        if (other.CompareTag("Kaonashi"))
        {
            OpenBox();
        }
    }

    void OpenBox()
    {
        isOpened = true;
        Debug.Log("가오나시 도착! 상자 오픈!");

        // 3. 상자 열기 애니메이션 실행
        if (boxAnimator != null)
        {
            boxAnimator.SetTrigger("Open");
        }

        // 4. 오르골 조각 활성화 (애니메이션에 포함 안 되어 있다면)
        if (musicBoxPiece != null)
        {
            musicBoxPiece.SetActive(true);
        }

        // 5. 소리 재생
        if (audioSource != null && boxOpenSound != null)
        {
            audioSource.PlayOneShot(boxOpenSound);
        }
    }
}