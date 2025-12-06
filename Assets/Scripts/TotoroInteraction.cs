using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors; // XRI 3.x

public class TotoroInteraction : MonoBehaviour
{
    [Header("토토로 설정")]
    public Animator totoroAnimator;

    [Header("상호작용 연결")]
    public XRSocketInteractor umbrellaSocket; // 우산 꽂는 곳
    public XRGrabInteractable acorn;          // 도토리 오브젝트

    void OnEnable()
    {
        // 이벤트 구독 (연결)
        if (umbrellaSocket != null)
            umbrellaSocket.selectEntered.AddListener(OnUmbrellaGiven);

        if (acorn != null)
            acorn.selectEntered.AddListener(OnAcornTaken);
    }

    void OnDisable()
    {
        // 이벤트 해제 (청소)
        if (umbrellaSocket != null)
            umbrellaSocket.selectEntered.RemoveListener(OnUmbrellaGiven);

        if (acorn != null)
            acorn.selectEntered.RemoveListener(OnAcornTaken);
    }

    // 1. 우산이 소켓에 들어왔을 때 -> 주는 자세 취하기
    private void OnUmbrellaGiven(SelectEnterEventArgs args)
    {
        // 혹시 들어온 게 우산인지 태그나 이름으로 확인할 수도 있지만,
        // 소켓 필터(Layer Mask)를 이미 해두셨으니 바로 실행합니다.
        if (totoroAnimator != null)
        {
            totoroAnimator.SetTrigger("Umbrella");
        }

        StartCoroutine(ActiveAcorn(2.5f));
    }

    // 2. 플레이어가 도토리를 잡았을 때 -> 다시 Idle로
    private void OnAcornTaken(SelectEnterEventArgs args)
    {
        // 플레이어 손(XR Ray/Direct Interactor)이 잡았는지 확인
        // (우산 소켓 같은 게 잡은 게 아니라면)
        if (totoroAnimator != null)
        {
            totoroAnimator.SetTrigger("Acorn");
        }
    }


    private System.Collections.IEnumerator ActiveAcorn(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        acorn.gameObject.SetActive(true); 
    }
}