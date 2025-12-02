using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // XRI 3.x 네임스페이스
using System.Collections;

public class PlantingSequence : MonoBehaviour
{
    [Header("연결할 오브젝트들")]
    public Transform acornObject;       // 움직일 도토리
    public MagicTreeGrow treeScript;    // 아까 만든 나무 성장 스크립트
    
    [Header("설정")]
    public float sinkDepth = 0.5f;      // 땅속으로 얼마나 깊게 들어갈지
    public float sinkDuration = 1.5f;   // 들어가는 데 걸리는 시간
    public float waitTime = 0.5f;       // 다 들어가고 나무 나올 때까지 대기 시간

    private XRGrabInteractable acornInteractable;
    private Collider acornCollider;

    void Awake()
    {
        // 도토리에서 필요한 컴포넌트 미리 찾아두기
        if (acornObject != null)
        {
            acornInteractable = acornObject.GetComponent<XRGrabInteractable>();
            acornCollider = acornObject.GetComponent<Collider>();
        }
    }

    public void StartPlantingSequence()
    {
        StartCoroutine(SequenceRoutine());
    }

    IEnumerator SequenceRoutine()
    {
        // 1. 도토리 조작 금지 (플레이어가 다시 못 잡게)
        if (acornInteractable != null) acornInteractable.enabled = false;
        
        // 2. 물리 충돌 끄기 (땅을 뚫고 들어가야 하므로)
        if (acornCollider != null) acornCollider.enabled = false;

        // 3. 도토리 땅 속으로 이동 (Sinking)
        Vector3 startPos = acornObject.position;
        Vector3 endPos = startPos + (Vector3.down * sinkDepth);
        
        float timer = 0f;
        while (timer < sinkDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / sinkDuration;
            
            // 부드럽게 이동
            acornObject.position = Vector3.Lerp(startPos, endPos, percent);
            yield return null;
        }

        // 4. 도토리 숨기기 (완전히 묻힘)
        acornObject.gameObject.SetActive(false);

        // 5. 잠시 대기 (긴장감 조성)
        yield return new WaitForSeconds(waitTime);

        // 6. 나무 성장 시작 (이전에 만든 스크립트 호출)
        if (treeScript != null)
        {
            treeScript.StartGrowing();
        }
    }
}