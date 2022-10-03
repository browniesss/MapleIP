using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // 카메라 스크립트 
    static public CameraFollow instance; // instance의 값을 공유

    public GameObject target; // 카메라가 따라갈 대상
    public float moveSpeed; // 카메라가 따라갈 속도
    private Vector3 targetPosition; // 대상의 현재 위치

    // 박스 컬라이더 영역의 최소 최대값
    public BoxCollider2D bound;
    private Vector3 minBound;
    private Vector3 maxBound;

    // 카메라의 반넓이와 반높이의 값 변수
    private float halfWidth;
    private float halfHeight;

    // 반 높이를 구하기 위해 필요한 카메라 변수
    private Camera theCamera;

    private void Awake()
    {

    }

    void Start()
    {
        theCamera = GetComponent<Camera>(); // 카매라 스크립트를 받아옴.
        minBound = bound.bounds.min; // 받아온 영역의 값들을 이용해 화면에 표시할 영역을 지정.
        maxBound = bound.bounds.max;

        halfHeight = theCamera.orthographicSize; 
        halfWidth = halfHeight * Screen.width / Screen.height;
    }

    void Update()
    {
        // 대상이 있는지 체크
        if (target.gameObject != null)
        {
            targetPosition.Set(target.transform.position.x, target.transform.position.y, this.transform.position.z);

            // Lerp를 이용해 부드럽게 이동.
            this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // clamp를 이용해 최솟값, 최댓값 제한
            float clampedX = Mathf.Clamp(this.transform.position.x, minBound.x + halfWidth, maxBound.x - halfWidth);
            float clampedY = Mathf.Clamp(this.transform.position.y, minBound.y + halfHeight, maxBound.y - halfHeight);

            this.transform.position = new Vector3(clampedX, clampedY, this.transform.position.z);
        }
    }

    public void SetBound(BoxCollider2D newBound) // 영역 지정 함수.
    {
        bound = newBound;
        minBound = bound.bounds.min;
        maxBound = bound.bounds.max;
    }
}
