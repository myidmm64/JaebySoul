using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerZoomShoot : MonoBehaviour
{
    [SerializeField]
    private GameObject _bullet = null; // 총알
    [SerializeField]
    private Transform _shootBulletPosition; // 총알이 나올 위치
    [SerializeField]
    private CinemachineFreeLook _virtualCamera; // 평상시의 버츄얼 카메라


    [SerializeField]
    private float _aimRange = 100f; // 에임을 할 수 있는 사정거리
    [SerializeField]
    private float _zoomAttackDelay = 1f; // 줌 샷 딜레이
    [SerializeField]
    private LayerMask _aimColliderLayerMask = new LayerMask(); // 에임이 딱 갈 수 있는 레이어
    [SerializeField]
    private Transform _debugTrasnform = null; // 에임위치 디버그용

    private bool _isZoomAttacking = false;

    private Player _player = null; // 플레이어

    Vector3 _mouseWorldPosition = Vector3.zero; // 마우스의 포지션


    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    /// <summary>
    /// 줌을 시작했을 때 실행될 시점 이벤튼
    /// </summary>
    public void ZoomIn()
    {
        // 카메라 시점 어깨로 바꾸기
        _virtualCamera.GetRig(0).GetCinemachineComponent<CinemachineComposer>().m_ScreenX = 0.33f;
        _virtualCamera.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_ScreenX = 0.33f;
        _virtualCamera.GetRig(2).GetCinemachineComponent<CinemachineComposer>().m_ScreenX = 0.33f;
    }

    /// <summary>
    /// 줌을 끝냈을 때 실행될 시점 이벤튼
    /// </summary>
    public void ZoomOut()
    {
        // 카메라 돌려놓기
        _virtualCamera.GetRig(0).GetCinemachineComponent<CinemachineComposer>().m_ScreenX = 0.5f;
        _virtualCamera.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_ScreenX = 0.5f;
        _virtualCamera.GetRig(2).GetCinemachineComponent<CinemachineComposer>().m_ScreenX = 0.5f;
    }
    
    /// <summary>
    /// 줌 샷 딜레이 함수 실행용
    /// </summary>
    public void ZoomShootDelay()
    {
        if (_isZoomAttacking)
            return;

        StartCoroutine(ZoomShootDelayCoroutine());
    }

    /// <summary>
    /// 줌 샷 딜레이
    /// </summary>
    /// <returns></returns>
    private IEnumerator ZoomShootDelayCoroutine()
    {
        _player.IsZoomAttackAble = false;
        Debug.Log("줌 불가");
        _isZoomAttacking = true;
        yield return new WaitForSeconds(_zoomAttackDelay);
        _player.IsZoomAttackAble = true;
        _isZoomAttacking = false;
        Debug.Log("줌 ㄱㄴ");
    }


    private void Update()
    {
        if (_player.IsZoom == false) // 만약에 줌을 하고있지 않으면 리턴
            return;

        _mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f); // 화면의 정중앙
        Ray ray = MainCam.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, _aimRange, _aimColliderLayerMask)) // 현재 위치에서 화면 정중앙으로 레이캐스트 발사
        {
            Debug.Log(raycastHit.collider.gameObject.name);
            //_debugTrasnform.position = raycastHit.point; 디버그용
            _mouseWorldPosition = raycastHit.point;
        }

        /*Vector3 worldAimTartget = _mouseWorldPosition;
        worldAimTartget.y = transform.position.y;
        Vector3 aimDirection = (worldAimTartget - transform.position).normalized;*/
    }

    /// <summary>
    /// 불렛을 소환
    /// </summary>
    public void ShootBullet()
    {
        Vector3 aimDir = (_mouseWorldPosition - _shootBulletPosition.position).normalized;

        Instantiate(_bullet, _shootBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));

    }
}
