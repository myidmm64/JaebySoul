using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    [SerializeField]
    private CinemachineFreeLook _cmVCam = null;
    [SerializeField]
    [Range(0, 25f)]
    private float _amplitude = 1, _intensity = 1;

    [SerializeField]
    [Range(0, 10f)]
    private float _duration = 0.1f;

    [SerializeField]
    private CinemachineBasicMultiChannelPerlin _noise = null;

    private void OnEnable()
    {
        _noise = _cmVCam.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
    }

    public void CompletePrevFeedBack()
    {
        StopAllCoroutines();
        _noise.m_AmplitudeGain = 0;
    }

    public void CreateFeedBack()
    {
        _noise.m_AmplitudeGain = _amplitude; // ��鸮�� ����
        _noise.m_FrequencyGain = _intensity; // ���� �� ����
        StartCoroutine(ShakeCorutine());
    }
    private IEnumerator ShakeCorutine()
    {
        float time = _duration;
        while (time >= 0)
        {
            _noise.m_AmplitudeGain = Mathf.Lerp(0, _amplitude, time / _duration);
            yield return null;
            time -= Time.deltaTime;
        }
        _noise.m_AmplitudeGain = 0;
    }
}
