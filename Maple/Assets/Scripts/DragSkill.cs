using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSkill : MonoBehaviour
{
    // 드래그 하는 스킬 슬롯 스크립트
    static public DragSkill instance; // 싱글톤 인스턴스
    public SkillSlot dragSlot; // 드래그 하는 슬롯을 넣어줌.

    [SerializeField]
    public Image imageSkill; // 스킬의 이미지

    public Image MyImage;

    void Start()
    {
        instance = this; // 싱글톤 
        MyImage = GetComponent<Image>(); 
    }

    public void DragSetImage(Image _skillImage) // 드래그 할때 이미지를 변경해줄 함수.
    {
        imageSkill = _skillImage;
        MyImage.sprite = imageSkill.sprite;
        SetColor(1);
    }

    public void SetColor(float _alpha) // 해당 슬롯과 이미지의 Color 값을 바꿔주는 함수.
    {
        Color color = new Color(255, 255, 255, _alpha);
        Color originColor = new Color(255, 255, 255, 0);

        imageSkill.color = originColor;

        MyImage.color = color;
    }
}
