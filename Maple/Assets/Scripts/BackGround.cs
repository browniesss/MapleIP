using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    // 맵 배경 스크립트
    public GameObject target; // 카메라가 따라갈 대상
    public float moveSpeed; // 카메라가 따라갈 속도
    private Vector3 targetPosition; // 대상의 현재 위치

    void Update()
    {
        // 대상이 있는지 체크
        if (target.gameObject != null)
        {
            targetPosition.Set(0, target.transform.position.y + 1f, 0);

            // 위지를 Lerp를 이용해 부드럽게 이동시켜줌.
            this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }
}
