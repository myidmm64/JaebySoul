using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ҷ��� ������ �� ��ũ��Ʈ
public class BulletProjectile : MonoBehaviour
{
    [SerializeField]
    private GameObject _vfxHitGreen; // Ÿ�ٿ� �¾��� �� ���� ����Ʈ
    [SerializeField]
    private GameObject _vfxHitRed; // Ÿ�ٿ� ���� �ʾ��� �� ���� ����Ʈ
    [SerializeField]
    private float _speed = 40f; // �ҷ��� �ӵ�
    private Rigidbody _rigid; // ������ٵ� ĳ�� �غ�

    [SerializeField]
    private int _damage = 35;
    public int Damage
    {
        get => _damage;
        set
        {
            _damage = value;
        }
    }

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _rigid.velocity = transform.forward * _speed; //�ҷ� �����̱�
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if(other.CompareTag("Enemy"))
        {
            if (other.GetComponent<SkulMove>() != null) // ���� ���ÿ� �΋H������ ������
                other.GetComponent<SkulMove>().Damage(_damage);
        }
        if(other.CompareTag("Boss"))
        {
            //�������� ������
            if (other.GetComponent<BossDamaged>() != null) // 
                other.GetComponent<BossDamaged>().Damage((int)(_damage * 0.33f));
        }

        if (other.GetComponent<BulletTarget>() != null) // Ÿ�ٿ� �¾Ҵ°�?
        {
            Instantiate(_vfxHitGreen, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_vfxHitRed, transform.position, Quaternion.identity);
        }
        Destroy(gameObject); // �ҷ� �ı�
    }
}
