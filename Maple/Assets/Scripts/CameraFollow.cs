using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // ī�޶� ��ũ��Ʈ 
    static public CameraFollow instance; // instance�� ���� ����

    public GameObject target; // ī�޶� ���� ���
    public float moveSpeed; // ī�޶� ���� �ӵ�
    private Vector3 targetPosition; // ����� ���� ��ġ

    // �ڽ� �ö��̴� ������ �ּ� �ִ밪
    public BoxCollider2D bound;
    private Vector3 minBound;
    private Vector3 maxBound;

    // ī�޶��� �ݳ��̿� �ݳ����� �� ����
    private float halfWidth;
    private float halfHeight;

    // �� ���̸� ���ϱ� ���� �ʿ��� ī�޶� ����
    private Camera theCamera;

    private void Awake()
    {

    }

    void Start()
    {
        theCamera = GetComponent<Camera>(); // ī�Ŷ� ��ũ��Ʈ�� �޾ƿ�.
        minBound = bound.bounds.min; // �޾ƿ� ������ ������ �̿��� ȭ�鿡 ǥ���� ������ ����.
        maxBound = bound.bounds.max;

        halfHeight = theCamera.orthographicSize; 
        halfWidth = halfHeight * Screen.width / Screen.height;
    }

    void Update()
    {
        // ����� �ִ��� üũ
        if (target.gameObject != null)
        {
            targetPosition.Set(target.transform.position.x, target.transform.position.y, this.transform.position.z);

            // Lerp�� �̿��� �ε巴�� �̵�.
            this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // clamp�� �̿��� �ּڰ�, �ִ� ����
            float clampedX = Mathf.Clamp(this.transform.position.x, minBound.x + halfWidth, maxBound.x - halfWidth);
            float clampedY = Mathf.Clamp(this.transform.position.y, minBound.y + halfHeight, maxBound.y - halfHeight);

            this.transform.position = new Vector3(clampedX, clampedY, this.transform.position.z);
        }
    }

    public void SetBound(BoxCollider2D newBound) // ���� ���� �Լ�.
    {
        bound = newBound;
        minBound = bound.bounds.min;
        maxBound = bound.bounds.max;
    }
}
