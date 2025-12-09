using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Unity 6 / XRI 3.x
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TotoroInteraction : MonoBehaviour
{
    [Header("토토로 설정")]
    public Animator totoroAnimator;

    [Header("상호작용 연결")]
    public XRSocketInteractor umbrellaSocket; // 우산 꽂는 곳
    public XRGrabInteractable acorn;          // 도토리 오브젝트
    
    [Header("UI 설정")]
    public GameObject messagePanel;           // ✨ 새로 추가: 띄울 패널 (Canvas 혹은 오브젝트)

    void OnEnable()
    {
        if (umbrellaSocket != null)
            umbrellaSocket.selectEntered.AddListener(OnUmbrellaGiven);

        if (acorn != null)
            acorn.selectEntered.AddListener(OnAcornTaken);
    }

    void OnDisable()
    {
        if (umbrellaSocket != null)
            umbrellaSocket.selectEntered.RemoveListener(OnUmbrellaGiven);

        if (acorn != null)
            acorn.selectEntered.RemoveListener(OnAcornTaken);
    }

    // 1. 우산이 소켓에 들어왔을 때
    private void OnUmbrellaGiven(SelectEnterEventArgs args)
    {
        if (totoroAnimator != null)
        {
            totoroAnimator.SetTrigger("Umbrella");
        }

        // 애니메이션 시간(약 2초) + 여유 0.5초 뒤에 실행
        StartCoroutine(ActiveAcorn(2.5f));
    }

    // 2. 플레이어가 도토리를 잡았을 때
    private void OnAcornTaken(SelectEnterEventArgs args)
    {
        if (totoroAnimator != null)
        {
            totoroAnimator.SetTrigger("Acorn");
        }
    }

    private System.Collections.IEnumerator ActiveAcorn(float seconds)
    {
        // 2.5초 대기 (토토로가 주섬주섬 꺼내는 시간)
        yield return new WaitForSeconds(seconds);
        
        // 도토리 등장
        acorn.gameObject.SetActive(true); 

        // ✨ 추가: 안내 패널 등장!
        if (messagePanel != null)
        {
            messagePanel.SetActive(true);
        }
    }
}