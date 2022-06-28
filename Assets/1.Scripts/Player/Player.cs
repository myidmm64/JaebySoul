using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using static Define;
using TMPro;

public class Player : MonoBehaviour, IMoveAble
{
    [SerializeField]
    private float _speed = 5f; // ĳ������ �̵��ӵ�
    [SerializeField]
    private int _maxAnimationIdx = 2;
    [SerializeField]
    private float _runSpeed = 10f; // ĳ���� �޸��� �̵��ӵ�
    [SerializeField]
    private Vector3 _gravityScale = new Vector3(0f, 9.8f, 0f); // ���߿� ������ �� �߷°�
    [SerializeField]
    private float _bodyRotateSpeed = 10f; // �� ������ �ӵ�
    [SerializeField]
    private float _headRotateSpeed = 2f; // ���콺�� ���� ���� ������ �ӵ�
    [SerializeField]
    private Vector2 _defaultSensitivity = Vector2.zero;
    [SerializeField]
    private float _normalSensitivity = 1f; // ��� �� ���콺 ����
    [SerializeField]
    private float _aimSensiticity = 0.5f; // �� �� ���콺 ����
    [SerializeField]
    private float _battleDuration = 3f; // ���� ���ӽð�
    [SerializeField]
    private float _zoomDuration = 3f; // �� ���ӽð�
    [SerializeField]
    private Image _crossHair = null; // ũ�ν����
    private Vector3 _amount = Vector3.zero; // �̵��� ���� ���Ͱ�
    private float _horizontal = 0f; // Input.GetAxisRaw("Horizontal")
    private float _vertical = 0f; // Input.GetAxisRaw("Vertical")
    [SerializeField]
    private BoxCollider _atkCollider = null; // �÷��̾� ���� �ݶ��̴�
    [SerializeField]
    private GameObject _sword = null; // �� ������Ʈ
    private PlayerAttack _playerAttack = null;

    private Coroutine IsBattleCo = null; // IsBattleCoroutine�� ��Ƶ� Coroutine ����
    private Coroutine IsZoomCo = null; // IsZoomCoroutine�� ��Ƶ� Coroutine ����
    private Coroutine IsAttackCo = null; // �� ��Ƶ� Coroutine ����

    private int _attackCnt = 0;
    public int AttackCnt { get => _attackCnt; }

    public UnityEvent OnBattle = null; // Battle ������ �� ����� �̺�Ʈ
    public UnityEvent OnIdle = null; // Battle ���°� �ƴ� �� ����� �̺�Ʈ
    public UnityEvent OnZoom = null; // ��Ŭ���� ������ �� ����� �̺�Ʈ
    public UnityEvent OnZoomOut = null; // 
    public UnityEvent OnZoomShoot = null; // ��Ŭ���� ������ ��Ŭ���� ������ �� ����� �̺�Ʈ
    public UnityEvent OnDash = null; // ��� Ű�� ������ �� ����� �̺�Ʈ

    private bool _isStop = false; // true�� �ȿ�����
    public bool IsStop { get => _isStop; set => _isStop = value; } // �������̽� ����
    private bool _isBattle = false; // true�� ��������
    public bool IsBattle { get => _isBattle; set => _isBattle = value; }
    private bool _isZoom = false; // true�� �� ����
    public bool IsZoom { get => _isZoom; set => _isZoom = value; }
    private bool _isRun = false; // true�� �޸��� ����
    public bool IsRun { get => _isRun; set => _isRun = value; }
    private bool _isFreeze = false; // true�� �������� ���ϴ� ����
    public bool IsFreeze { get => _isFreeze; set => _isFreeze = value; }
    private bool _isAttackAble = true; // true�� ����Ŭ�� ����
    public bool IsAttackAble { get => _isAttackAble; set => _isAttackAble = value; }
    private bool _isZoomAttackAble = true; // true�� �Ѿ� ��� ����
    public bool IsZoomAttackAble { get => _isZoomAttackAble; set => _isZoomAttackAble = value; }

    [SerializeField]
    private CinemachineFreeLook _cinemacine = null; // ���� �����ؼ� ������� ����
    [SerializeField]
    private CinemachineFollowZoom _zoom = null; // �� ���� ������� ����

    private CharacterController _characterController = null; // ĳ���� ��Ʈ�ѷ� ĳ�� �غ�
    private CollisionFlags _collisionFlags = CollisionFlags.None; // CollisionFlags �ʱ�ȭ
    private Transform _cam = null; // Camera.main.Transform ĳ�� �غ�
    private Animator _animator = null; // �ִϸ����� ĳ�� �غ�
    private PlayerDamaged _playerDamaged = null;

    [SerializeField]
    private PlayerState _playerState = PlayerState.None; // �÷��̾� ����
    public PlayerState playerState => _playerState;

    private int _coin = 0;
    public int Coin { get => _coin; 
    set
        {
            _coin = value;
            _coinText.SetText($"{_coin}");
        }
    } // ���� �������ִ� ���� 


    [SerializeField]
    private TextMeshProUGUI _coinText = null;

    private int _monsterCnt = 0; //
    public int MonsterCnt
    {
        get => _monsterCnt;
        set
        {
            if (_isWeakMonsterEnd)
                return;
            _monsterCnt = value;
            _monsterCnt = Mathf.Clamp(_monsterCnt, 0, 66);
            _monsterCntText.SetText($"{_monsterCnt} / 66");
            if(_monsterCnt >= 6)
            {
                _isWeakMonsterEnd = true;
                _bossSpawner.SetActive(true);
                nightChanger.AlwaysNight = true;
                Debug.Log("��������");
            }
        }
    }

    private bool _isWeakMonsterEnd = false;

    [SerializeField]
    private TextMeshProUGUI _monsterCntText = null;
    [SerializeField]
    private GameObject _bossSpawner = null;

    //�÷��̾��� ����
    public enum PlayerState
    {
        None = -1,
        Idle,
        Battle,
        Zoom,
        Run,
        Dash,
        ZoomShot,
        Skilling
    }

    private void Awake()
    {
        // ĳ��
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        _playerAttack = _atkCollider.GetComponent<PlayerAttack>();
        _cam = MainCam.transform;
        _playerDamaged = GetComponent<PlayerDamaged>();

        //���߿� Core�� �ű��
        //Cursor.visible = false; // ���콺 ������ ��Ȱ��ȭ

    }

    private void Start()
    {
        // �ʱ�ȭ
        Init();
    }

    public void Init()
    {
        _defaultSensitivity = new Vector2(_cinemacine.m_XAxis.m_MaxSpeed, _cinemacine.m_YAxis.m_MaxSpeed);
        OnIdle?.Invoke();
        ExitZoom();
    }

    private void Update()
    {
        Move();
        StateDoSomething();
    }


    /// <summary>
    /// ������Ʈ�� ���� ������ ��
    /// </summary>
    private void StateDoSomething()
    {
        if (IsFreeze)
            return;

        switch (_playerState)
        {
            case PlayerState.None:
                break;
            case PlayerState.Idle:
                BodyRotate(_amount);
                break;
            case PlayerState.Battle:
                HeadRotate();
                break;
            case PlayerState.Zoom:
                HeadRotate();
                break;
            case PlayerState.Run:
                BodyRotate(_amount);
                break;
            case PlayerState.Dash:
                break;
            case PlayerState.ZoomShot:
                HeadRotate();
                break;
            case PlayerState.Skilling:
                HeadRotate();
                break;
            default:
                break;
        }
    }

    /*private void OnGUI()
    {
        GUIStyle gUI = new GUIStyle();
        gUI.fontSize = 50;
        gUI.fontStyle = FontStyle.Bold;
        gUI.normal.textColor = Color.red;
        GUI.Label(new Rect(10, 100, 100, 200), $"Coin : {Coin}", gUI);
        GUI.Label(new Rect(10, 260, 100, 200), $"cnt : {MonsterCnt}", gUI);


        //if (GUI.Button(new Rect(10,100,600,80), $"ĳ���Ϳ� ���� ���� �ϴ� ��ư", gUI))
        //{
        //    Debug.Log("��ư ���� !!");
        //}
    }*/

    /// <summary>
    /// ĳ���� �̵� �Լ�
    /// </summary>
    public void Move()
    {
        if (IsFreeze || IsStop)
            return;

        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        // isStop�̰ų� Ű�� ������ �ʾ����� return
        if((_horizontal == 0f && _vertical == 0f) || _isStop == true)
        {
            _animator.SetBool("IsMove", false);
            return;
        }
        _animator.SetBool("IsMove", true);

        SetMoveAnimation();

        //�̵� ������
        Vector3 forward = _cam.TransformDirection(Vector3.forward);
        forward.y = 0f;
        Vector3 right = new Vector3(forward.z , 0f , -forward.x);
        _amount = (forward * _vertical + right * _horizontal) * Time.deltaTime;
        _amount *= IsRun ? _runSpeed : _speed;

        if (_collisionFlags == CollisionFlags.None)
            _amount -= _gravityScale * Time.deltaTime;

        _collisionFlags = _characterController.Move(_amount);

    }

    /// <summary>
    /// �ִϸ����Ϳ� Setfloat�� ���� �� ����
    /// </summary>
    private void SetMoveAnimation()
    {
        float ver = Input.GetAxis("Vertical");
        float ho = Input.GetAxis("Horizontal");

        _animator.SetFloat("MoveX", ho);
        _animator.SetFloat("MoveY", ver);
    }

    /// <summary>
    /// Battle ���°� �ƴ� �� �� ������ �Լ�
    /// </summary>
    /// <param name="dir"></param>
    private void BodyRotate(Vector3 dir)
    {
        if (IsFreeze)
            return;

        // ���� ������
        dir.y = 0f;
        if(dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), _bodyRotateSpeed * Time.deltaTime);
    }

    /// <summary>
    /// ���� ������ �� �� ������ �Լ�
    /// </summary>
    /// <param name="��� ȸ��"></param>
    public void HeadRotate(bool immediately = false)
    {

        Quaternion rot = Quaternion.identity;
        //rot = Quaternion.Euler(new Vector3(0f, 60f, 0f));
        rot *= Quaternion.Euler(new Vector3(0f, _cinemacine.m_XAxis.Value, 0f));

        if (immediately)
            transform.rotation = rot;
        else
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, _headRotateSpeed * Time.deltaTime);
    }

    /// <summary>
    /// ���� �ִϸ��̼� �������� �� ����� ��
    /// </summary>
    public void SwordAttackStart()
    {
        //attackAble
        if (IsAttackCo != null)
            StopCoroutine(IsAttackCo);

        IsAttackCo = StartCoroutine(IsAttackCoroutine());

        OnBattle?.Invoke();
        _sword.SetActive(true);
        //_atkCollider.enabled = true;
        _isStop = true;
        IsAttackAble = false;
    }

    /// <summary>
    /// ���� ���� �ڷ�ƾ !!
    /// </summary>
    /// <returns></returns>
    private IEnumerator IsAttackCoroutine()
    {
        _attackCnt++;
        _animator.SetInteger("AttackCnt", _attackCnt);
        Debug.Log($"attack Cnt => {_attackCnt}");

        _playerAttack.AttackEffectSpawn(_attackCnt);

        if (_attackCnt == _maxAnimationIdx + 1)
        {
            _attackCnt = 0;
            _animator.SetInteger("AttackCnt", _attackCnt);
            StopCoroutine(IsAttackCo);
        }

        yield return new WaitForSeconds(1f);
        _attackCnt = 0;
        _animator.SetInteger("AttackCnt", _attackCnt);
        Debug.Log($"���� ī��Ʈ �ʱ�ȭ");
    }
    
    /// <summary>
    /// ���� �ִϸ��̼� ������ �� ����� ��
    /// </summary>
    public void SwordAttackEnd()
    {
        //_atkCollider.enabled = false;
        _isStop = false;
        IsAttackAble = true;
    }

    /// <summary>
    /// ��Ʋ �̺�Ʈ
    /// </summary>
    public void OnBattleReset()
    {
        _sword.SetActive(true);
        _playerState = PlayerState.Battle;

        CrossHairEnable(false);

        HeadRotate(true);

        if (IsZoomCo != null)
            StopCoroutine(IsZoomCo);
        if (IsBattleCo != null)
        {
            StopCoroutine(IsBattleCo);
            _isBattle = false;
            _animator.SetBool("IsBattle", _isBattle);
        }
        IsBattleCo = StartCoroutine(IsBattleCoroutine(_battleDuration));
    }


    /// <summary>
    /// Idle �̺�Ʈ
    /// </summary>
    public void OnIdleReset()
    {
        _sword.SetActive(false);

        IsBattle = false;
        IsZoom = false;

        _playerState = PlayerState.Idle;

        CrossHairEnable(false);

        if (IsBattleCo != null)
        {
            StopCoroutine(IsBattleCo);
            _isBattle = false;
            _animator.SetBool("IsBattle", _isBattle);
        }
        if (IsZoomCo != null)
            StopCoroutine(IsZoomCo);
    }

    /// <summary>
    /// �� �̺�Ʈ
    /// </summary>
    public void OnZoomReset()
    {
        //��Ʋ ��� �� �����־�� ��
        _sword.SetActive(false);
        _isBattle = false;
        //�ִϸ��̼�

        CrossHairEnable(true);

        Zoom();

        if (IsBattleCo != null)
        {
            StopCoroutine(IsBattleCo);
            _isBattle = false;
            _animator.SetBool("IsBattle", _isBattle);
        }
        if (IsZoomCo != null)
            StopCoroutine(IsZoomCo);
        IsZoomCo = StartCoroutine(IsZoomCoroutine(_zoomDuration));
    }

    /// <summary>
    /// �� �� �̺�Ʈ
    /// </summary>
    public void OnZoomShootReset()
    {
        if (IsZoomCo != null)
            StopCoroutine(IsZoomCo);
        IsZoomCo = StartCoroutine(IsZoomCoroutine(_zoomDuration));
         
        _animator.SetTrigger("ZoomShoot");
        _playerState = PlayerState.ZoomShot;

        Debug.Log("Shoot !!");
    }

    /// <summary>
    /// ���� �ϴ� �Լ�
    /// </summary>
    public void Zoom()
    {
        _playerState = PlayerState.Zoom;
        // ���� �� ������ �ٲٱ�
        _zoom.enabled = true;
        _zoom.m_Width = 2f;

        _animator.SetBool("Zoom", true);

        // ���� �� ������ �ٲٱ�
        _cinemacine.m_XAxis.m_MaxSpeed = _defaultSensitivity.x * _aimSensiticity; 
        _cinemacine.m_YAxis.m_MaxSpeed = _defaultSensitivity.y * _aimSensiticity;

    }

    /// <summary>
    /// �� �ƿ� �Լ�
    /// </summary>
    public void ExitZoom()
    {
        if (IsZoomCo != null)
            StopCoroutine(IsZoomCo);

        if (IsBattle) // ��Ʋ ���¿��ٸ� ��Ʋ��
        {
            _playerState = PlayerState.Battle;
        }
        else // ���̵� ���¿��ٸ� ���̵��
        {
            IsBattle = false;
            IsZoom = false;
            _playerState = PlayerState.Idle;
        }

        _animator.SetBool("Zoom", false);

        _zoom.enabled = false;

        //���� ������� ��������
        _cinemacine.m_XAxis.m_MaxSpeed = _defaultSensitivity.x * _normalSensitivity;
        _cinemacine.m_YAxis.m_MaxSpeed = _defaultSensitivity.y * _normalSensitivity;
    }

    /// <summary>
    /// ������ ��Ƽ��
    /// </summary>
    /// <param name="val"></param>
    public void CrossHairEnable(bool val)
    {
        _crossHair.enabled = val;
        Image[] crossHairsChildren = null;
        crossHairsChildren = _crossHair.GetComponentsInChildren<Image>();
        foreach (Image e in crossHairsChildren)
        {
            e.enabled = val;
        }
    }

    /// <summary>
    /// ��Ʋ ���� ��ȯ
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator IsBattleCoroutine(float time)
    {
        _isBattle = true;
        _animator.SetBool("IsBattle", _isBattle);
        yield return new WaitForSeconds(time);
        _isBattle = false;
        _animator.SetBool("IsBattle", _isBattle);
        OnIdle?.Invoke();
    }

    /// <summary>
    /// �� ���� ��ȯ
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator IsZoomCoroutine(float time)
    {
        _isZoom = true;
        _animator.SetBool("IsBattle", _isZoom);
        yield return new WaitForSeconds(time);
        _isZoom = false;
        _animator.SetBool("IsBattle", _isZoom);
        OnIdle?.Invoke();
        ExitZoom();
    }

    /// <summary>
    /// �÷��̾� ���� ��ȯ
    /// </summary>
    /// <param name="state"></param>
    public void SetState(PlayerState state)
    {
        _playerState = state;
    }

    public void Dead()
    {
        ExitZoom();
        MonoBehaviour[] monoBehaviours = GetComponents<MonoBehaviour>();
        foreach(var m in monoBehaviours)
        {
            m.StopAllCoroutines();
            m.DOKill();
        }

        IsFreeze = true;
        IsStop = true;
        IsAttackAble = false;
        IsZoomAttackAble = false;
    }

    public void PopUpText(string text, TextMeshProUGUI textMesh, Sequence seq)
    {

        textMesh.enabled = false;
        seq.Kill();

        Vector3 currentPos = textMesh.rectTransform.position;
        textMesh.rectTransform.anchoredPosition = new Vector3(220f, -50f, currentPos.z);

        seq = DOTween.Sequence();

        textMesh.enabled = true;
        seq.Append(textMesh.rectTransform.DOAnchorPosY(textMesh.rectTransform.rect.y, 0.3f).SetEase(Ease.OutBack));
        seq.AppendCallback(() =>
        {
            textMesh.enabled = false;
        });
    }

    private bool _particleDamaged = false; 

    private void OnParticleCollision(GameObject other)
    {
        if (_particleDamaged) return;
        StartCoroutine(ParticleDamageCoroutine());
        Debug.Log("��ƼŬ");
        _playerDamaged.HP -= 2;
    }

    private IEnumerator ParticleDamageCoroutine()
    {
        _particleDamaged = true;
        yield return new WaitForSeconds(0.4f);
        _particleDamaged = false;
    }

    public void Knockback(Vector3 dir)
    {
        transform.Translate(dir * 5f);
    }
}
