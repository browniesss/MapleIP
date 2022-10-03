using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // ���� �Ŵ��� ��ũ��Ʈ 
    public AudioClip[] audioList; // ���� ����� ����.

    AudioSource audioSource;

    int randBgm;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        randBgm = Random.Range(0, 3); // ���������� 0 ~ 2 ���� �޾ƿ� �� 

        audioSource.clip = audioList[randBgm]; // �ش� ��ũ��Ʈ�� ������ �ִ� ������Ʈ�� AudiSource Ŭ���� audioList�� bgm �� �������� �־�
        audioSource.Play(); // ���
    }

    void Update()
    {
        
    }
}
