using UnityEngine;
using Unity.Cinemachine; // [중요] 네임스페이스가 변경되었습니다.

public class CameraSetting : MonoBehaviour
{
    void Start()
    {
        // 추적할 대상(Player) 찾기
        GameObject targetObject = GameObject.FindGameObjectWithTag("Player");

        if (targetObject != null)
        {
            SetCamTarget(targetObject.transform);
        }
    }

    public void SetCamTarget(Transform target)
    {
        // 내 오브젝트에 붙은 CinemachineCamera 컴포넌트 가져오기
        CinemachineCamera cam = GetComponent<CinemachineCamera>();

        if (cam != null)
        {
            // 타겟 연결
            cam.Follow = target; // 카메라가 따라다닐 대상

            Debug.Log($"카메라 타겟이 {target.name}으로 설정됨");
        }
    }
}