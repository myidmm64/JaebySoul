using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������ ������ �� ��ũ��Ʈ
public class Coin : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 100f; // ȸ���ӵ�
    private Vector3 _rotateVec = Vector3.zero; // ȸ������

    private void Start()
    {
        _rotateVec = new Vector3(0f, 0f, _rotateSpeed * Time.deltaTime); // ȸ������ ĳ��
    }

    private void Update()
    {
        transform.Rotate(_rotateVec); // ȸ��
    }
}
