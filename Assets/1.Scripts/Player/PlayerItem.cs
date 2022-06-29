using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerItem : MonoBehaviour
{
    [field: SerializeField]
    private UnityEvent OnHPPosion = null; // HP������ �Ծ��� �� ����� �̺�Ʈ
    [field: SerializeField]
    private UnityEvent OnMPPosion = null; // MP������ �Ծ��� �� ����� �̺�Ʈ


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)) // FŰ�� ������ HP ����
        {
            OnHPPosion?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.G)) // GŰ�� ������ MP����
        {
            OnMPPosion?.Invoke();
        }
    }
}
