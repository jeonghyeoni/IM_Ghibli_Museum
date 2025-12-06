using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Interactable 관련
using UnityEngine.XR.Interaction.Toolkit.Interactors;   // [중요] XRBaseInteractor가 여기 들어있습니다.

public class HandAudioFeedback : MonoBehaviour
{
    // [수정] IXRInteractor -> XRBaseInteractor (클래스로 변경)
    private XRBaseInteractor interactor; 

    void Awake()
    {
        // 내 오브젝트에 붙어있는 Ray 또는 Direct Interactor를 찾아옵니다.
        interactor = GetComponent<XRBaseInteractor>();
    }

    void OnEnable()
    {
        if (interactor != null)
        {
            // XRBaseInteractor 클래스에는 이 이벤트들이 정의되어 있습니다.
            interactor.hoverEntered.AddListener(OnHover);
            interactor.selectEntered.AddListener(OnGrab);
        }
    }

    void OnDisable()
    {
        if (interactor != null)
        {
            interactor.hoverEntered.RemoveListener(OnHover);
            interactor.selectEntered.RemoveListener(OnGrab);
        }
    }

    private void OnHover(HoverEnterEventArgs args)
    {
        // 1. 잡을 수 있는 물건(Grab Interactable)인지 확인
        // 2. 혹시라도 소켓이나 다른 기계장치면 무시 (보통 이 스크립트는 손에 붙이므로 이 검사는 생략 가능하지만 안전하게)
        if (args.interactableObject is XRGrabInteractable)
        {
            SoundManager.Instance.PlayHoverSound();
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (args.interactableObject is XRGrabInteractable)
        {
            SoundManager.Instance.PlayGrabSound();
        }
    }
}