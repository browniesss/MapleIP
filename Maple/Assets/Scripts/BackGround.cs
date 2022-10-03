using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    // �� ��� ��ũ��Ʈ
    public GameObject target; // ī�޶� ���� ���
    public float moveSpeed; // ī�޶� ���� �ӵ�
    private Vector3 targetPosition; // ����� ���� ��ġ

    void Update()
    {
        // ����� �ִ��� üũ
        if (target.gameObject != null)
        {
            targetPosition.Set(0, target.transform.position.y + 1f, 0);

            // ������ Lerp�� �̿��� �ε巴�� �̵�������.
            this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }
}
