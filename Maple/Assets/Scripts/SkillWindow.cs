using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillWindow : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    // ��ų UI ��ũ��Ʈ
    public GameObject Skill_UI = null;
    [SerializeField]
    private bool isSkill_Open = false; // �����ִ��� üũ�� BOOL ����


    void Start()
    {
    }


    public RectTransform window; //Drag Move Window
    private Vector2 downPosition;

    public void OnPointerDown(PointerEventData data) // ���콺 Ŭ���� ������
    {
        downPosition = data.position; // downPosition�� ��������.
    }

    public void OnDrag(PointerEventData data) // �巡�� ���̸�
    {
        Vector2 offset = data.position - downPosition;
        downPosition = data.position;

        window.anchoredPosition += offset; // ��ǥ�� ��������
    }

    void toggleSkill_Window() // ��ų â�� �״� ���� �ϴ� �Լ�
    {
        if (Input.GetKeyDown(KeyCode.K)) // K Ű�� ������ 
        {
            isSkill_Open = !isSkill_Open; // ���� ������ �ݴ� BOOL ���� �־��ְ�
        }

        Skill_UI.SetActive(isSkill_Open); // �� ���� �°� Ȱ��ȭ Ȥ�� ��Ȱ��ȭ ������.
    }

    void Update()
    {
        toggleSkill_Window();
    }
}
