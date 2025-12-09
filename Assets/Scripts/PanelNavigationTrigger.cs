using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PanelNavigationTrigger : MonoBehaviour
{
    public enum NavType { Previous, Next }
    
    [Header("이 버튼의 역할 설정")]
    public NavType navigationType;

    private void Start()
    {
        // 내 부모(혹은 부모의 부모) 중에 관리자 스크립트를 찾음
        var controller = GetComponentInParent<PanelGroupController>();
        var btn = GetComponent<Button>();

        if (controller != null)
        {
            // 버튼 클릭 시 관리자의 함수를 실행하도록 자동 연결
            btn.onClick.AddListener(() => 
            {
                if (navigationType == NavType.Next)
                    controller.GoNext();
                else
                    controller.GoPrev();
            });
        }
        else
        {
            Debug.LogWarning("부모 오브젝트에서 PanelGroupController를 찾을 수 없습니다!");
        }
    }
}