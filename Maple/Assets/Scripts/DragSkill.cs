using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSkill : MonoBehaviour
{
    // �巡�� �ϴ� ��ų ���� ��ũ��Ʈ
    static public DragSkill instance; // �̱��� �ν��Ͻ�
    public SkillSlot dragSlot; // �巡�� �ϴ� ������ �־���.

    [SerializeField]
    public Image imageSkill; // ��ų�� �̹���

    public Image MyImage;

    void Start()
    {
        instance = this; // �̱��� 
        MyImage = GetComponent<Image>(); 
    }

    public void DragSetImage(Image _skillImage) // �巡�� �Ҷ� �̹����� �������� �Լ�.
    {
        imageSkill = _skillImage;
        MyImage.sprite = imageSkill.sprite;
        SetColor(1);
    }

    public void SetColor(float _alpha) // �ش� ���԰� �̹����� Color ���� �ٲ��ִ� �Լ�.
    {
        Color color = new Color(255, 255, 255, _alpha);
        Color originColor = new Color(255, 255, 255, 0);

        imageSkill.color = originColor;

        MyImage.color = color;
    }
}
