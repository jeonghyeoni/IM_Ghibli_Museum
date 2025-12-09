using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Unity 6 / XRI 3.x
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections; 

public class PonyoDining : MonoBehaviour
{
    [Header("포뇨 연결")]
    public Animator ponyoAnimator;
    public GameObject musicBoxPiece; // 보상 (오르골 조각)

    [Header("성공 패널 설정 (햄 라멘)")]
    public GameObject successPanel;      // ✨ 추가: 오르골 조각 발견 패널
    public float successPanelDelay = 1.5f; // ✨ 추가: 조각 등장 후 패널이 뜰 때까지 대기 시간

    [Header("힌트 패널 설정 (그냥 라멘)")]
    public PanelPopupBehavior hintPanel; // 힌트 패널 스크립트
    public float hintDelay = 2.0f;       // 거절 후 패널이 뜰 때까지 대기 시간

    [Header("오디오 (선택)")]
    public AudioSource audioSource;
    public AudioClip happySound;  
    public AudioClip refuseSound; 

    private XRSocketInteractor socketInteractor;

    void Awake()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
    }

    void OnEnable()
    {
        socketInteractor.selectEntered.AddListener(CheckRamen);
    }

    void OnDisable()
    {
        socketInteractor.selectEntered.RemoveListener(CheckRamen);
    }

    private void CheckRamen(SelectEnterEventArgs args)
    {
        GameObject ramenObj = args.interactableObject.transform.gameObject;
        RamenStatus ramenStatus = ramenObj.GetComponent<RamenStatus>();

        if (ramenStatus == null) return;

        // 1. 햄이 있는지 검사
        if (ramenStatus.HasHam())
        {
            // [성공] 
            Debug.Log("햄 라면이다! 맛있다!");
            ponyoAnimator.SetTrigger("Happy");

            // 오르골 조각 등장
            if (musicBoxPiece != null)
            {
                musicBoxPiece.SetActive(true);
            }

            // 효과음 재생
            if (audioSource && happySound) audioSource.PlayOneShot(happySound);

            // ✨ 추가: 딜레이 후 성공 패널 띄우기
            StartCoroutine(ShowSuccessPanelRoutine());
        }
        else
        {
            // [실패] 햄이 없음
            Debug.Log("햄이 없잖아! 싫어!");

            // 거절 애니메이션 및 소리
            ponyoAnimator.SetTrigger("Refuse");
            if (audioSource && refuseSound) audioSource.PlayOneShot(refuseSound);

            // 딜레이 후 힌트 패널 띄우기
            StartCoroutine(ShowHintRoutine());

            // 라면 뱉어내기
            StartCoroutine(EjectRamen(args.interactableObject));
        }
    }

    // ✨ 추가: 성공 패널 띄우는 코루틴
    IEnumerator ShowSuccessPanelRoutine()
    {
        // 설정한 시간(successPanelDelay)만큼 대기
        yield return new WaitForSeconds(successPanelDelay);

        // 패널 활성화
        if (successPanel != null)
        {
            successPanel.SetActive(true);
        }
    }

    // 힌트 패널 띄우는 코루틴
    IEnumerator ShowHintRoutine()
    {
        yield return new WaitForSeconds(hintDelay);

        if (hintPanel != null)
        {
            hintPanel.ShowPanel();
        }
    }

    private IEnumerator EjectRamen(IXRSelectInteractable ramen)
    {
        yield return new WaitForSeconds(3.0f); 

        socketInteractor.interactionManager.SelectExit(socketInteractor, ramen);
        socketInteractor.enabled = false;

        Transform ramenTransform = ramen.transform;
        Rigidbody ramenRb = ramenTransform.GetComponent<Rigidbody>();

        if (ramenRb != null)
        {
            // Unity 6: linearVelocity / Unity 2022 이하: velocity
            ramenRb.linearVelocity = Vector3.zero; 
            ramenRb.angularVelocity = Vector3.zero;

            Vector3 ejectForce = (socketInteractor.transform.forward + Vector3.up).normalized * 1.5f;
            ramenRb.AddForce(ejectForce, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(2.0f);
        socketInteractor.enabled = true;
    }
}