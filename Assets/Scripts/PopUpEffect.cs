using UnityEngine;
using System.Collections;

public class PopUpEffect : MonoBehaviour
{
    public float popDuration = 0.5f; // 커지는 데 걸리는 시간 (빠르게!)
    public AnimationCurve popCurve;  // 튕기는 느낌 그래프

    private Vector3 finalScale;

    void Awake()
    {
        // 원래 크기 저장 후 0으로 숨김
        finalScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    // 오브젝트가 켜질 때(SetActive true) 자동으로 실행
    void OnEnable()
    {
        StartCoroutine(PopRoutine());
    }

    IEnumerator PopRoutine()
    {
        float timer = 0f;

        while (timer < popDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / popDuration;
            
            // 커브 값이 없으면 그냥 직선으로 커짐
            float curveValue = (popCurve != null && popCurve.length > 0) ? popCurve.Evaluate(percent) : percent;

            transform.localScale = finalScale * curveValue;
            yield return null;
        }

        transform.localScale = finalScale;
    }
}