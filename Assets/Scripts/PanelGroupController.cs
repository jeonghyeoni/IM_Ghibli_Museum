using UnityEngine;
using System.Collections.Generic;

public class PanelGroupController : MonoBehaviour
{
    [Header("패널 설정")]
    public List<GameObject> panels = new List<GameObject>();

    [Header("사운드 설정")]
    public AudioSource audioSource;   // 소리를 낼 스피커 컴포넌트
    public AudioClip changeSound;     // 재생할 오디오 파일 (mp3, wav 등)
    
    private int currentIndex = 0;

    void Start()
    {
        if (panels.Count == 0)
        {
            foreach (Transform child in transform)
            {
                panels.Add(child.gameObject);
            }
        }

        ShowCurrentPanelOnly();
    }

    public void GoNext()
    {
        if (panels.Count == 0) return;

        panels[currentIndex].SetActive(false);

        currentIndex++;
        if (currentIndex >= panels.Count) currentIndex = 0;

        panels[currentIndex].SetActive(true);
        
        // 소리 재생
        PlaySound();
    }

    public void GoPrev()
    {
        if (panels.Count == 0) return;

        panels[currentIndex].SetActive(false);

        currentIndex--;
        if (currentIndex < 0) currentIndex = panels.Count - 1;

        panels[currentIndex].SetActive(true);

        // 소리 재생
        PlaySound();
    }

    private void ShowCurrentPanelOnly()
    {
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(i == currentIndex);
        }
    }

    // 소리 재생 전용 함수
    private void PlaySound()
    {
        // 오디오 소스와 클립이 모두 연결되어 있을 때만 소리 냄
        if (audioSource != null && changeSound != null)
        {
            // PlayOneShot은 소리가 겹쳐도 끊기지 않고 자연스럽게 냅니다
            audioSource.PlayOneShot(changeSound);
        }
    }
}