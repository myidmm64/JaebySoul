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
        if(Input.GetKeyDown(KeyCode.F)) // TŰ�� ������ HP ����
        {
            OnHPPosion?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.G)) // YŰ�� ������ MP����
        {
            OnMPPosion?.Invoke();
        }
    }
}
