using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossSpawner : MonoBehaviour
{
    private Player _player = null;
    [SerializeField]
    private GameObject _dragonPrefab = null;
    [SerializeField]
    private Transform _spawnPosition = null;
    [SerializeField]
    private GameObject _effect = null;

    [SerializeField]
    private TextMeshProUGUI _text = null;
    [SerializeField]
    private TextMeshProUGUI _firstText = null;

    private ShakeCamera _shakeCamera = null;

    private bool _spawned = false;


    private void Awake()
    {
        _shakeCamera = GetComponent<ShakeCamera>();     
    }

    private void OnEnable()
    {
        StartCoroutine(TextOn());
    }

    private IEnumerator TextOn()
    {
        _firstText.enabled = true;
        yield return new WaitForSeconds(3f);
        _firstText.enabled = false;
    }

    private void Update()
    {
        if(_player != null)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                if(_player.MonsterCnt >= 0)
                {
                    if (_spawned) return;
                    _spawned = true;
                    //상호작용
                    _shakeCamera.CreateFeedBack();
                    StartCoroutine(SpawnBoss(_player));
                }
            }
        }
    }

    private IEnumerator SpawnBoss(Player player)
    {
        yield return new WaitForSeconds(6f);
        _effect.SetActive(false);
        _dragonPrefab.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_spawned) return;

        if(other.CompareTag("Player"))
        {
            _text.enabled = true;
            _player = other.GetComponent<Player>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _text.enabled = false;
        _player = null;
    }
}
