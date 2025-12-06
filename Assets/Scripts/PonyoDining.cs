using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PonyoDining : MonoBehaviour
{
    [Header("포뇨 연결")]
    public Animator ponyoAnimator;
    public GameObject musicBoxPiece; // 보상 (오르골 조각)

    [Header("오디오 (선택)")]
    public AudioSource audioSource;
    public AudioClip happySound;  // "우와!"
    public AudioClip refuseSound; // "흥! 햄 줘!"

    private XRSocketInteractor socketInteractor;

    void Awake()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
    }

    void OnEnable()
    {
        // 소켓에 물건이 들어오면 CheckRamen 함수 실행
        socketInteractor.selectEntered.AddListener(CheckRamen);
    }

    void OnDisable()
    {
        socketInteractor.selectEntered.RemoveListener(CheckRamen);
    }

    // 소켓에 들어온 물건(args.interactableObject)을 검사
    private void CheckRamen(SelectEnterEventArgs args)
    {
        // 1. 들어온 물건에서 RamenStatus 스크립트 찾기
        GameObject ramenObj = args.interactableObject.transform.gameObject;
        RamenStatus ramenStatus = ramenObj.GetComponent<RamenStatus>();

        // 라면이 아니면 무시
        if (ramenStatus == null) return;

        // 2. 햄이 있는지 검사
        if (ramenStatus.HasHam())
        {
            // [성공] 햄이 있음!
            Debug.Log("햄 라면이다! 맛있다!");

            // 기뻐하는 애니메이션
            ponyoAnimator.SetTrigger("Happy");

            // 오르골 조각 등장
            if (musicBoxPiece != null)
            {
                musicBoxPiece.SetActive(true);
                SoundManager.Instance.PlayPieceSound();
            }
            // 소리 재생
            if (audioSource && happySound) audioSource.PlayOneShot(happySound);

            // (옵션) 라면을 더 이상 못 꺼내게 하려면 소켓의 Interaction Layer를 잠가버릴 수도 있음
        }
        else
        {
            // [실패] 햄이 없음...
            Debug.Log("햄이 없잖아! 싫어!");

            // 도리도리 애니메이션
            ponyoAnimator.SetTrigger("Refuse");

            // 소리 재생
            if (audioSource && refuseSound) audioSource.PlayOneShot(refuseSound);

            // [중요] 거부했으니 소켓에서 라면을 퉤! 하고 뱉어내게 만들기
            // (안 그러면 햄 없는 라면이 계속 꽂혀 있음)
            StartCoroutine(EjectRamen(args.interactableObject));
        }
    }

    // 라면 뱉어내기 코루틴
    private System.Collections.IEnumerator EjectRamen(IXRSelectInteractable ramen)
    {
        // 1. 애니메이션 보여줄 시간 (도리도리 하는 동안 대기)
        yield return new WaitForSeconds(3.0f); // 0.5초는 너무 짧을 수 있어서 늘림

        // 2. 강제로 잡기 해제 (뱉기)
        socketInteractor.interactionManager.SelectExit(socketInteractor, ramen);

        // [핵심 1] 소켓을 잠시 끕니다. (안 그러면 바로 다시 잡아버림)
        socketInteractor.enabled = false;

        // [핵심 2] 라면을 플레이어 쪽으로 툭 밀어냅니다.
        // IXRSelectInteractable 인터페이스에서 실제 게임 오브젝트를 가져옴
        Transform ramenTransform = ramen.transform;
        Rigidbody ramenRb = ramenTransform.GetComponent<Rigidbody>();

        if (ramenRb != null)
        {
            // 물리 힘을 초기화 (혹시 멈춰있을까봐)
            ramenRb.linearVelocity = Vector3.zero; // Unity 6에서는 velocity 대신 linearVelocity 권장
            ramenRb.angularVelocity = Vector3.zero;

            // 소켓의 앞쪽(Forward)이나 위쪽(Up)으로 튕겨나가게 힘을 줌
            // 힘의 방향과 세기(2.0f)는 상황에 맞춰 조절하세요.
            Vector3 ejectForce = (socketInteractor.transform.forward + Vector3.up).normalized * 1.5f;
            ramenRb.AddForce(ejectForce, ForceMode.Impulse);
        }

        // 3. 소켓이 라면을 다시 잡지 않도록 2초 정도 쿨타임을 가짐
        yield return new WaitForSeconds(2.0f);

        // 4. 소켓 다시 켜기 (이제 다시 넣을 수 있음)
        socketInteractor.enabled = true;
    }
}