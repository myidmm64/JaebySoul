using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Define;
using Random = UnityEngine.Random;

public class SkulMove : MonoBehaviour
{

    // ����
    public enum States
    {
        NONE,
        IDLE,
        MOVE,
        WAIT,
        GOTARGET,
        ATK,
        DAMAGE,
        DIE
    }

    [Header("�⺻�Ӽ�")]
    public States _state = States.NONE; // ������Ʈ �ʱ�ȭ
    public float spdMove = 2f; // �̵��ӵ�
    [SerializeField]
    private GameObject damageObj = null; // �������� �޾��� �� ������ ������Ʈ

    public GameObject targetCharacter = null; // Ÿ��
    public Transform targetTransform = null; 
    public Vector3 posTarget = Vector3.zero; 
    private Animation _animation = null;
    private Transform _transform = null;

    [Header("�ִϸ��̼� Ŭ��")]
    public AnimationClip IdleAnimationClip = null;
    public AnimationClip MoveAnimaitonClip = null;
    public AnimationClip AttackAnimaitonClip = null;
    public AnimationClip DamageAnimationClip = null;
    public AnimationClip DieAnimationClip = null;
    [Header("�����Ӽ�")]
    public int hp = 100; // ü��
    public float AtkRange = 1.5f; // ���� ��Ÿ�
    public GameObject effectDamage = null; // �ǰ� �� ���� ����Ʈ
    public GameObject effectDie = null; // �׾��� �� ���� ����Ʈ
    public float moveRadius = 10f; // �̵� �ݰ�
    [SerializeField]
    private SkinnedMeshRenderer skinnedMeshRenderer = null; // ���� ��Ų �޽�

    [SerializeField]
    private BoxCollider _atkCollider = null; // ���� �ݶ��̴�
    [SerializeField]
    private Collider _col = null; // ������ �ݶ��̴�
    [SerializeField]
    private float attackDistance = 0.3f; // ������ ���� �Ÿ�

    private Action<int> OnDieEvent = null; // �׾��� �� ������ �̺�Ʈ

    private Rigidbody _rigid = null; // ������ٵ� ĳ�� �غ�

    private bool _isAttack = false; // ���� �������ΰ�?

    [SerializeField]
    private GameObject _light = null;

    private int _damage = 1;
    public int damage
    {
        get => _damage;
        set
        {
            _damage = value;
        }
    }

    #region �ִϸ��̼� �̺�Ʈ
    private void OnAtkAnimationFinished()
    {
        Debug.Log("Atk animation Finished");
        _isAttack = false;
        _atkCollider.enabled = false;
    }
    private void OnDamageAnimationFinished()
    {
        skinnedMeshRenderer.material.color = Color.white;
        Debug.Log("Damage animation Finished");
        _isAttack = false;
    }
    private void OnDieAnimationFinished()
    {
        Debug.Log("Die animation Finished");

        Instantiate(effectDie, transform.position + new Vector3(0f, 0.5f, 0f), transform.rotation);
    }
    void OnAnimationEvent(AnimationClip clip, string funcNmae)
    {
        //�̺�Ʈ �������
        AnimationEvent newAnimationEvent = new AnimationEvent();
        //�̺�Ʈ �Լ� ����
        newAnimationEvent.functionName = funcNmae;
        //������ ������ ȣ��
        newAnimationEvent.time = clip.length - 0.2f;
        //�̺�Ʈ �־���
        clip.AddEvent(newAnimationEvent);
    }
    #endregion
    private void Start()
    {
        // �ʱ�ȭ
        _state = States.IDLE;
        // ĳ��
        _transform = GetComponent<Transform>();
        _animation = GetComponent<Animation>();
        _rigid = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();

        // �ִϸ��̼� �ʱ�ȭ
        _animation[IdleAnimationClip.name].wrapMode = WrapMode.Loop;
        _animation[MoveAnimaitonClip.name].wrapMode = WrapMode.Loop;
        _animation[DieAnimationClip.name].wrapMode = WrapMode.Once;
        _animation[DieAnimationClip.name].layer = 5;
        _animation[DamageAnimationClip.name].wrapMode = WrapMode.Once;
        _animation[DamageAnimationClip.name].layer = 5;
        _animation[AttackAnimaitonClip.name].wrapMode = WrapMode.Once;
        _animation[DamageAnimationClip.name].layer = 10;
        _animation[AttackAnimaitonClip.name].speed = 1.5f;

        OnAnimationEvent(AttackAnimaitonClip, "OnAtkAnimationFinished");
        OnAnimationEvent(DamageAnimationClip, "OnDamageAnimationFinished");
        OnAnimationEvent(DieAnimationClip, "OnDieAnimationFinished");

        spdMove = Random.Range(spdMove - 1f, spdMove + 1f); // ���ǵ带 ��������

        OnDieEvent += player.GetComponent<LevelUp>().ExpUp; // �׾��� �� ������ �̺�Ʈ�� ����ġ �߰� �Լ� �ֱ�

        _light = transform.Find("Point Light").gameObject;

        nightChanger.OnNight += NightReset;
        nightChanger.OnDay += DayReset;

        if (nightChanger.IsNight)
            nightChanger.OnNight?.Invoke();
        else
            nightChanger.OnDay?.Invoke();

    }
    private void Update()
    {
        CheckState();
        AnimationCtrl();
    }

    private void StartInit()
    {
        Debug.Log("�ذ� �̴� ����");
    }

    /// <summary>
    /// �ذ� ���¿� ���� ������ �����ϴ� �Լ�
    /// </summary>
    void CheckState()
    {
        switch (_state)
        {
            case States.IDLE:
                _isAttack = false;
                SetIdle();
                break;
            case States.GOTARGET:
            case States.MOVE:
                if(_isAttack == false)
                    SetMove();
                break;
            case States.WAIT:
                break;
            case States.ATK:
                SetAttack();
                break;
            case States.DAMAGE:
                break;
            case States.DIE:
                break;
            default:
                break;

        }
    }
    /// <summary>
    /// ������ ���� �̵� �� �����
    /// </summary>
    /// <returns></returns>
    IEnumerator SetWait()
    {
        //�ذ� ���¸� �����·� ����
        Debug.Log("���");
        _state = States.WAIT;
        //����ϴ� �ð�
        float timeWait = Random.Range(1f, 3f);
        //��� �ð��� �־������
        yield return new WaitForSeconds(timeWait);
        _state = States.IDLE;
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    void SetAttack()
    {
        Vector3 temp = targetTransform.position - transform.position;
        if (temp.magnitude > AtkRange * AtkRange + attackDistance)
        {
            _state = States.GOTARGET;
        }

        float distance = Vector3.Distance(targetTransform.position, _transform.position);

        if (distance > AtkRange + attackDistance)
        {
            _state = States.GOTARGET;
        }
        //to be continue
    }

    /// <summary>
    /// ��ǥ�� ����
    /// </summary>
    /// <param name="target"></param>
    private void OnCkTarget(GameObject target)
    {
        targetCharacter = target;

        targetTransform = targetCharacter.transform;

        _state = States.GOTARGET;
    }

    /// <summary>
    /// ���°� ������ �� ����
    /// </summary>
    void SetMove()
    {
        //����� ������ �� ���� ����
        Vector3 distance = Vector3.zero;
        //����
        Vector3 posLookAt = Vector3.zero;
        switch (_state)
        {
            case States.MOVE:
                //Ÿ�� ���� ��
                if (posTarget != Vector3.zero)
                {
                    //���� ���ϱ�
                    distance = posTarget - _transform.position;
                    //���� distance�� ���̰� range���� ������
                    if (distance.magnitude < AtkRange)
                    {
                        StartCoroutine(SetWait());
                        return;
                    }
                    //���� ���ϱ�
                    posLookAt = new Vector3(posTarget.x, _transform.position.y, posTarget.z);
                }
                break;
            case States.GOTARGET:
                //Ÿ�� ���� ��
                if (targetCharacter != null)
                {
                    //Ÿ�ٰ� ����
                    distance = targetCharacter.transform.position - _transform.position;

                    if (distance.magnitude < AtkRange)
                    {
                        _state = States.ATK;
                        return;
                    }

                    posLookAt = new Vector3(targetCharacter.transform.position.x,
                        _transform.position.y,
                        targetCharacter.transform.position.z
                        );
                }
                break;
            default:
                break;
        }

        Vector3 direction = distance.normalized;

        direction = new Vector3(direction.x, 0f, direction.z);

        Vector3 amount = direction * spdMove * Time.deltaTime;

        _transform.Translate(amount, Space.World);

        _transform.LookAt(posLookAt);
    }

    /// <summary>
    /// ���°� ����� �� ����
    /// </summary>
    void SetIdle()
    {
        if (targetCharacter == null)
        {
            posTarget = new Vector3(_transform.position.x + Random.Range(-moveRadius, moveRadius),
                _transform.position.y + 1000f,
                _transform.position.z + Random.Range(-moveRadius, moveRadius)
                );
            //����ĳ��Ʈ ������ ��ǥ ����
            Ray ray = new Ray(posTarget, Vector3.down);
            //�浹ü ���� ����
            RaycastHit infoRaycast = new RaycastHit();
            //���� �浹ü�� �ֳĸ�
            if (Physics.Raycast(ray, out infoRaycast, Mathf.Infinity))
            {
                //������ ��ǥ ���Ϳ� ���̰��� �߰�
                posTarget.y = infoRaycast.point.y;
            }
            // ���¸� �����
            _state = States.MOVE;
        }
        else
        {

            // ���¸� ��Ÿ������
            _state = States.GOTARGET;
        }
    }
    /// <summary>
    /// �ִϸ��̼� ��� �Լ�
    /// </summary>
    void AnimationCtrl()
    {
        // ���¿� ���� �ִϸ��̼� ����
        switch (_state)
        {
            case States.WAIT:
            case States.IDLE:
                _animation.CrossFade(IdleAnimationClip.name);
                break;
            case States.MOVE:
            case States.GOTARGET:
                if(_isAttack == false)
                    _animation.CrossFade(MoveAnimaitonClip.name);
                break;
            case States.ATK:
                _animation.CrossFade(AttackAnimaitonClip.name);
                _isAttack = true;
                _atkCollider.enabled = true;
                break;
            case States.DIE:
                _animation.CrossFade(DieAnimationClip.name);
                break;
            case States.DAMAGE:
                _animation.CrossFade(IdleAnimationClip.name);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// �ǰ� �Լ�
    /// </summary>
    /// <param name="damage"></param>
    public void Damage(int damage)
    {
        hp -= damage;

        if (hp > 0)
        {
            Instantiate(effectDie, transform.position + new Vector3(0f, 0.5f, 0f), transform.rotation); // �ǰ� ����Ʈ ����

            _animation.CrossFade(DamageAnimationClip.name);

            EffectDamageTween(); // �ǰ� �̺�Ʈ 

            _rigid.AddForce(transform.forward * -1 * 0f, ForceMode.Impulse); // �� �������� �˹�
            Invoke("ResetVelocity", 0.5f); // ���ν�Ƽ �ʱ�ȭ�ϱ�
        }
        else
        {
            if (_state == States.DIE)
                return;
            _state = States.DIE;

            _col.enabled = false; // ���� �ݶ��̴� ���ֱ�
            _atkCollider.enabled = false; // ���� �ݶ��̴� ���ֱ�
            _atkCollider.gameObject.tag = "Untagged";

            OnDieEvent?.Invoke(1); // ����ġ ������Ű��
            OnDieEvent?.Invoke(1); // ����ġ ������Ű��
            player.MonsterCnt++;

            Destroy(gameObject, DieAnimationClip.length - 0.15f); // DIE �ִϸ��̼� ���� �� ��Ʈ����

            GameObject obj = Instantiate(damageObj, transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.Euler(new Vector3(90f, 0f, 0f))); // �׾��� �� ���� ������Ʈ ����
            obj.transform.SetParent(null);
            obj.SetActive(true); 
            obj.transform.DOMoveY(transform.position.y + 1f, 1f);

            nightChanger.OnNight -= NightReset;
            nightChanger.OnDay -= DayReset;
        }
    }
    
    /// <summary>
    /// ������ٵ� �ӵ� �ʱ�ȭ �Լ�
    /// </summary>
    public void ResetVelocity() => _rigid.velocity = Vector3.zero;

    /// <summary>
    /// �¾��� �� ����� �Լ�
    /// </summary>
    private void EffectDamageTween()
    {
        StartCoroutine(Damaged()); // ������ �ٲٴ� �Լ�
    }

    /// <summary>
    /// �¾��� �� ����� �Լ� 2
    /// </summary>
    /// <returns></returns>
    private IEnumerator Damaged()
    {
        for(int i =0; i<4; i++)
        {
            skinnedMeshRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            skinnedMeshRenderer.material.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private int _attackDamage = 1;

    public void NightReset()
    {
        _light.SetActive(true);
        _attackDamage = _atkCollider.GetComponent<SkulAttack>().Damage;
        _atkCollider.GetComponent<SkulAttack>().Damage = 2;
    }
    public void DayReset()
    {
        _light.SetActive(false);
        _atkCollider.GetComponent<SkulAttack>().Damage = _attackDamage;
    }
}
