using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField]
    protected TextMeshProUGUI _text = null; // �������� ������ ǥ�����ִ� �ؽ�Ʈ
    [SerializeField]
    protected Player _player = null; // �÷��̾ ���� �� �� ������ ���

    private int _price = 1; // �������� ����
    public int Price { get => _price; set => _price = value; } // �������� ���� getset

    private int _count = 0; // �������� ����
    public int Count { 
        get => _count; 
        set
        {
            _count = value;
            _text?.SetText($"{_count}");
        }
    }

    public abstract void UseItem(); // ������ ���
    public abstract void Buy(); // ������ ����
}
