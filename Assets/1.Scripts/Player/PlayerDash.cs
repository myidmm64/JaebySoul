using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerDash : MonoBehaviour
{
    private Animator _animator = null; // 애니메이터 캐싱 준비
    private Player _player = null; // 플레이어 캐싱 준비
    [SerializeField]
    private float _coolTime = 2f; // 대시 쿨타임
    [SerializeField]
    private float _currentTime = 2f;
    [SerializeField]
    private TextMeshProUGUI _coolTimeText = null;
    [SerializeField]
    private Image _fade = null;

    [SerializeField]
    private float _dashPower = 5f; // 대시 양
    [SerializeField]
    private LayerMask _mapLayer;
    private bool _dashAble = true; // 대시가 가능한가

    private float _horizontal = 0f; // 입력값
    private float _vertical = 0f; // 입력값
    private Vector3 _dashDirection = Vector3.zero; // 대시 방향

    [SerializeField]
    private TextMeshProUGUI _noDashText = null;
    private Sequence seqE = null;

    public UnityEvent OnDash = null;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetDashDir();

            Ray ray = new Ray(transform.position, _dashDirection);
            if(Physics.Raycast(ray, 11f, _mapLayer))
            {
                _player.PopUpText("CANT DASH !!", _noDashText, seqE);
                return;
            }
            Ray ray2 = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray2, 11f, _mapLayer))
            {
                _player.PopUpText("CANT DASH !!", _noDashText, seqE);
                return;
            }


            if (_player.IsRun || _player.IsZoom || _player.IsFreeze || _player.playerState == Player.PlayerState.Skilling) // 달리고있거나 줌 상태면 리턴
            {
                return;
            }

            if (_dashAble) // 대시가 가능하면 실행
            {
                OnDash?.Invoke();
                Dash();
                StartCoroutine(DashAnimation());
                StartCoroutine(DashCoroutine());
            }
        }

        _currentTime += Time.deltaTime;
        _currentTime = Mathf.Clamp(_currentTime, 0f, _coolTime);
        if(_dashAble)
            _coolTimeText.SetText("");
        else
            _coolTimeText.SetText((_coolTime - _currentTime).ToString("N1"));
    }

    private void SetDashDir()
    {
        // 레이캐스트로 제한두기
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");
        _dashDirection = new Vector3(_horizontal, 0f, _vertical);
        if (_dashDirection == Vector3.zero)
            _dashDirection = transform.forward; // 안 눌렀을 때 디폴트값으로 앞방향
        else
            _dashDirection = transform.rotation * _dashDirection;
    }

    /// <summary>
    /// 대시를 수행하는 함수
    /// </summary>
    private void Dash()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");
        _dashDirection = new Vector3(_horizontal, 0f, _vertical);

        if(_player.IsBattle || _player.IsZoom)
        {
            if(_dashDirection == Vector3.zero)
                _dashDirection = transform.forward; // 안 눌렀을 때 디폴트값으로 앞방향
            else
                _dashDirection = transform.rotation * _dashDirection;

            if (_vertical < 0f)
                transform.rotation *= Quaternion.Euler(new Vector3(0f, 180f, 0f));

            transform.DOMove(transform.position + _dashDirection * 5f, 0.5f);
        }
        else
            transform.DOMove(transform.position + transform.forward.normalized * 5f, 0.5f);
    }

    /// <summary>
    /// 대시 애니메이션 실행 함수
    /// </summary>
    /// <returns></returns>
    private IEnumerator DashAnimation()
    {
        int layer = _player.gameObject.layer;
        //무적판정 추가
        _player.IsStop = true;
        _player.IsFreeze = true;
        _player.SetState(Player.PlayerState.Dash);
        _animator.SetTrigger("IsDash");
        _player.gameObject.layer = 11; // 무적 레이어로 옮김

        yield return new WaitForSeconds(0.5f);
        _player.IsStop = false;
        _player.IsFreeze = false;
        _player.gameObject.layer = layer; // 레이어 다시 옮김

        // 대시 이전 상태로 되돌리기
        if (_player.IsBattle)
            _player.SetState(Player.PlayerState.Battle);
        else if (_player.IsZoom)
            _player.SetState(Player.PlayerState.Zoom);
        else
            _player.SetState(Player.PlayerState.Idle);
    }
    
    /// <summary>
    /// 대시 가능하게 바꿔주는 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator DashCoroutine()
    {
        _dashAble = false;
        _fade.enabled = true;
        _currentTime = 0f;
        yield return new WaitForSeconds(_coolTime);
        _fade.enabled = false;
        _dashAble = true;
    }
}
