using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingCredit : MonoBehaviour
{
    [SerializeField]
    private Image _panelImage = null;
    [SerializeField]
    private TextMeshProUGUI _creditText = null;
    [SerializeField]
    private RectTransform _targetRect = null;

    [SerializeField]
    private TextMeshProUGUI _beforeCredit = null;

    [SerializeField]
    private GameObject _player = null;

    [ContextMenu("Play")]
    public void EndingText()
    {
        _panelImage.enabled = false;
        StartCoroutine(DelayCoroutine());
    }

    private IEnumerator DelayCoroutine()
    {
        _beforeCredit.enabled = true;
        for(int i = 15; i > 0; i--)
        {
            _beforeCredit.SetText($"클리어 축하드립니다 !!!\n{i}초 뒤에 엔딩 크레딧이 나옵니다.");
            yield return new WaitForSeconds(1f);
        }
        _beforeCredit.enabled = false;

        _player.SetActive(false);

        _panelImage.enabled = true;

        _creditText.enabled = true;

        _creditText.rectTransform.DOMove(_targetRect.position, 40f).OnComplete(()=>
        {
            SceneManager.LoadScene("StartScene");
        });
    }

}
