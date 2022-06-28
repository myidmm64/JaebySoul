using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MPPosion : Item
{
    [SerializeField]
    private PlayerUseSkill _playerUseSkill = null; // MP�� �÷��ֱ� ���� ��ũ��Ʈ ��������
    [SerializeField]
    private TextMeshProUGUI _Pricetext = null; // ���� �ؽ�Ʈ
    private Button _buyButton = null;
    [SerializeField]
    private Image _cantBuyImage = null;
    [SerializeField]
    private int _mpUp = 1; // MP�� �󸶳� �÷��� ������

    private void Awake()
    {
        _buyButton = _Pricetext.transform.parent.GetComponent<Button>();
    }

    private void Start()
    {
        //�ʱ�ȭ
        Count = 0;
        Price = 2;
        _Pricetext.SetText($"���� : {Price}");
    }

    public override void UseItem()
    {
        if(_playerUseSkill.MP >= _playerUseSkill.MaxMP)
        {
            return;
        }

        if (Count > 0) // ������ 1 �̻��϶��� ����
        {
            _playerUseSkill.MP += _mpUp;
            Count--;
        }
    }

    public void MarketInit()
    {
        if (Price <= _player.Coin)
        {
            _buyButton.enabled = true;
            _cantBuyImage.enabled = false;
        }
        else
        {
            _buyButton.enabled = false;
            _cantBuyImage.enabled = true;
        }
    }


    public void BuyEvent()
    {
        if (Price > _player.Coin)
        {
            _buyButton.enabled = false;
            _cantBuyImage.enabled = true;
        }
    }

    public override void Buy()
    {
        if(Price <= _player.Coin) // ���� �÷��̾��� ������ ���ݺ��� ������
        {
            _buyButton.enabled = true;
            _cantBuyImage.enabled = false;

            Count++;
            _player.Coin -= Price;
            _Pricetext.SetText($"���� : {Price}");

            BuyEvent();
        }
    }
}
