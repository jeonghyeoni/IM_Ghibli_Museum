using UnityEngine;
using System.Collections;

public class DayNightManager : MonoBehaviour
{
    [Header("태양 설정")]
    public Light sunLight;

    [Header("별 파티클 설정")]
    public ParticleSystem starParticles; 

    [Header("시간 설정")]
    public float transitionDuration = 10.0f; 

    [Header("태양 각도")]
    public float dayAngle = 50f;   
    public float nightAngle = -30f; 

    void Start()
    {
        // [핵심] 게임 시작하자마자 별을 강제로 투명하게 만듭니다.
        SetStarAlpha(0f);
    }

    public void StartNightSequence()
    {
        StartCoroutine(TransitionToNight());
    }

    IEnumerator TransitionToNight()
    {
        float timer = 0f;

        // 파티클이 꺼져있다면 켭니다 (투명한 상태로 재생 시작)
        if (starParticles != null && !starParticles.isPlaying) 
        {
            starParticles.Play();
        }

        while (timer < transitionDuration)
        {
            float progress = timer / transitionDuration; // 0 ~ 1

            // 1. 태양 제어
            float currentAngle = Mathf.Lerp(dayAngle, nightAngle, progress);
            if (sunLight != null)
            {
                sunLight.transform.rotation = Quaternion.Euler(currentAngle, -30f, 0f);
                sunLight.intensity = Mathf.Lerp(1.3f, 0.0f, progress);
            }

            // 2. [핵심] 별 나타나게 하기 (Fade In)
            if (progress > 0.8f)
            {
                // 0.5~1.0 구간을 0~1로 변환
                float starProgress = (progress - 0.8f) * 5;
                
                // 알파값(투명도)을 0에서 1로 부드럽게 올림
                float newAlpha = Mathf.Lerp(0f, 1f, starProgress);
                SetStarAlpha(newAlpha);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // 최종 상태 고정
        if (sunLight != null)
        {
            sunLight.transform.rotation = Quaternion.Euler(nightAngle, -30f, 0f);
            sunLight.intensity = 0f;
        }
        SetStarAlpha(1f); // 별 완전 선명하게
    }

    // [보조 함수] 별의 투명도를 조절하는 함수
    private void SetStarAlpha(float alpha)
    {
        if (starParticles == null) return;

        var renderer = starParticles.GetComponent<ParticleSystemRenderer>();
        if (renderer != null && renderer.material != null)
        {
            // 재질의 색상을 가져와서 알파값만 바꿈
            Color color = renderer.material.color;
            color.a = alpha;
            renderer.material.color = color;
        }
    }
}