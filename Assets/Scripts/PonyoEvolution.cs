using UnityEngine;
using System.Collections;

public class PonyoEvolution : MonoBehaviour
{
    [Header("모델 연결")]
    public GameObject eatingPonyoModel;
    public GameObject humanPonyoModel;

    [Header("UI 설정")]
    public GameObject messagePanel;   // ✨ 추가: 띄울 안내 패널
    public float panelDelay = 1.5f;   // ✨ 추가: 변신 완료 후 패널 뜰 때까지 대기 시간

    [Header("등장 효과 설정")]
    public float scaleDuration = 0.5f;
    public AnimationCurve popCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("오디오 및 파티클")]
    public AudioSource audioSource;
    public AudioClip eatingSound;
    public AudioClip transformSound;

    [Range(0f, 1f)]
    public float popVolume = 0.5f;
    public ParticleSystem smokeEffect;

    private Vector3 finalScale;

    void Awake()
    {
        if (humanPonyoModel != null)
        {
            finalScale = humanPonyoModel.transform.localScale;
        }

        // 시작할 때 패널은 꺼둠 (안전을 위해)
        if (messagePanel != null)
            messagePanel.SetActive(false);
    }

    public void StartEvolution()
    {
        StartCoroutine(EvolutionRoutine());
    }

    IEnumerator EvolutionRoutine()
    {
        // 초기 상태 설정
        eatingPonyoModel.SetActive(true);
        humanPonyoModel.SetActive(false);
        
        humanPonyoModel.transform.localScale = Vector3.zero;

        if (smokeEffect != null)
        {
            smokeEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            smokeEffect.gameObject.SetActive(false);
        }

        // 1. 챱챱챱 소리 재생
        if (audioSource != null && eatingSound != null)
        {
            audioSource.PlayOneShot(eatingSound);
            yield return new WaitForSeconds(eatingSound.length);
        }
        else
        {
            yield return new WaitForSeconds(2.0f);
        }

        // 2. 모델 교체
        eatingPonyoModel.SetActive(false);
        humanPonyoModel.SetActive(true);

        // 3. 연기 효과
        if (smokeEffect != null)
        {
            smokeEffect.gameObject.SetActive(true);
            smokeEffect.Play();
        }

        // 4. 등장 소리
        if (audioSource != null && transformSound != null)
        {
            audioSource.PlayOneShot(transformSound, popVolume);
        }

        // 5. 크기 키우기 (변신 애니메이션)
        float timer = 0f;
        while (timer < scaleDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / scaleDuration;
            
            float curveValue = popCurve.Evaluate(percent);
            
            humanPonyoModel.transform.localScale = finalScale * curveValue;
            
            yield return null;
        }

        // 확실하게 원래 크기로 고정
        humanPonyoModel.transform.localScale = finalScale;

        // ✨ 추가: 변신 후 잠시 대기 (플레이어가 포뇨를 볼 시간)
        yield return new WaitForSeconds(panelDelay);

        // ✨ 추가: 패널 등장
        if (messagePanel != null)
        {
            messagePanel.SetActive(true);
        }
    }
}