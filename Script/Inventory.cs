using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private OkOrCancel theOOC;
    private DatabaseManager theDatabase;
    private OrderManager theOrder;
    private AudioManager theAudio;
    private Equipment theEquip;

    public string key_sound;
    public string enter_sound;
    public string cancel_sound;
    public string open_sound;
    public string beep_sound; // 잘못된 행동 했을 시 사운드

    private InventorySlot[] slots;

    private List<Item> inventoryItemList; // 플레이어가 소지한 아이템 리스트
    private List<Item> inventoryTabList; // 선택된 탭에 따라 다르게 보여질 아이템 리스트

    public Text Description_Text; // 부연설명
    public string[] tabDescription; // 탭 부연설명

    public Transform tf; // slot 부모객체 (grid slot)

    public GameObject go_OOC; // 선택지 활성화 비활성화
    public GameObject go; // 인벤토리 활성화 비활성화
    public GameObject[] selectedTabImage; // 소모 장비 이런 탭들
    private int selectedItem; // 선택된 아이템
    private int selectedTab; // 선택된 텝

    private int page;
    private int slotCount; // 활성화된 슬롯의 개수
    private const int MAX_SLOTS_COUNT = 10; // 최대 슬롯 개수

    private bool activated; // 인벤토리 활성화시 true;
    private bool tabActivated; // 탭 활성화시 true;
    private bool itemActivated; // 아이템 활성화시 true (아이템 방향키로 고를때 탭도 같이 이동 안되게
    private bool stopKeyInput; // 키입력 제한(소비할 때 질의가 나올텐데 그 때 키입력 방지)
    private bool preventExec; // 중복실행 제한

    public GameObject prefab_floating_Text;

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);
    


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        theOOC = FindObjectOfType<OkOrCancel>();
        theAudio = FindObjectOfType<AudioManager>();
        theOrder = FindObjectOfType<OrderManager>();
        theDatabase = FindObjectOfType<DatabaseManager>();
        theEquip = FindObjectOfType<Equipment>();

        inventoryItemList = new List<Item>();
        inventoryTabList = new List<Item>();
        slots = tf.GetComponentsInChildren<InventorySlot>(); // grid slot 밑에 있는 슬롯들이 다 얘한테 들어감. tf가 그리드 슬롯
    }

    public List<Item> SaveItem()
    {
        return inventoryItemList;
    }

    public void LoadItem(List<Item> _itemList)
    {
        inventoryItemList = _itemList;
    }

    public void EquipToInventory(Item _item)
    {
        inventoryItemList.Add(_item);
    }
    public void GetAnItem(int _itemID, int _count = 1)
    {
        for(int i = 0;i < theDatabase.itemList.Count;i++) // 데이터베이스 아이템 검색
        {
            if (_itemID == theDatabase.itemList[i].itemID) // 데이터베이스에 아이템 발견
            {
                var clone = Instantiate(prefab_floating_Text, PlayerManager.instance.transform.position, Quaternion.Euler(Vector3.zero)); // 정확한 형식을 모를 떄 var , instantiate 는 프리펩 생성해서 클론에 넘
                // instantiate(대상,위치,각도)
                clone.GetComponent<FloatingText>().text.text = theDatabase.itemList[i].itemName + " " + _count + "개 획득";
                clone.transform.SetParent(this.transform);

                for(int j = 0;j < inventoryItemList.Count;j++) //소지품에 같은 아이템이 있는지 검색
                {
                    if(inventoryItemList[j].itemID == _itemID) // 소지품에 같은 템이 있으니 개수만 증감
                    {   if (inventoryItemList[j].itemType == Item.ItemType.Use)
                        {
                            inventoryItemList[j].itemCount += _count;
                            return;
                        }
                        else
                        {
                            inventoryItemList.Add(theDatabase.itemList[i]);
                        }
                        return;
                    }
                }
                inventoryItemList.Add(theDatabase.itemList[i]); // 소지품에 해당 아이템 추가.
                inventoryItemList[inventoryItemList.Count - 1].itemCount = _count;
                return;
            }
        }
        Debug.LogError("데이터베이스에 해당 ID값을 가진 아이템이 존재하지 않습니다."); // 데이터베이스에 itemID 없음
    }

    public void ShowTab() // 탭 활성화
    {
        RemoveSlot();
        SelectedTab();
    }

    public void RemoveSlot() // 인벤토리 슬롯 초기화
    {
        for(int i = 0;i < slots.Length;i++)
        {
            slots[i].RemoveItem();
            slots[i].gameObject.SetActive(false);
        }
    }

    public void SelectedTab() // 선택된 탭을 제외하고 다른 모든 텝의 컬러 알파값 0으로 조정.
    {
        StopAllCoroutines();
        Color color = selectedTabImage[selectedTab].GetComponent<Image>().color;
        color.a = 0f;
        for(int i = 0;i < selectedTabImage.Length;i++)
        {
            selectedTabImage[i].GetComponent<Image>().color = color;
        }
        Description_Text.text = tabDescription[selectedTab];
        StartCoroutine(SelectedTabEffectCoroutine());
    }

    IEnumerator SelectedTabEffectCoroutine() //선택된 탭 반짝임 효과
    {
        while(tabActivated)
        {
            Color color = selectedTabImage[selectedTab].GetComponent<Image>().color;
            while(color.a < 0.5f)
            {
                color.a += 0.03f;
                selectedTabImage[selectedTab].GetComponent<Image>().color = color;
                yield return waitTime;
            }
            while (color.a > 0f)
            {
                color.a -= 0.03f;
                selectedTabImage[selectedTab].GetComponent<Image>().color = color;
                yield return waitTime;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    void ShowPage()
    {
        slotCount = -1;
        for (int i = page * MAX_SLOTS_COUNT; i < inventoryTabList.Count; i++) // 인벤토리 탭 리스트의 내용을, 인벤토리 슬롯에 추가
        {
            slotCount = i - (page * MAX_SLOTS_COUNT);
            slots[slotCount].gameObject.SetActive(true);
            slots[slotCount].Additem(inventoryTabList[i]);

            if (slotCount == MAX_SLOTS_COUNT - 1)
                break;
        }
    }


    public void ShowItem() // 아이템 활성화 (inventoryTabList에 조건에 맞는 아이템들만 넣어주고, 인벤토리 슬롯에 출력
    {
        inventoryTabList.Clear();
        RemoveSlot();
        selectedItem = 0;
        page = 0;

        switch (selectedTab) // 탭에 따른 아이템 분류, 그것을 인벤토리 탭 리스트에 추가
        {
            case 0:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Use == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
            case 1:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Equip == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
            case 2:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Quest == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
            case 3:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.ETC == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
        }

        ShowPage();
        SelectedItem();
    }
    public void SelectedItem() // 선택된 아이템을 제외하고, 다른 모든 템의 컬러 알파값을 0으로 조정
    {
        StopAllCoroutines();
        if (slotCount > -1) 
        {
            Color color = slots[0].selected_Item.GetComponent<Image>().color;
            color.a = 0f;
            for (int i = 0; i <= slotCount; i++)
                slots[i].selected_Item.GetComponent<Image>().color = color;

            Description_Text.text = inventoryTabList[selectedItem].itemDescription;
            StartCoroutine(SelectedItemEffectCoroutine());
        }
        else
        {
            Description_Text.text = "해당 타입의 아이템을 소유하고 있지 않습니다.";
        }
    }

    IEnumerator SelectedItemEffectCoroutine() // 선택된 아이템 반짝임 효과
    {
        while (itemActivated)
        {
            Color color = slots[0].GetComponent<Image>().color;
            while (color.a < 0.5f)
            {
                color.a += 0.03f;
                slots[selectedItem].selected_Item.GetComponent<Image>().color = color;
                yield return waitTime;
            }
            while (color.a > 0f)
            {
                color.a -= 0.03f;
                slots[selectedItem].selected_Item.GetComponent<Image>().color = color;
                yield return waitTime;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!stopKeyInput)
        {
            if(Input.GetKeyDown(KeyCode.I))
            {
                activated = !activated;

                if(activated)
                {
                    theAudio.Play(open_sound);
                    theOrder.NotMove();
                    go.SetActive(true);
                    selectedTab = 0;
                    tabActivated = true;
                    itemActivated = false;
                    ShowTab();
                }
                else
                {
                    theAudio.Play(cancel_sound);
                    StopAllCoroutines();
                    go.SetActive(false);
                    tabActivated = false;
                    itemActivated = false;
                    theOrder.Move();
                }
            }

            if(activated)
            {
                if(tabActivated) // 탭 활성화시 키입력 처리
                {
                    if(Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        if (selectedTab < selectedTabImage.Length - 1)
                            selectedTab++;
                        else
                            selectedTab = 0;
                        theAudio.Play(key_sound);
                        SelectedTab();
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        if (selectedTab > 0 )
                            selectedTab--;
                        else
                            selectedTab = selectedTabImage.Length - 1;
                        theAudio.Play(key_sound);
                        SelectedTab();
                    }
                    else if(Input.GetKeyDown(KeyCode.Z))
                    {
                        theAudio.Play(enter_sound);
                        Color color = selectedTabImage[selectedTab].GetComponent<Image>().color;
                        color.a = 0.25f;
                        selectedTabImage[selectedTab].GetComponent<Image>().color = color;
                        itemActivated = true;
                        tabActivated = false;
                        preventExec = true;
                        ShowItem();

                    }
                }

                else if (itemActivated) // 아이템 활성화시 키입력 처리
                {
                    if (inventoryTabList.Count > 0)
                    {
                        if (Input.GetKeyDown(KeyCode.DownArrow))
                        {
                            if(selectedItem + 2 > slotCount)
                            {
                                if (page < (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT)
                                    page++;
                                else
                                    page = 0;

                                RemoveSlot();
                                ShowPage();
                                
                                selectedItem = -2;
                            }

                            if (selectedItem < slotCount - 1)
                                selectedItem += 2;
                            else
                                selectedItem %= 2;
                            theAudio.Play(key_sound);
                            SelectedItem();
                        }
                        else if (Input.GetKeyDown(KeyCode.UpArrow))
                        {
                            if (selectedItem - 2 < 0)
                            {
                                if (page != 0)
                                    page--;
                                else
                                    page = (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT;

                                RemoveSlot();
                                ShowPage();
                            }

                            if (selectedItem > 1)
                                selectedItem -= 2;
                            else
                                selectedItem = slotCount - selectedItem;

                            theAudio.Play(key_sound);
                            SelectedItem();
                        }
                        else if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            if (selectedItem + 1 > slotCount)
                            {
                                if (page < (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT)
                                    page++;
                                else
                                    page = 0;

                                RemoveSlot();
                                ShowPage();
                                selectedItem = -1;
                            }

                            if (selectedItem < slotCount)
                                selectedItem++;
                            else
                                selectedItem = 0;
                            theAudio.Play(key_sound);
                            SelectedItem();
                        }
                        else if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            if (selectedItem - 1 < 0)
                            {
                                if (page != 0)
                                    page--;
                                else
                                    page = (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT;

                                RemoveSlot();
                                ShowPage();
                            }

                            if (selectedItem > 0)
                                selectedItem--;
                            else
                                selectedItem = slotCount;
                            theAudio.Play(key_sound);
                            SelectedItem();
                        }
                        else if (Input.GetKeyDown(KeyCode.Z) && !preventExec)
                        {
                            if (selectedTab == 0)
                            {
                                StartCoroutine(OOCCoroutine("사용", "취소"));
                            }
                            else if (selectedTab == 1)
                            {
                                StartCoroutine(OOCCoroutine("장착", "취소"));
                            }
                            else
                            {
                                theAudio.Play(beep_sound);
                            }
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.X))
                    {
                        theAudio.Play(cancel_sound);
                        StopAllCoroutines();
                        itemActivated = false;
                        tabActivated = true;
                        ShowTab();
                    }
                }

                if (Input.GetKeyUp(KeyCode.Z)) // 중복 실행 방지
                    preventExec = false;
            }
        }
    }

    IEnumerator OOCCoroutine(string _up, string _down)
    {
        theAudio.Play(enter_sound);
        stopKeyInput = true;

        go_OOC.SetActive(true);
        theOOC.ShowTwoChoice(_up, _down);
        yield return new WaitUntil(() => !theOOC.activated);
        if(theOOC.GetResult())
        {
            for(int i = 0;i < inventoryItemList.Count;i++)
            {
                if(inventoryItemList[i].itemID == inventoryTabList[selectedItem].itemID)
                {
                    if(selectedTab == 0)
                    {
                        theDatabase.UseItem(inventoryItemList[i].itemID);
                        if (inventoryItemList[i].itemCount > 1)
                            inventoryItemList[i].itemCount--;
                        else
                            inventoryItemList.RemoveAt(i);

                        //theAudio.Play() 아이템 먹는 소리 출력
                        ShowItem();
                        break;
                    }
                    else if(selectedTab == 1)
                    {
                        theEquip.EquipItem(inventoryItemList[i]);
                        inventoryItemList.RemoveAt(i);
                        ShowItem();
                        break;
                    }

                }
            }
        }
        stopKeyInput = false;
        go_OOC.SetActive(false);
    }
}
