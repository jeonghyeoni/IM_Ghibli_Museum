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
    public float minMoveSpeed = 0.05f; 
    public float animationSpeedMultiplier = 2.0f; 
    public float smoothing = 10.0f; 

    // 내부 변수들
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private bool isEquipped = false;
    private Collider[] outfitColliders; 
    
    private Vector3 lastPosition; 
    private float currentVertical = 0f; 

    // [핵심] 1초 대기 타이머 변수
    private float stopTimer = 0f;
    private bool isSolid = false; // 현재 실체화 상태인지 확인

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        
        // 옷의 모든 콜라이더 저장
        outfitColliders = GetComponentsInChildren<Collider>();

        if (chihiroAnimator == null) chihiroAnimator = GetComponentInChildren<Animator>();
        
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
            // 1. 위치 동기화 (카메라 따라다니기)
            Vector3 targetEuler = playerCamera.eulerAngles;
            Quaternion targetRotation = Quaternion.Euler(0, targetEuler.y, 0);
            Vector3 targetPosition = playerCamera.position + (targetRotation * wearOffset);

            transform.position = targetPosition;
            transform.rotation = targetRotation;

            // 2. 이동 속도 계산
            Vector3 currentPosFlat = new Vector3(targetPosition.x, 0, targetPosition.z);
            Vector3 lastPosFlat = new Vector3(lastPosition.x, 0, lastPosition.z);
            Vector3 displacement = currentPosFlat - lastPosFlat;
            float rawSpeed = displacement.magnitude / Time.deltaTime;

            // 3. 애니메이션 처리
            HandleAnimation(displacement, rawSpeed);

            // 4. [핵심] 움직임에 따른 물리 상태 전환 (Trigger <-> Collider)
            HandlePhysicsState(rawSpeed);

            lastPosition = targetPosition;
        }
    }

    // ==================================================================================
    // [핵심 로직] 움직이면 유령(Trigger), 1초 멈추면 실체(Collider)
    // ==================================================================================
    private void HandlePhysicsState(float currentSpeed)
    {
        // 움직이는 중 (속도가 0.1 이상)
        if (currentSpeed > 0.1f)
        {
            stopTimer = 0f; // 타이머 초기화
            
            // 움직이자마자 즉시 유령 모드 (밀림 방지)
            if (isSolid) 
            {
                SetCollidersTrigger(true);
                isSolid = false;
            }
        }
        // 멈춰 있음
        else
        {
            stopTimer += Time.deltaTime;

            // 1초 이상 멈춰 있었다면?
            if (stopTimer >= 1.0f)
            {
                // 아직 유령 상태라면 -> 실체화 (잡을 수 있게 됨)
                if (!isSolid)
                {
                    SetCollidersTrigger(false);
                    isSolid = true;
                }
            }
        }
    }
    
    // Trigger 상태 일괄 변경 함수
    private void SetCollidersTrigger(bool isTriggerState)
    {
        if (outfitColliders == null) return;

        foreach (Collider col in outfitColliders)
        {
            if (col.isTrigger != isTriggerState)
            {
                col.isTrigger = isTriggerState;
            }
        }
    }
    // ==================================================================================

    private void OnActivate(ActivateEventArgs args)
    {
        if (isEquipped) return; 

        if (args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor selectInteractor)
        {
            grabInteractable.interactionManager.SelectExit(selectInteractor, grabInteractable);
        }

        transform.SetParent(null);
        rb.isKinematic = true; 
        isEquipped = true;
        
        // 입자마자 유령 모드로 시작
        SetCollidersTrigger(true);
        isSolid = false;
        stopTimer = 0f;

        if (chihiroAnimator != null) chihiroAnimator.SetBool("IsEquipped", true);

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

            // 벗었을 땐 무조건 실체화 (바닥에 굴러야 함)
            SetCollidersTrigger(false);
            isSolid = true;
        }
    }

    private void HandleAnimation(Vector3 displacement, float rawSpeed)
    {
        Vector3 localMove = transform.InverseTransformDirection(displacement);
        
        float directionSign = 1f;
        if (localMove.z < -0.05f * Time.deltaTime) directionSign = -1f;

        float targetInput = 0f;
        if (rawSpeed > minMoveSpeed)
        {
            targetInput = directionSign * rawSpeed * animationSpeedMultiplier;
            targetInput = Mathf.Clamp(targetInput, -1f, 1f);
        }
        
        currentVertical = Mathf.Lerp(currentVertical, targetInput, Time.deltaTime * smoothing);

        if (chihiroAnimator != null) chihiroAnimator.SetFloat("Vertical", currentVertical);
    }
}