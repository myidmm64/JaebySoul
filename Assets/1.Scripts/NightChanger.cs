using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//밤으로 바꾸는 스크립트
public class NightChanger : MonoBehaviour
{

    [SerializeField]
    private float _speed = 10f; // 회전속도
    [SerializeField]
    private List<GameObject> lights = new List<GameObject>(); // 플레이어의 라이트
    private bool _isNight = false; // 현재 밤인가?
    public bool IsNight => _isNight;

    [SerializeField]
    private Material _daySkyboxMaterial;
    [SerializeField]
    private Material _nightSkyboxMaterial;

    public Action OnNight;
    public Action OnDay;

    private bool _alwaysNight = false;
    public bool AlwaysNight
    {
        set => _alwaysNight = value;
    }

    private void Start()
    {
        OnDay?.Invoke();
    }

    private void Update()
    {
        if(_alwaysNight)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0f, transform.rotation.y, transform.rotation.z));
            transform.rotation *= Quaternion.Euler(new Vector3(260f, 0f, 0f));
            _isNight = true;
            RenderSettings.skybox = _nightSkyboxMaterial;
            OnNight?.Invoke();
            return;
        }


        transform.Rotate(Vector3.right, _speed * Time.deltaTime); // 계속 회전

        if(transform.eulerAngles.x >= 150f) // 일정 회전값 넘어가면 밤
            if(_isNight == false)
            {
                _isNight = true;
                OnNight?.Invoke();
                RenderSettings.skybox = _nightSkyboxMaterial;
                for (int i =0; i<lights.Count; i++)
                {
                    lights[i].SetActive(true);
                }
            }

        if ((transform.eulerAngles.x >= 10f) && (transform.eulerAngles.x < 150f))
            if (_isNight == true)
            {
                _isNight = false;
                OnDay?.Invoke();
                RenderSettings.skybox = _daySkyboxMaterial;
                for (int i = 0; i < lights.Count; i++)
                {
                    lights[i].SetActive(false);
                }
            }
    }
}
