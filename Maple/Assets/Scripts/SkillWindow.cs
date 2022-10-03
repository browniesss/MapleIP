using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillWindow : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    // 스킬 UI 스크립트
    public GameObject Skill_UI = null;
    [SerializeField]
    private bool isSkill_Open = false; // 열려있는지 체크할 BOOL 변수


    void Start()
    {
    }


    public RectTransform window; //Drag Move Window
    private Vector2 downPosition;

    public void OnPointerDown(PointerEventData data) // 마우스 클릭이 있으면
    {
        downPosition = data.position; // downPosition을 지정해줌.
    }

    public void OnDrag(PointerEventData data) // 드래그 중이면
    {
        Vector2 offset = data.position - downPosition;
        downPosition = data.position;

        window.anchoredPosition += offset; // 좌표를 변경해줌
    }

    void toggleSkill_Window() // 스킬 창을 켰다 껐다 하는 함수
    {
        if (Input.GetKeyDown(KeyCode.K)) // K 키를 누르면 
        {
            isSkill_Open = !isSkill_Open; // 현재 상태의 반대 BOOL 값을 넣어주고
        }

        Skill_UI.SetActive(isSkill_Open); // 그 값에 맞게 활성화 혹은 비활성화 시켜줌.
    }

    void Update()
    {
        toggleSkill_Window();
    }
}
