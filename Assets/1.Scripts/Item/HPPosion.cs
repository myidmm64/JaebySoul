using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPPosion : Item
{
    [SerializeField]
    private PlayerDamaged _playerDamage = null; // HP�� �÷��ֱ� ���� ��ũ��Ʈ ��������
    [SerializeField]
    private TextMeshProUGUI _Pricetext = null; // ���� �ؽ�Ʈ
    private Button _buyButton = null;
    [SerializeField]
    private Image _cantBuyImage = null;
    [SerializeField]
    private int _hpUp = 1; // HP�� �󸶳� �÷��� ������


    private void Awake()
    {
        _buyButton = _Pricetext.transform.parent.GetComponent<Button>();
    }

    private void Start()
    {
        // �ʱ�ȭ
        Count = 0;
        Price = 1;
        _Pricetext.SetText($"���� : {Price}");
    }

    public override void UseItem()
    {
        if(_playerDamage.HP >= _playerDamage.MaxHP)
        {
            return;
        }

        if(Count > 0) // ������ 1�� �̻��� ���� ����
        {
            _playerDamage.HP += _hpUp;
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
        if (Price <= _player.Coin) // �ÿ��̾��� ������ ���� �̻��̶�� ����
        {
            _buyButton.enabled = true;
            _cantBuyImage.enabled = false;

            Count++;
            _player.Coin -= Price;
            Price+= 1;
            Price = Mathf.Clamp(Price, 1, 3);
            _Pricetext.SetText($"���� : {Price}");

            BuyEvent();
        }
    }
}
