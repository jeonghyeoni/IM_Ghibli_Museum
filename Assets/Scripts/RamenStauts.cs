using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors; // Unity 6 XRI 필수

public class RamenStatus : MonoBehaviour
{
    [Header("자식 연결")]
    public XRSocketInteractor hamSocket; // 라면 위에 있는 햄 소켓

    // 외부에서 "햄 있어?" 라고 물어보는 함수
    public bool HasHam()
    {
        // 소켓에 무언가(햄)가 꽂혀 있다면(hasSelection) true 반환
        if (hamSocket != null && hamSocket.hasSelection)
        {
            return true;
        }
        return false;
    }
}