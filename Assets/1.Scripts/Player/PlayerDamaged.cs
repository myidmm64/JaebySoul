using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerDamaged : MonoBehaviour
{
    [SerializeField]
    private int _hp = 10; // �÷��̾��� Hp
    public int HP
    {
        get => _hp;

        set
        {
            _hp = value;

            _hp = Mathf.Clamp(_hp, 0 , _maxHP);

            _hpSlider.value = (float)_hp / _maxHP;

            if (_hp <= 3)
            {
                _hpWaringImage.enabled = true;
            }
            else
            {
                _hpWaringImage.enabled = false;
            }

            if (_isDamage == false)
            {
                if (HP <= 0)
                {
                    if (_isDead)
                        return;
                    _isDead = true;

                    OnDie?.Invoke();

                    _animator.SetTrigger("Death");

                    Invoke("Restart", 2f); // �����
                    return;
                }

                StartCoroutine(DamageCoroutine());
            }
        }
    }

    [SerializeField]
    private int _maxHP = 10;
    public int MaxHP
    {
        get => _maxHP;

        set
        {
            _maxHP = value;

            _hpSlider.value = (float)_hp / _maxHP;

        }
    }

    [SerializeField]
    private float _damageDelay = 1f; // �´� ������

    private bool _isDamage = false; // ���� �°��ִ°�
    private bool _isDead = false; // �׾��°�?
    private Animator _animator = null; // �ִϸ����� ĳ���غ�

    private Player _player = null;

    [field:SerializeField]
    private UnityEvent OnDie = null;


    [SerializeField]
    private Slider _hpSlider = null;
    [SerializeField]
    private Image _hpWaringImage = null;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        HP = MaxHP;
    }

    /*private void OnGUI()
    {
        GUIStyle gUI = new GUIStyle();
        gUI.fontSize = 50;
        gUI.fontStyle = FontStyle.Bold;
        gUI.normal.textColor = Color.red;
        GUI.Label(new Rect(10, 60, 100, 200), $"HP : {_hp}", gUI);
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAtk")) // ���� ���� ���ݿ� �¾Ҵ°�
        {
            if (_isDamage == false)
            {
                Debug.Log(other.gameObject.name);
                HP -= other.GetComponent<SkulAttack>().Damage;

            }
        }
    }

    /// <summary>
    /// ���� ����� �Լ�
    /// </summary>
    public void Restart()
    {
        _player.Init();
        SceneManager.LoadScene("SampleScene");
    }

    /// <summary>
    /// �ǰ� ���� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    private IEnumerator DamageCoroutine()
    {
        _isDamage = true;
        _animator.SetTrigger("Damage");
        Debug.Log("�¾Ҿ��");
        yield return new WaitForSeconds(_damageDelay);
        _isDamage = false;
    }
}
