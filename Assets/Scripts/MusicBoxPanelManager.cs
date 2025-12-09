using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro 사용을 위해 필수
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors; // XRI 3.x

public class MusicBoxPanelManager : MonoBehaviour
{
    [Header("UI 설정")]
    public TextMeshProUGUI statusText; // 상태 텍스트 (0/3 Found...)

    [Header("오르골 소켓 연결")]
    // 오르골 부품을 끼울 소켓 3개를 여기에 등록하세요.
    public List<XRSocketInteractor> sockets; 

    private int maxPieces = 3;

    void Start()
    {
        // 처음 상태 업데이트
        UpdatePieceCount();
    }

    void OnEnable()
    {
        // 모든 소켓에 이벤트 연결 (부품 끼움/뺌 감지)
        foreach (var socket in sockets)
        {
            if (socket != null)
            {
                socket.selectEntered.AddListener(OnSocketChanged);
                socket.selectExited.AddListener(OnSocketChanged);
            }
        }
    }

    void OnDisable()
    {
        // 이벤트 연결 해제
        foreach (var socket in sockets)
        {
            if (socket != null)
            {
                socket.selectEntered.RemoveListener(OnSocketChanged);
                socket.selectExited.RemoveListener(OnSocketChanged);
            }
        }
    }

    // 소켓에 무언가 들어오거나 나갔을 때 호출
    private void OnSocketChanged(SelectEnterEventArgs args) => UpdatePieceCount();
    private void OnSocketChanged(SelectExitEventArgs args) => UpdatePieceCount();

    // 개수를 세고 텍스트를 바꾸는 핵심 로직
    private void UpdatePieceCount()
    {
        int currentCount = 0;

        // 현재 부품이 끼워져 있는 소켓의 개수를 셉니다.
        foreach (var socket in sockets)
        {
            if (socket != null && socket.hasSelection)
            {
                currentCount++;
            }
        }

        // 텍스트 업데이트 로직
        if (statusText != null)
        {
            if (currentCount < maxPieces)
            {
                // 아직 다 못 모았을 때 (예: 1/3 Music box pieces found!)
                statusText.text = $"{currentCount}/{maxPieces} Music box pieces found!";
            }
            else
            {
                // 3개를 다 모았을 때 (줄바꿈은 \n)
                statusText.text = "All the pieces have been found\nEnjoy your music box! Thank you for playing!";
                
                // (선택사항) 여기서 엔딩 음악을 재생하거나 파티클을 터뜨리는 함수를 호출해도 좋습니다.
                OnCompleteMusicBox(); 
            }
        }
    }

    private void OnCompleteMusicBox()
    {
        Debug.Log("오르골 완성! 엔딩 연출 시작");
        // 여기에 오르골 연주 시작 코드를 넣으시면 됩니다.
    }
}