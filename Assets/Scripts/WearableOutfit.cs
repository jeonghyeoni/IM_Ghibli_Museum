using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Unity 6 XRI 필수

public class WearableOutfit : MonoBehaviour
{
    [Header("착용 설정")]
    public Transform playerCamera; // XR Origin의 Main Camera를 넣기
    public Vector3 wearOffset = new Vector3(0, 0, 0.1f); // 카메라 기준 옷 위치
    public Vector3 wearRotation = new Vector3(0, 0, 0); // 옷의 회전값

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private bool isEquipped = false; // 현재 입고 있는지 여부

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        // XRI 이벤트 연결
        grabInteractable.activated.AddListener(OnActivate);
        grabInteractable.selectEntered.AddListener(OnGrab);
    }

    void OnDisable()
    {
        grabInteractable.activated.RemoveListener(OnActivate);
        grabInteractable.selectEntered.RemoveListener(OnGrab);
    }

    // 1. 트리거 버튼(Activate)을 눌렀을 때 -> 옷 입기
    private void OnActivate(ActivateEventArgs args)
    {
        if (isEquipped) return; // 이미 입고 있으면 패스

        // 1-1. 현재 잡고 있는 손에서 강제로 놓게 만듦 (Drop)
        if (args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor selectInteractor)
        {
            grabInteractable.interactionManager.SelectExit(selectInteractor, grabInteractable);
        }

        // 1-2. 플레이어 몸(카메라)에 착용 (부모 설정)
        transform.SetParent(playerCamera);
        
        // 1-3. 위치 및 회전 정렬 (목 아래로)
        transform.localPosition = wearOffset;
        transform.localEulerAngles = wearRotation;

        // 1-4. 물리 끄기 (몸에 붙어있어야 하니까)
        rb.isKinematic = true; 
        
        isEquipped = true;
    }

    // 2. 옷을 다시 잡았을 때 -> 옷 벗기
    private void OnGrab(SelectEnterEventArgs args)
    {
        // 입고 있는 상태에서 잡았다면 -> 벗는 동작으로 간주
        if (isEquipped)
        {
            // 2-1. 부모 해제 (카메라에서 분리)
            transform.SetParent(null);

            // 2-2. 물리 다시 켜기
            rb.isKinematic = false;

            isEquipped = false;
            
            // 이후는 자동으로 XRI가 손에 붙잡은 상태로 처리함
        }
    }
}