using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using static Define;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _attackEffect = new List<GameObject>(); // ���� ����Ʈ ����Ʈ

    [SerializeField]
    private GameObject _criticalEffect = null;

    [SerializeField]
    [Range(0f, 100f)]
    private float _critical = 20f;

    [SerializeField]
    private TextMeshProUGUI _criticalText = null;
    [SerializeField]
    private RectTransform textTrans = null;

    [SerializeField]
    private GameObject _finalAttackEffect = null;

    [SerializeField]
    private GameObject _attackDamageEffect = null;

    [SerializeField]
    private Transform _playerTransform = null; // �÷��̾��� Ʈ������
    private Player _player = null; // 
    private PlayerInputs _playerInput = null; // �⺻���� �Լ� ��������

    [SerializeField]
    private int _damage = 10; // ���� ������
    public int Damage { get => _damage; set => _damage = value; }

    Sequence seq = null;

    private void Awake() 
    {
        //�÷��̾� Ʈ������ �پ������� ��������
        if (_playerTransform == null) return;

        _player = _playerTransform.GetComponent<Player>();
        _playerInput = _player.GetComponent<PlayerInputs>();

    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);


        if(other.CompareTag("Enemy")) 
        {
            other.GetComponent<SkulMove>()?.Damage(damage : _damage); //���� �༮���� ������ �ֱ�
        }
        if(other.CompareTag("Boss"))
        {
            other.GetComponent<BossDamaged>()?.Damage(damage: _damage); //���� �༮���� ������ �ֱ�
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward * 3f, out hit))
            {
                GameObject effect = Instantiate(_attackDamageEffect, hit.point, Quaternion.identity);
            }
        }
    }

    private void Update()
    {
        if (_player == null) return; // ���� Ʈ�������� �־����� �ʾ����� ����

        if(Input.GetMouseButtonDown(0))
        {
            if(_player.AttackCnt > 0)
            {
                AttackReady(_player.AttackCnt);
            }

        }
    }

    private void AttackReady(int idx)
    {
        switch (idx)
        {
            case 1: // 2��° ���ݺ���
                _playerInput.MeleeAttack();
                break;
            case 2:
                _playerInput.MeleeAttack();
                break;
            case 3:
                _playerInput.MeleeAttack();
                break;
            default:
                break;
        }

    }

    GameObject obj = null;

    public void AttackEffectSpawn(int value) // ���� �ε������� �ٸ� ������Ʈ�� �׷� �� ����
    {
        bool isCritical = false;

        int random = Random.Range(0, _attackEffect.Count); // ����Ʈ �ε��� ��������

        if (Random.Range(0, 100f) < _critical)
        {
            isCritical = true;
            obj = Instantiate(_criticalEffect, _playerTransform); // ũ��Ƽ�� ����Ʈ

            _player.PopUpText("ũ��Ƽ��!!", _criticalText, seq);
        }
        else
            obj = Instantiate(_attackEffect[Random.Range(0, _attackEffect.Count)], _playerTransform); // ���� ����Ʈ ����


        if (value == 4)
        {
            obj.AddComponent<EffectMove>();
        }


        // ������Ʈ �ʱ�ȭ
        float rand = Random.Range(0.3f, 0.45f);
        obj.transform.localScale = Vector3.one * rand;
        obj.transform.rotation *= Quaternion.Euler(new Vector3(0, 90f, 0));
        switch (value)
        {
            case 1:
                obj.GetComponent<PlayerAttack>().Damage = _damage;
                Destroy(obj, 0.5f);
                break;
            case 2:
                obj.transform.rotation *= Quaternion.Euler(new Vector3(45f, 0, 0));
                obj.transform.localScale = Vector3.one * Random.Range(0.45f, 0.55f);
                obj.GetComponent<PlayerAttack>().Damage = (int)(_damage * 1.2f);
                Destroy(obj, 0.5f);
                break;
            case 3:
                obj.transform.rotation *= Quaternion.Euler(new Vector3(-35f, 0, 0));
                obj.transform.localScale = Vector3.one * Random.Range(0.45f, 0.55f);
                obj.GetComponent<PlayerAttack>().Damage = (int)(_damage * 1.5f);
                Destroy(obj, 0.5f);
                break;
            case 4:
                Destroy(obj, 1f);
                break;
            default:
                break;
        }

        if(isCritical)
            obj.GetComponent<PlayerAttack>().Damage = (int)(_damage * 2f);

        obj.transform.position += obj.transform.right * -1f * 1f + Vector3.up * 0.5f;

        obj.transform.SetParent(null);

    }
}
