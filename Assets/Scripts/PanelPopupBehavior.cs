using UnityEngine;

public class PanelPopupBehavior : MonoBehaviour
{
    [Header("설정")]
    public float distance = 1.0f; // 플레이어 눈앞 몇 미터에 띄울지 (보통 0.5 ~ 1.5 추천)
    public float heightOffset = 0.1f;
    // 이 함수를 호출하면 패널이 플레이어 앞으로 이동하고 켜집니다.
    public void ShowPanel()
    {
        // 1. 메인 카메라(플레이어의 눈) 찾기
        Transform cameraTransform = Camera.main.transform;

        // 2. 위치 계산: 카메라 위치 + (카메라가 보는 방향 * 거리)
        Vector3 targetPosition = cameraTransform.position + (cameraTransform.forward * distance);
        
        // 높이 살짝 보정 (너무 정직하게 눈앞이면 부담스러우니 살짝 아래로)
        targetPosition.y += heightOffset;

        // 3. 위치 적용
        transform.position = targetPosition;

        // 4. 회전 계산: 패널이 항상 플레이어를 정면으로 바라보게 함
        // (UI는 보통 뒷면이 안 보이고 앞면이 보여야 하므로, 카메라가 보는 방향과 같은 방향을 보게 하거나,
        //  플레이어 위치에서 멀어지는 방향을 보게 해야 글자가 뒤집히지 않습니다.)
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);

        // 5. 패널 켜기
        gameObject.SetActive(true);
    }
}