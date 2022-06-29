using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//������ �ٲٴ� ��ũ��Ʈ
public class NightChanger : MonoBehaviour
{

    [SerializeField]
    private float _speed = 10f; // ȸ���ӵ�
    [SerializeField]
    private List<GameObject> lights = new List<GameObject>(); // �÷��̾��� ����Ʈ
    private bool _isNight = false; // ���� ���ΰ�?
    public bool IsNight => _isNight;

    [SerializeField]
    private Material _daySkyboxMaterial;
    [SerializeField]
    private Material _nightSkyboxMaterial;

    public Action OnNight;
    public Action OnDay;

    public UnityEvent OnNightBG;
    public UnityEvent OnDayBG;

    private bool _alwaysNight = false;
    public bool AlwaysNight
    {
        set => _alwaysNight = value;
    }

    private void Start()
    {
        OnDay?.Invoke();
        OnDayBG?.Invoke();
    }

    private void Update()
    {
        if(_alwaysNight)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0f, transform.rotation.y, transform.rotation.z));
            transform.rotation *= Quaternion.Euler(new Vector3(260f, 0f, 0f));
            _isNight = true;
            RenderSettings.skybox = _nightSkyboxMaterial;
            return;
        }


        transform.Rotate(Vector3.right, _speed * Time.deltaTime); // ��� ȸ��

        if(transform.eulerAngles.x >= 150f) // ���� ȸ���� �Ѿ�� ��
            if(_isNight == false)
            {
                _isNight = true;
                OnNight?.Invoke();
                OnNightBG?.Invoke();
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
