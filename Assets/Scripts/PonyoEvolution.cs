using UnityEngine;
using System.Collections;

public class PonyoEvolution : MonoBehaviour
{
    [Header("모델 연결")]
    public GameObject eatingPonyoModel;
    public GameObject humanPonyoModel;

    [Header("등장 효과 설정")]
    public float scaleDuration = 0.5f; // 커지는 데 걸리는 시간
    public AnimationCurve popCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 띠용~ 하는 느낌의 그래프

    [Header("오디오 및 파티클")]
    public AudioSource audioSource;
    public AudioClip eatingSound;
    public AudioClip transformSound;
    public ParticleSystem smokeEffect;

    private Vector3 finalScale; // 원래 포뇨의 크기 저장용

    void Awake()
    {
        // 게임 시작 시, 에디터에 설정해둔 인간 포뇨의 크기를 기억해둡니다.
        if (humanPonyoModel != null)
        {
            finalScale = humanPonyoModel.transform.localScale;
        }
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
        
        // 인간 포뇨 크기를 미리 0으로 만들어 둡니다.
        humanPonyoModel.transform.localScale = Vector3.zero;

        // 파티클 초기화
        if (smokeEffect != null)
        {
            smokeEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            smokeEffect.gameObject.SetActive(false);
        }

        // 1. 챱챱챱 소리 재생 및 대기
        if (audioSource != null && eatingSound != null)
        {
            audioSource.PlayOneShot(eatingSound);
            yield return new WaitForSeconds(eatingSound.length);
        }
        else
        {
            yield return new WaitForSeconds(2.0f);
        }

        // 2. 모델 교체 (변신 시작!)
        eatingPonyoModel.SetActive(false); // 먹는 포뇨 퇴장
        humanPonyoModel.SetActive(true);   // 인간 포뇨 등장 (Scale 0 상태)

        // 3. 연기 효과 펑!
        if (smokeEffect != null)
        {
            smokeEffect.gameObject.SetActive(true);
            smokeEffect.Play();
        }

        // 4. 등장 소리 재생
        if (audioSource != null && transformSound != null)
        {
            audioSource.PlayOneShot(transformSound);
        }

        // 5. [추가됨] 크기 키우기 (Scale 0 -> 원래 크기)
        float timer = 0f;
        while (timer < scaleDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / scaleDuration;
            
            // 커브를 이용해 띠용~ 하는 느낌 적용
            float curveValue = popCurve.Evaluate(percent);
            
            humanPonyoModel.transform.localScale = finalScale * curveValue;
            
            yield return null;
        }

        // 확실하게 원래 크기로 고정
        humanPonyoModel.transform.localScale = finalScale;
    }
}