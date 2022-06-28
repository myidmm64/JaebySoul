using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRun : MonoBehaviour
{
    private Player _player = null; // �÷��̾� ĳ�� �غ�
    private PlayerZoomShoot _playerZoomShot = null;
    private Animator _animator = null; // �ִϸ����� ĳ�� �غ�

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _player = GetComponent<Player>();
        _playerZoomShot = GetComponent<PlayerZoomShoot>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) // ���� ����Ʈ�� ���������� �� �޸���
        {
            _player.OnIdle?.Invoke();
            _player.SetState(Player.PlayerState.Run);
            _player.IsRun = true;
            _animator.SetBool("IsRun", true);
            _player.ExitZoom();
            _playerZoomShot.ZoomOut();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _player.SetState(Player.PlayerState.Idle);
            _player.IsRun = false;
            _animator.SetBool("IsRun", false);
        }
    }
}
