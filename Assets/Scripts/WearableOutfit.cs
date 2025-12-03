using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WearableOutfit : MonoBehaviour
{
    [Header("착용 설정")]
    public Transform playerCamera; 
    public Vector3 wearOffset = new Vector3(0, -1.3f, -0.2f); 
    
    [Header("애니메이션 설정")]
    public Animator chihiroAnimator;
    
    // [오류 해결] 이 변수가 빠져 있었습니다!
    public float minMoveSpeed = 0.05f; // 움직임 감지 최소 속도
    
    public float animationSpeedMultiplier = 2.0f; // 애니메이션 속도 배율
    public float smoothing = 10.0f; // 부드러운 전환 값

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private bool isEquipped = false;

    private Vector3 lastPosition; // 이전 위치 저장용
    private float currentVertical = 0f; // 현재 애니메이션 값

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        
        if (chihiroAnimator == null) chihiroAnimator = GetComponentInChildren<Animator>();
        
        // 애니메이션 끊김 방지 강제 설정
        if (chihiroAnimator != null)
        {
            chihiroAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            chihiroAnimator.applyRootMotion = false;
        }
    }

    void OnEnable()
    {
        grabInteractable.activated.AddListener(OnActivate);
        grabInteractable.selectEntered.AddListener(OnGrab);
    }

    void OnDisable()
    {
        grabInteractable.activated.RemoveListener(OnActivate);
        grabInteractable.selectEntered.RemoveListener(OnGrab);
    }

    void LateUpdate()
    {
        if (isEquipped && playerCamera != null)
        {
            // 1. 위치 동기화
            Vector3 targetEuler = playerCamera.eulerAngles;
            Quaternion targetRotation = Quaternion.Euler(0, targetEuler.y, 0);
            Vector3 targetPosition = playerCamera.position + (targetRotation * wearOffset);

            transform.position = targetPosition;
            transform.rotation = targetRotation;

            // 2. 이동 속도 및 방향 계산 (옆걸음질 포함 버전)
            
            // A. 높이를 제외한 평면 이동 거리 계산
            Vector3 currentPosFlat = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 lastPosFlat = new Vector3(lastPosition.x, 0, lastPosition.z);
            
            Vector3 displacement = currentPosFlat - lastPosFlat;
            float rawSpeed = displacement.magnitude / Time.deltaTime;

            // B. 로컬 기준 이동 방향 계산
            Vector3 localMove = transform.InverseTransformDirection(displacement);
            
            // C. 방향 결정 로직
            // 뒤로 가는 경우(음수)가 아니면 무조건 1(앞)로 설정해 옆걸음질도 걷게 함
            float directionSign = 1f;
            if (localMove.z < -0.05f * Time.deltaTime) 
            {
                directionSign = -1f;
            }

            // D. 최종 입력값 계산
            float targetInput = 0f;

            // [수정] 이제 minMoveSpeed 변수가 선언되었으므로 오류가 나지 않습니다.
            if (rawSpeed > minMoveSpeed)
            {
                // 방향 * 속도 * 배율
                targetInput = directionSign * rawSpeed * animationSpeedMultiplier;
                targetInput = Mathf.Clamp(targetInput, -1f, 1f);
            }
            
            // E. 부드러운 적용 (Lerp)
            currentVertical = Mathf.Lerp(currentVertical, targetInput, Time.deltaTime * smoothing);

            if (chihiroAnimator != null)
            {
                chihiroAnimator.SetFloat("Vertical", currentVertical);
            }

            lastPosition = targetPosition;
        }
    }

    private void OnActivate(ActivateEventArgs args)
    {
        if (isEquipped) return; 

        if (args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor selectInteractor)
        {
            grabInteractable.interactionManager.SelectExit(selectInteractor, grabInteractable);
        }

        // 부모 해제 (시뮬레이터 진동 방지)
        transform.SetParent(null);
        
        rb.isKinematic = true; 
        isEquipped = true;
        
        // 애니메이터 상태 전환
        if (chihiroAnimator != null) chihiroAnimator.SetBool("IsEquipped", true);

        // 위치 초기화
        lastPosition = transform.position;
        currentVertical = 0f;
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (isEquipped)
        {
            if (chihiroAnimator != null) 
            {
                chihiroAnimator.SetBool("IsEquipped", false);
                chihiroAnimator.SetFloat("Vertical", 0);
            }

            transform.SetParent(null);
            rb.isKinematic = false;
            isEquipped = false;
        }
    }
}