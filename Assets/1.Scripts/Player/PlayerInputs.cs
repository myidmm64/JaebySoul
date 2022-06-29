using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class PlayerInputs : MonoBehaviour
{
    private Player _playerMove = null; // �÷��̾� ĳ���غ�
    private PlayerUseSkill _playerUseSkill = null; // �÷��̾� ĳ�� �غ�

    [field:SerializeField]
    private UnityEvent OnEscapeButton = null; // esc ��ư�� ������ �� ����� �̺�Ʈ
    [field: SerializeField]
    private UnityEvent OnTapButton = null; // tab ��ư�� ������ �� ����� �̺�Ʈ

    private Animator _animator = null;

    [SerializeField]
    private GameObject _option = null; // �ɼ�â
    [SerializeField]
    private GameObject _market = null; // ����â
    [SerializeField]
    private ButtonManager _manager = null;

    [SerializeField]
    private TextMeshProUGUI _noMPText = null;
    private Sequence seq = null;

    private bool _isOpenUI = false; // �ɼ� UI�� �����ִ°�?
    private bool _isMarketOpen = false; // ���� UI�� ���� �ִ°�?

    public UnityEvent OnZoom;
    public UnityEvent OnRangeAttack;

    private void Awake()
    {
        _playerMove = GetComponent<Player>();
        _playerUseSkill = GetComponent<PlayerUseSkill>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        //�ʱ�ȭ
        _option.SetActive(false);
        _market.SetActive(false);
    }

    private void Update()
    {
        BattleFunc();

        VisibleMenu();

        VisibleMarket();
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    private void VisibleMarket()
    {
        if (_isOpenUI) return;

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (_isMarketOpen) // �ݱ�
            {
                _isMarketOpen = false;
                _market.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Confined;
                Time.timeScale = 1f;
            }
            else // ����
            {
                OnTapButton?.Invoke();
                _isMarketOpen = true;
                _market.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0f;
            }
        }
    }

    /// <summary>
    /// �ɼ� ����
    /// </summary>
    private void VisibleMenu()
    {
        if (_isMarketOpen) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(_isOpenUI) // �ݱ�
            {
                if (_manager.IsOpen) return;

                _isOpenUI = false;
                _option.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Confined;
                Time.timeScale = 1f;
            }
            else // ����
            {
                _isOpenUI = true;
                _option.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0f;
            }
        }
    }

    /// <summary>
    /// ���� ��ǲ �ޱ�
    /// </summary>
    private void BattleFunc()
    {
        //���� Ŭ�� �� ��Ʋ �̺�Ʈ
        if(Input.GetMouseButtonDown(0))
        {
            if (_playerMove.IsRun || _playerMove.IsFreeze || _playerMove.IsAttackAble == false) return; // ���� �����Ѱ�?

            if (_playerMove.IsZoom == false) // ���� �������� �׳� ��������
            {
                if (_playerMove.AttackCnt == 0)
                {
                    MeleeAttack(); // ù��° ������ �� start
                }
            }
            else if (_playerMove.IsZoom == true && _playerMove.IsZoomAttackAble) // ���� ���� �ϰ��־��ٸ� �ܼ� 
            {
                if (_playerUseSkill.MP <= 1) // ������ ������ ���� ����
                {
                    _playerMove.PopUpText("NO MP !!", _noMPText, seq);
                    return;
                }
                _playerUseSkill.MP-= 2; // �ܼ��� �� �� ���� ����
                OnRangeAttack?.Invoke();
                _playerMove.OnZoomShoot?.Invoke();
            }
        }
        //������ Ŭ�� �� �� �̺�Ʈ
        else if(Input.GetMouseButtonDown(1))
        {
            if (_playerMove.IsRun || _playerMove.IsFreeze || _playerMove.IsAttackAble == false) return;

            OnZoom?.Invoke();
            _playerMove.OnZoom?.Invoke();
        }
        //������ Ŭ�� ���� �� �� ����
        if(Input.GetMouseButtonUp(1))
        {
            if (_playerMove.IsRun || _playerMove.IsFreeze || _playerMove.IsAttackAble == false) return;

            _playerMove.ExitZoom();
            _playerMove.CrossHairEnable(false);
            _playerMove.OnZoomOut?.Invoke();
        }
    }

    //���������� �ϴ� �Լ�
    public void MeleeAttack() 
    {
        _animator.SetTrigger("Shoot");
        _playerMove.IsAttackAble = false;
        //_playerMove.OnBattle?.Invoke();
        _playerMove.OnBattleReset();
    }

}
