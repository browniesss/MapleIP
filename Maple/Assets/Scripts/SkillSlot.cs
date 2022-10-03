using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    // 스킬 슬롯 스크립트
    public Skill skill = null;

    public Image skill_Image = null; // 스킬 이미지
    public Image skill_Info = null; // 스킬 정보 이미지
    public Image coolTime = null; // 스킬 쿨타임 이미지

    public bool isSkill_Window = false; // 스킬 창의 스킬인지

    public QuickSlot quickSlot = null; // 퀵슬롯

    private void Awake()
    {
        quickSlot = FindObjectOfType<QuickSlot>();
    }

    public void OnPointerEnter(PointerEventData eventData) // 마우스 포인터가 올라와 있다면 
    {
        if (skill != null) // 슬롯에 아이템이 있다면
        {
            skill_Image.sprite = skill.skill_Image; 
            skill_Info.sprite = skill.skill_Info;
            skill_Info.color = new Color(1, 1, 1, 1);
        }
    }

    public void OnPointerExit(PointerEventData eventData) // 마우스 포인터가 나갔다면
    {
        skill_Info.color = new Color(1, 1, 1, 0);
    }

    // 마우스 드래그가 시작 됐을 때 발생하는 이벤트
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (skill != null) // 스킬이 있다면 
        {
            DragSkill.instance.dragSlot = this; // DragSkill = 현재 드래그 하는 스킬 슬롯을 저장하는 슬롯의 dragSlot 을 해당 슬롯으로 지정해줌.
            DragSkill.instance.DragSetImage(skill_Image); // 이미지를 변경 후
            DragSkill.instance.transform.position = eventData.position; // 마우스 위치로 좌표 지정
        }
    }

    // 마우스 드래그 중일 때 계속 발생하는 이벤트
    public void OnDrag(PointerEventData eventData) 
    {
        if (skill != null)
            DragSkill.instance.transform.position = eventData.position; // 마우스 위치로 좌표를 옮겨줌.
    }

    // 마우스 드래그가 끝났을 때 발생하는 이벤트
    public void OnEndDrag(PointerEventData eventData)
    {
        if (skill != null)
        {
            if (!eventData.pointerCurrentRaycast.gameObject) // 인벤토리 바깥에서 놓았을 경우.
            {
                DragSkill.instance.SetColor(0); // 투명도를 0으로 해줘서 화면에서 보이지 않게 해준 후 
                DragSkill.instance.dragSlot = null; // dragSlot 을 null로 비워준 후 
                return; // return
            }

            if (DragSkill.instance.dragSlot == this) // 원래 슬롯에 놓았을 경우.
            {
                DragSkill.instance.SetColor(0); // dragSlot의 투명도를 0으로 해줘서 화면에서 보이지 않게 해준 후 
                DragSkill.instance.dragSlot = null; // dragSlot 을 null 로 비워준 후 
                skill_Image.color = new Color(255, 255, 255, 1); // 다시 스킬 이미지의 색, 투명도를 원상태로 복구해준 후 
                return; // return 
            }

            if (DragSkill.instance.dragSlot != null)  // 드래그가 끝났는데 dragSlot이 있다면 - 드래그가 정상적으로 이루어지지 않았다면
            {
                DragSkill.instance.SetColor(0); // 투명도를 0으로 해줘서 화면에서 보이지 않게 해준 후 
                DragSkill.instance.dragSlot = null; // dragSlot을 null 로 비워줌.
            }
        }
    }

    // 해당 슬롯에 무언가가 마우스 드롭 됐을 때 발생하는 이벤트
    public void OnDrop(PointerEventData eventData)
    {
        if (isSkill_Window) // 해당 슬롯이 스킬 창의 슬롯이라면 return
        {
            return;
        }

        if (DragSkill.instance.dragSlot != null) // 내려놓을 인벤토리 슬롯이 있다면 
        {
            SkillSlot tempSlot = DragSkill.instance.dragSlot; // tempSlot 이라는 스킬슬롯 변수를 선언해서 DragSkill 의 dragSlot 을 넣어줌.

            if (tempSlot == this) // 원래 슬롯에 놓았을 경우.
            {
                DragSkill.instance.SetColor(0); // 투명도를 0으로 바꿔줘서 화면에서 보이지 않게 함.
                DragSkill.instance.dragSlot = null; // dragSlot 을 null로 비워줌 
                skill_Image.color = new Color(255, 255, 255, 1); // 투명도랑 색을 원상태를 돌려줌.
                return; // 그리고 return 
            }
            else // 그게 아니라면
            {
                ChangeSlot(); // 슬롯을 변경해 줌. 
                DragSkill.instance.MyImage.color = new Color(1, 1, 1, 0); // 이미지 투명도를 0으로 변경.
            }
        }

        quickSlot.Update_QuickSlot(); // 퀵슬롯 정보를 업데이트 해줌.
    }

    private void ChangeSlot() // 슬롯 변경 함수.
    {
        Skill _tempSkill = skill;

        if (DragSkill.instance.dragSlot.isSkill_Window) // DragSkill 슬롯이 스킬 창의 슬롯이라면
            CheckQuickSlot(); // 퀵슬롯을 체크함.

        AddSkill(DragSkill.instance.dragSlot.skill); // DragSkill의 스킬을 추가해줌

        if (!DragSkill.instance.dragSlot.isSkill_Window) // 만약 스킬창에서 드래그 한게 아니라면
        {
            if (_tempSkill != null) // 스킬이 없지 않다면
            {
                DragSkill.instance.dragSlot.AddSkill(_tempSkill);  // 현재 DragSkill의 슬롯에 바꿀 스킬을 넣어줌.
            }
            else // 스킬이 없었다면
                DragSkill.instance.dragSlot.ClearSlot(); // 해당 슬롯을 비워줌.

            float tempTime = startTime;
            bool tempCool = coolTime_Start;

            startTime = DragSkill.instance.dragSlot.startTime; // 드래그한 스킬의 시작한 시간을 넣어주고        
            coolTime_Start = DragSkill.instance.dragSlot.coolTime_Start; // 드래그한 스킬의 쿨타임 여부를 넣어줌.

            DragSkill.instance.dragSlot.startTime = tempTime; // 현재 스킬의 시작한 시간을 드래그 스킬에 넣어주고
            DragSkill.instance.dragSlot.coolTime_Start = tempCool; // 현재 스킬의 쿨타임 여부를 드래그 스킬에 넣어줌.
            DragSkill.instance.dragSlot.coolTime.fillAmount = 0; 
        }

        DragSkill.instance.dragSlot = null; // 그리고 드래그 스킬의 드래그 슬롯을 비워줌
    }

    private void CheckQuickSlot() // 퀵슬롯 체크 함수
    {
        for (int i = 0; i < quickSlot.Skill_slots.Length; i++)
        {
            if (quickSlot.Skill_slots[i].skill == DragSkill.instance.dragSlot.skill) // 같은 스킬을 가진 슬롯이 있다면
            {
                quickSlot.Skill_slots[i].ClearSlot(); // 그 슬롯을 초기화 해준 후 
                return; // 리턴
            }
        }
    }

    // 스킬 이미지의 투명도 조절
    private void SetColor(float _alpha)
    {
        Color color = skill_Image.color;
        color.a = _alpha;
        skill_Image.color = color;
    }

    // 인벤토리에 새로운 아이템 슬롯 추가
    public void AddSkill(Skill _skill)
    {
        skill = _skill;
        skill_Image.sprite = _skill.skill_Image;

        SetColor(1);
    }

    private void ClearSlot() // 슬롯을 비워주는 함수
    {
        skill = null;
        skill_Image.sprite = null;
        SetColor(0);
    }

    public bool coolTime_Start = false;
    public float startTime = 0f;

    public void Skill_CoolTime()
    {
        if (coolTime_Start)
        {
            coolTime.fillAmount = (skill.CoolTime - ((float)Time.time - startTime)) / skill.CoolTime;
            if(coolTime.fillAmount == 0) // 쿨타임이 다됐으면
            {
                skill.usable_Skill = true;
            }
        }
    }

    void Update()
    {
        Skill_CoolTime();
    }
}
