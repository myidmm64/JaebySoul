using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BossDamaged : MonoBehaviour
{
    [SerializeField]
    private Slider _hpSlider = null;
    [SerializeField]
    private int _maxHP;
    private int _hp;
    public int HP
    {
        get => _hp;
        set
        {
            _hp = value;
            _hp = Mathf.Clamp(_hp, 0, _maxHP);
            _hpSlider.value = (float)_hp / _maxHP;
        }
    }
    private Animator _animator = null;

    private bool _isDead = false;

    private BossMove _bossMove = null;

    [SerializeField]
    private GameObject _coin = null;
    [SerializeField]
    private GameObject _dieEffect = null;

    private void Awake()
    {
        _bossMove = GetComponent<BossMove>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        HP = _maxHP;
    }

    public void Damage(int damage)
    {
        if (_isDead) return;

        HP -= damage;

        if(HP <= 0)
        {
            _isDead = true;
            _bossMove.CoroutineStop();
            _animator.SetTrigger("Die");
            Die();
        }
    }

    private void Die()
    {
        GameObject effect = Instantiate(_dieEffect, transform.position, Quaternion.Euler(-90f, 0f, 70f));
        Destroy(effect, 3f);
        StartCoroutine(CoinBoom());
    }

    private IEnumerator CoinBoom()
    {
        for(int i =0; i<40; i++)
        {
            Vector3 random = Random.insideUnitSphere * 5f;
            Instantiate(_coin, transform.position + new Vector3(random.x, 0.5f, random.z), Quaternion.Euler(new Vector3(90f, 0f, 0f)));
            yield return new WaitForSeconds(0.1f);
        }
    }
}
