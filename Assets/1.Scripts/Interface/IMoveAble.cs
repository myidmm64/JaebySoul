using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveAble
{
    public bool IsStop { get; set; } // ����ߵǴ���

    public void Move(); // �������� �̵� �Լ�
}
