using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    // ��ų ���� ��ũ��Ʈ
    public Skill skill = null;

    public Image skill_Image = null; // ��ų �̹���
    public Image skill_Info = null; // ��ų ���� �̹���
    public Image coolTime = null; // ��ų ��Ÿ�� �̹���

    public bool isSkill_Window = false; // ��ų â�� ��ų����

    public QuickSlot quickSlot = null; // ������

    private void Awake()
    {
        quickSlot = FindObjectOfType<QuickSlot>();
    }

    public void OnPointerEnter(PointerEventData eventData) // ���콺 �����Ͱ� �ö�� �ִٸ� 
    {
        if (skill != null) // ���Կ� �������� �ִٸ�
        {
            skill_Image.sprite = skill.skill_Image; 
            skill_Info.sprite = skill.skill_Info;
            skill_Info.color = new Color(1, 1, 1, 1);
        }
    }

    public void OnPointerExit(PointerEventData eventData) // ���콺 �����Ͱ� �����ٸ�
    {
        skill_Info.color = new Color(1, 1, 1, 0);
    }

    // ���콺 �巡�װ� ���� ���� �� �߻��ϴ� �̺�Ʈ
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (skill != null) // ��ų�� �ִٸ� 
        {
            DragSkill.instance.dragSlot = this; // DragSkill = ���� �巡�� �ϴ� ��ų ������ �����ϴ� ������ dragSlot �� �ش� �������� ��������.
            DragSkill.instance.DragSetImage(skill_Image); // �̹����� ���� ��
            DragSkill.instance.transform.position = eventData.position; // ���콺 ��ġ�� ��ǥ ����
        }
    }

    // ���콺 �巡�� ���� �� ��� �߻��ϴ� �̺�Ʈ
    public void OnDrag(PointerEventData eventData) 
    {
        if (skill != null)
            DragSkill.instance.transform.position = eventData.position; // ���콺 ��ġ�� ��ǥ�� �Ű���.
    }

    // ���콺 �巡�װ� ������ �� �߻��ϴ� �̺�Ʈ
    public void OnEndDrag(PointerEventData eventData)
    {
        if (skill != null)
        {
            if (!eventData.pointerCurrentRaycast.gameObject) // �κ��丮 �ٱ����� ������ ���.
            {
                DragSkill.instance.SetColor(0); // ������ 0���� ���༭ ȭ�鿡�� ������ �ʰ� ���� �� 
                DragSkill.instance.dragSlot = null; // dragSlot �� null�� ����� �� 
                return; // return
            }

            if (DragSkill.instance.dragSlot == this) // ���� ���Կ� ������ ���.
            {
                DragSkill.instance.SetColor(0); // dragSlot�� ������ 0���� ���༭ ȭ�鿡�� ������ �ʰ� ���� �� 
                DragSkill.instance.dragSlot = null; // dragSlot �� null �� ����� �� 
                skill_Image.color = new Color(255, 255, 255, 1); // �ٽ� ��ų �̹����� ��, ������ �����·� �������� �� 
                return; // return 
            }

            if (DragSkill.instance.dragSlot != null)  // �巡�װ� �����µ� dragSlot�� �ִٸ� - �巡�װ� ���������� �̷������ �ʾҴٸ�
            {
                DragSkill.instance.SetColor(0); // ������ 0���� ���༭ ȭ�鿡�� ������ �ʰ� ���� �� 
                DragSkill.instance.dragSlot = null; // dragSlot�� null �� �����.
            }
        }
    }

    // �ش� ���Կ� ���𰡰� ���콺 ��� ���� �� �߻��ϴ� �̺�Ʈ
    public void OnDrop(PointerEventData eventData)
    {
        if (isSkill_Window) // �ش� ������ ��ų â�� �����̶�� return
        {
            return;
        }

        if (DragSkill.instance.dragSlot != null) // �������� �κ��丮 ������ �ִٸ� 
        {
            SkillSlot tempSlot = DragSkill.instance.dragSlot; // tempSlot �̶�� ��ų���� ������ �����ؼ� DragSkill �� dragSlot �� �־���.

            if (tempSlot == this) // ���� ���Կ� ������ ���.
            {
                DragSkill.instance.SetColor(0); // ������ 0���� �ٲ��༭ ȭ�鿡�� ������ �ʰ� ��.
                DragSkill.instance.dragSlot = null; // dragSlot �� null�� ����� 
                skill_Image.color = new Color(255, 255, 255, 1); // ������ ���� �����¸� ������.
                return; // �׸��� return 
            }
            else // �װ� �ƴ϶��
            {
                ChangeSlot(); // ������ ������ ��. 
                DragSkill.instance.MyImage.color = new Color(1, 1, 1, 0); // �̹��� ������ 0���� ����.
            }
        }

        quickSlot.Update_QuickSlot(); // ������ ������ ������Ʈ ����.
    }

    private void ChangeSlot() // ���� ���� �Լ�.
    {
        Skill _tempSkill = skill;

        if (DragSkill.instance.dragSlot.isSkill_Window) // DragSkill ������ ��ų â�� �����̶��
            CheckQuickSlot(); // �������� üũ��.

        AddSkill(DragSkill.instance.dragSlot.skill); // DragSkill�� ��ų�� �߰�����

        if (!DragSkill.instance.dragSlot.isSkill_Window) // ���� ��ųâ���� �巡�� �Ѱ� �ƴ϶��
        {
            if (_tempSkill != null) // ��ų�� ���� �ʴٸ�
            {
                DragSkill.instance.dragSlot.AddSkill(_tempSkill);  // ���� DragSkill�� ���Կ� �ٲ� ��ų�� �־���.
            }
            else // ��ų�� �����ٸ�
                DragSkill.instance.dragSlot.ClearSlot(); // �ش� ������ �����.

            float tempTime = startTime;
            bool tempCool = coolTime_Start;

            startTime = DragSkill.instance.dragSlot.startTime; // �巡���� ��ų�� ������ �ð��� �־��ְ�        
            coolTime_Start = DragSkill.instance.dragSlot.coolTime_Start; // �巡���� ��ų�� ��Ÿ�� ���θ� �־���.

            DragSkill.instance.dragSlot.startTime = tempTime; // ���� ��ų�� ������ �ð��� �巡�� ��ų�� �־��ְ�
            DragSkill.instance.dragSlot.coolTime_Start = tempCool; // ���� ��ų�� ��Ÿ�� ���θ� �巡�� ��ų�� �־���.
            DragSkill.instance.dragSlot.coolTime.fillAmount = 0; 
        }

        DragSkill.instance.dragSlot = null; // �׸��� �巡�� ��ų�� �巡�� ������ �����
    }

    private void CheckQuickSlot() // ������ üũ �Լ�
    {
        for (int i = 0; i < quickSlot.Skill_slots.Length; i++)
        {
            if (quickSlot.Skill_slots[i].skill == DragSkill.instance.dragSlot.skill) // ���� ��ų�� ���� ������ �ִٸ�
            {
                quickSlot.Skill_slots[i].ClearSlot(); // �� ������ �ʱ�ȭ ���� �� 
                return; // ����
            }
        }
    }

    // ��ų �̹����� ���� ����
    private void SetColor(float _alpha)
    {
        Color color = skill_Image.color;
        color.a = _alpha;
        skill_Image.color = color;
    }

    // �κ��丮�� ���ο� ������ ���� �߰�
    public void AddSkill(Skill _skill)
    {
        skill = _skill;
        skill_Image.sprite = _skill.skill_Image;

        SetColor(1);
    }

    private void ClearSlot() // ������ ����ִ� �Լ�
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
            if(coolTime.fillAmount == 0) // ��Ÿ���� �ٵ�����
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
