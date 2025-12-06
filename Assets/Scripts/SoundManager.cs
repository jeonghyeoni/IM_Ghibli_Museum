using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("오디오 소스 연결")]
    public AudioSource sfxSource;

    [Header("공통 효과음")]
    public AudioClip pieceAppearClip; // 기존 오르골 조각 소리
    
    [Header("인터랙션 효과음 (추가됨)")]
    public AudioClip hoverClip; // 쳐다볼 때 (틱!)
    public AudioClip grabClip;  // 잡을 때 (탁!)

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayPieceSound()
    {
        if (sfxSource && pieceAppearClip) sfxSource.PlayOneShot(pieceAppearClip);
    }

    // [추가] 호버 소리 재생
    public void PlayHoverSound()
    {
        // 소리가 너무 자주 겹치지 않게 볼륨을 살짝 낮추거나 피치를 조절해도 좋습니다.
        if (sfxSource && hoverClip) sfxSource.PlayOneShot(hoverClip, 1.0f); 
    }

    // [추가] 그랩 소리 재생
    public void PlayGrabSound()
    {
        if (sfxSource && grabClip) sfxSource.PlayOneShot(grabClip, 1.0f);
    }
}