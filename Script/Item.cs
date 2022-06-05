using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Item 
{
    public int itemID; // 템 고유 id값,중복불가
    public string itemName; // 템이름,중복가능
    public string itemDescription; // 템 설명
    public int itemCount; // 소지 개수
    public Sprite itemIcon;
    public ItemType itemType;

    public enum ItemType // enum = 열거
    {
        Use,
        Equip,
        Quest,
        ETC
    }

    public int atk;
    public int def;
    public int avd; // 회피율
    public int hit;
    public int add_hp;
    public int add_mp;
    public int recover_hp;
    public int recover_mp;

    public Item(int _itemID, string _itemName, string _itemDes, ItemType _itemType, int _atk = 0,
        int _def = 0, int _recover_hp = 0,int _recover_mp = 0, int _itemCount = 1) // 생성자(클래스가 호출만 되도 실행) 리턴값이 없슴 
    {
        itemID = _itemID;
        itemName = _itemName;
        itemDescription = _itemDes;
        itemType = _itemType;
        itemCount = _itemCount;
        itemIcon = Resources.Load("ItemIcon/" + _itemID.ToString(), typeof(Sprite)) as Sprite; // Resources에 있는거 Sprite형태로 가져옴 

        atk = _atk;
        def = _def;
        recover_hp = _recover_hp;
        recover_mp = _recover_mp;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
