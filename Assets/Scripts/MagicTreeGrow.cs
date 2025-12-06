using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class MagicTreeGrow : MonoBehaviour
{
    public float growDuration = 2.0f; // 다 자라는데 걸리는 시간
    public AnimationCurve growCurve;  // 성장의 느낌 (그래프)
    public UnityEvent onGrowComplete;
    private Vector3 targetScale;

    void Awake()
    {
        // 원래 크기 저장
        targetScale = transform.localScale;
        // 시작할 땐 안 보이게 0으로
        transform.localScale = Vector3.zero;
    }

    public void StartGrowing()
    {
        StartCoroutine(GrowRoutine());
    }

    IEnumerator GrowRoutine()
    {
        float timer = 0f;

        while (timer < growDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / growDuration;
            
            // 커브를 이용해 꿀렁거리는 느낌 적용
            float curveValue = growCurve.Evaluate(percent); 
            
            transform.localScale = targetScale * curveValue;
            yield return null;
        }

        transform.localScale = targetScale; // 최종 크기 확정

        onGrowComplete?.Invoke();

        SoundManager.Instance.PlayPieceSound();
    }
}