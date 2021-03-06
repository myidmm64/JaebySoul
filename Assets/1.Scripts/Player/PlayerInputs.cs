using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class PlayerInputs : MonoBehaviour
{
    private Player _playerMove = null; // 플레이어 캐싱준비
    private PlayerUseSkill _playerUseSkill = null; // 플레이어 캐싱 준비

    [field:SerializeField]
    private UnityEvent OnEscapeButton = null; // esc 버튼이 눌렸을 때 발행될 이벤트
    [field: SerializeField]
    private UnityEvent OnTapButton = null; // tab 버튼이 눌렸을 때 발행될 이벤트

    private Animator _animator = null;

    [SerializeField]
    private GameObject _option = null; // 옵션창
    [SerializeField]
    private GameObject _market = null; // 상점창
    [SerializeField]
    private ButtonManager _manager = null;

    [SerializeField]
    private TextMeshProUGUI _noMPText = null;
    private Sequence seq = null;

    private bool _isOpenUI = false; // 옵션 UI를 열고있는가?
    private bool _isMarketOpen = false; // 상점 UI를 열고 있는가?

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
        //초기화
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
    /// 상점 띄우기
    /// </summary>
    private void VisibleMarket()
    {
        if (_isOpenUI) return;

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (_isMarketOpen) // 닫기
            {
                _isMarketOpen = false;
                _market.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Confined;
                Time.timeScale = 1f;
            }
            else // 열기
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
    /// 옵션 띄우기
    /// </summary>
    private void VisibleMenu()
    {
        if (_isMarketOpen) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(_isOpenUI) // 닫기
            {
                if (_manager.IsOpen) return;

                _isOpenUI = false;
                _option.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Confined;
                Time.timeScale = 1f;
            }
            else // 열기
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
    /// 전투 인풋 받기
    /// </summary>
    private void BattleFunc()
    {
        //왼쪽 클릭 시 배틀 이벤트
        if(Input.GetMouseButtonDown(0))
        {
            if (_playerMove.IsRun || _playerMove.IsFreeze || _playerMove.IsAttackAble == false) return; // 공격 가능한가?

            if (_playerMove.IsZoom == false) // 줌을 안했으면 그냥 근접공격
            {
                if (_playerMove.AttackCnt == 0)
                {
                    MeleeAttack(); // 첫번째 공격일 때 start
                }
            }
            else if (_playerMove.IsZoom == true && _playerMove.IsZoomAttackAble) // 만약 줌을 하고있었다면 줌샷 
            {
                if (_playerUseSkill.MP <= 1) // 마나가 없으면 실행 안함
                {
                    _playerMove.PopUpText("NO MP !!", _noMPText, seq);
                    return;
                }
                _playerUseSkill.MP-= 2; // 줌샷을 할 때 마나 감소
                OnRangeAttack?.Invoke();
                _playerMove.OnZoomShoot?.Invoke();
            }
        }
        //오른쪽 클릭 시 줌 이벤트
        else if(Input.GetMouseButtonDown(1))
        {
            if (_playerMove.IsRun || _playerMove.IsFreeze || _playerMove.IsAttackAble == false) return;

            OnZoom?.Invoke();
            _playerMove.OnZoom?.Invoke();
        }
        //오른쪽 클릭 해제 시 줌 해제
        if(Input.GetMouseButtonUp(1))
        {
            if (_playerMove.IsRun || _playerMove.IsFreeze || _playerMove.IsAttackAble == false) return;

            _playerMove.ExitZoom();
            _playerMove.CrossHairEnable(false);
            _playerMove.OnZoomOut?.Invoke();
        }
    }

    //근접공격을 하는 함수
    public void MeleeAttack() 
    {
        _animator.SetTrigger("Shoot");
        _playerMove.IsAttackAble = false;
        //_playerMove.OnBattle?.Invoke();
        _playerMove.OnBattleReset();
    }

}
