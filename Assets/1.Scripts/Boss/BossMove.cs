using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.Events;

public class BossMove : MonoBehaviour
{
    [SerializeField]
    private GameObject _player = null;
    [SerializeField]
    private float _speed = 2f;
    [SerializeField]
    private float _tailSummonInterval = 2f;
    [SerializeField]
    private float _spawnerSummonInterval = 10f;
    [SerializeField]
    private GameObject _tailPrefab = null;
    [SerializeField]
    private GameObject _spawner = null;
    [SerializeField]
    private GameObject _auraEffect = null;

    [SerializeField]
    private GameObject _meleeAttackEffect = null;
    [SerializeField]
    private Transform[] _effectPos = null;
    private Animator _animator = null;
    private CharacterController _characterController = null;

    [SerializeField]
    private Transform[] _spawnerRandomPos = null;

    private Coroutine _stateChangeCoroutine = null;
    [SerializeField]
    private Canvas _bossCanvas = null;

    private BossDamaged _bossDamaged = null;

    [SerializeField]
    private TextMeshProUGUI _tooltipText = null;
    private Coroutine _tooltipCoroutine = null;

    private bool _berserkerMode = false;

    public UnityEvent OnStart = null;
    public UnityEvent OnFly = null;
    public UnityEvent OnMeleeAttack = null;
    public UnityEvent OnTail = null;

    public enum BossStates
    {
        None,
        Idle,
        Walk,
        MeleeAttack,
        FireAttack,
        Dead,
        JumpHeal
    }

    [SerializeField]
    private BossStates _state = BossStates.None;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _bossDamaged = GetComponent<BossDamaged>();
        //_characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        StartCoroutine(SummonTail());
        StartCoroutine(SummonSkeleton());

        _stateChangeCoroutine = StartCoroutine(StateCoroutine());

        _bossCanvas.gameObject.SetActive(true);
        transform.LookAt(_player.transform.position);

    }

    private IEnumerator ToolTipOn()
    {
        _tooltipText.enabled = true;
        yield return new WaitForSeconds(3f);
        _tooltipText.enabled = false;
    }

    private void Update()
    {
        StateSometing();
    }

    private IEnumerator SummonSkeleton()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnerSummonInterval);

            GameObject spawner = Instantiate(_spawner);
            spawner.transform.position = _spawnerRandomPos[Random.Range(0, _spawnerRandomPos.Length)].position;
        }
    }

    private IEnumerator SummonTail()
    {
        while(true)
        {
            yield return new WaitForSeconds(_tailSummonInterval);
            GameObject effect = Instantiate(_auraEffect, _player.transform.position + Vector3.up * (-1), Quaternion.identity);
            yield return new WaitForSeconds(2f);
            GameObject tail = Instantiate(_tailPrefab, effect.transform.position + new Vector3(0f, -2f, 1.5f) , Quaternion.Euler(new Vector3(-90f, 0f ,0f)));
            OnTail?.Invoke();
            tail.transform.DOMoveY(tail.transform.position.y + 3f, 0.4f);
            Destroy(effect);
        }
    }

    private IEnumerator GenericSummonTail()
    {
        yield return new WaitForSeconds(_tailSummonInterval);
        GameObject effect = Instantiate(_auraEffect, _player.transform.position + Vector3.up * (-1), Quaternion.identity);
        yield return new WaitForSeconds(2f);
        GameObject tail = Instantiate(_tailPrefab, effect.transform.position + new Vector3(0f, -2f, 1.5f), Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
        tail.transform.DOMoveY(tail.transform.position.y + 3f, 0.4f);
        Destroy(effect);
    }

    private IEnumerator StateCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        OnStart?.Invoke();
        yield return new WaitForSeconds(3.5f);

        while (true)
        {

            switch (RandomState())
            {
                case BossStates.None:
                    break;
                case BossStates.Idle:
                    _state = BossStates.Idle;
                    _animator.SetBool("Walk", false);
                    break;
                case BossStates.Walk:
                    _state = BossStates.Walk;
                    _animator.SetBool("Walk", true);
                    break;
                case BossStates.MeleeAttack:
                    _state = BossStates.MeleeAttack;
                    StartCoroutine(MeleeAttack());
                    _animator.SetTrigger("Attack");
                    break;
                case BossStates.FireAttack:
                    _state = BossStates.FireAttack;
                    break;
                case BossStates.JumpHeal:
                    _state = BossStates.JumpHeal;
                    break;

            }

            if (_state == BossStates.Idle)
                yield return new WaitForSeconds(1f);
            if (_state == BossStates.JumpHeal)
                yield return new WaitForSeconds(5f);
            else
                yield return new WaitForSeconds(3f);
        }
    }


    private BossStates RandomState()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) < 2.5f) // 가까이 있다면
        {
            _animator.SetTrigger("Knockback");
            StartCoroutine(Healing());

            if (_tooltipCoroutine != null)
                StopCoroutine(_tooltipCoroutine);

            _tooltipCoroutine = StartCoroutine(ToolTipOn());
            OnFly?.Invoke();
            return BossStates.JumpHeal;
        }
        else if (Vector3.Distance(transform.position + Vector3.forward * 2.3f, _player.transform.position) < 5f) // 가까이 있다면
        {
            return Random.Range(0, 100) < 35 ? BossStates.Idle : BossStates.MeleeAttack;
        }
        else if (Vector3.Distance(transform.position + Vector3.forward * 2.3f, _player.transform.position) >= 5f) // 좀 떨어져 있다면
        {
            _berserkerMode = false;
            return (BossStates)Random.Range(2, 4);
        }
        else if (Vector3.Distance(transform.position + Vector3.forward * 2.3f, _player.transform.position) >= 20f) // 많이 떨어져있다면
        {
            _berserkerMode = true;
            return BossStates.Walk;
        }
        else
        {
            return (BossStates)Random.Range(2, 4);
        }
    }

    private IEnumerator Healing()
    {
        for(int i = 0; i<5; i++)
        {
            yield return new WaitForSeconds(0.8f);
            _bossDamaged.HP += 30;
        }
    }

    private void StateSometing()
    {
        switch (_state)
        {
            case BossStates.None:
                break;
            case BossStates.Idle:
                transform.LookAt(_player.transform.position);
                break;
            case BossStates.Walk:
                transform.LookAt(_player.transform.position);
                if (Vector3.Distance(transform.position + transform.forward * 4.5f, _player.transform.position) >= 3f)
                {
                    transform.position = Vector3.Lerp(transform.position, _player.transform.position, Time.deltaTime * (_berserkerMode ? _speed * 2f : _speed));
                }
                else
                {
                    _animator.SetBool("Walk", false);
                }
                break;
            case BossStates.MeleeAttack:
                transform.LookAt(_player.transform.position);
                break;
            case BossStates.FireAttack:
                break;
            case BossStates.JumpHeal:
                transform.LookAt(_player.transform.position);
                break;
            case BossStates.Dead:
                break;
            default:
                break;
        }

    }

    private IEnumerator MeleeAttack()
    {
        yield return new WaitForSeconds(1.7f);
        for(int i = 0; i<_effectPos.Length; i++)
        {
            GameObject efffect = Instantiate(_meleeAttackEffect, _effectPos[i]);
            Destroy(efffect, 2f);
        }
    }

    public void OnMeleeAttackSound()
    {
        OnMeleeAttack?.Invoke();
    }
    public void OnDieSound()
    {
        OnStart?.Invoke();
    }

    public void CoroutineStop()
    {
        _state = BossStates.None;
        StopAllCoroutines();
    }
}
