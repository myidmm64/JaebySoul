using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioMixer _mixer;

    private bool _isBGFull = true;
    private bool _isEffectFull = true;

    public void BGVol()
    {
        if (_isBGFull)
        {
            _isBGFull = false;
            _mixer.SetFloat("_BG", -80f);
        }
        else
        {
            _isBGFull = true;
            _mixer.SetFloat("_BG", 0f);
        }
    }

    public void EffectVol()
    {
        if (_isEffectFull)
        {
            _isEffectFull = false;
            _mixer.SetFloat("_Effect", -80f);
        }
        else
        {
            _isEffectFull = true;
            _mixer.SetFloat("_Effect", 0f);
        }
    }
}
