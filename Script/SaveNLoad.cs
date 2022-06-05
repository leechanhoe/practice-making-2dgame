using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; // 형식변환기
using UnityEngine.SceneManagement;

public class SaveNLoad : MonoBehaviour
{
    [System.Serializable] // 세이브와 로드 기능에서 필수 , 직렬화임 컴터입장에서 읽고 쓰기 쉬움
    public class Data
    {
        public float playerX;
        public float playerY;
        public float playerZ;

        public int playerLv;
        public int playerHp;
        public int playerMp;

        public int playerCurrentHp;
        public int playerCurrentMp;
        public int playerCurrentExp;

        public int playerHpr;
        public int playerMpr;

        public int playerAtk;
        public int playerDef;

        public int added_atk;
        public int added_def;
        public int added_hpr;
        public int added_mpr;

        public List<int> playerItemInventory;
        public List<int> playerItemInventoryCount;
        public List<int> playerEquipItem;

        public string mapName;
        public string sceneName;
        public List<bool> swList;
        public List<string> swNameList;
        public List<string> varNameList;
        public List<float> varNumberList;
    }

    private PlayerManager thePlayer;
    private PlayerStat thePlayerStat;
    private DatabaseManager theDatabase;
    private Inventory theInven;
    private Equipment theEquip;
    private FadeManager theFade;

    public Data data;

    private Vector3 vector;

    public void CallSave()
    {
        theDatabase = FindObjectOfType<DatabaseManager>();
        thePlayer = FindObjectOfType<PlayerManager>();
        thePlayerStat = FindObjectOfType<PlayerStat>();
        theEquip = FindObjectOfType<Equipment>();
        theInven = FindObjectOfType<Inventory>();
        //몬스터는 무빙오브젝트로 한번에

        data.playerX = thePlayer.transform.position.x;
        data.playerY = thePlayer.transform.position.y;
        data.playerZ = thePlayer.transform.position.z;

        data.playerLv = thePlayerStat.character_Lv;
        data.playerHp = thePlayerStat.hp;
        data.playerMp = thePlayerStat.mp;
        data.playerCurrentHp = thePlayerStat.currentHp;
        data.playerCurrentMp = thePlayerStat.currentMp;
        data.playerCurrentExp = thePlayerStat.currentExp;
        data.playerAtk = thePlayerStat.atk;
        data.playerDef = thePlayerStat.def;
        data.playerHpr = thePlayerStat.recover_hp;
        data.playerMpr = thePlayerStat.recover_mp;
        data.added_atk = theEquip.added_atk;
        data.added_def = theEquip.added_def;
        data.added_hpr = theEquip.added_hpr;
        data.added_mpr = theEquip.added_mpr;

        data.mapName = thePlayer.currentMapName;
        data.sceneName = thePlayer.currentSceneName;
        Debug.Log("기초 데이터 입력 성공");

        data.playerItemInventory.Clear();
        data.playerItemInventoryCount.Clear();
        data.playerEquipItem.Clear();

        for(int i = 0;i < theDatabase.var_name.Length;i++)
        {
            data.varNameList.Add(theDatabase.var_name[i]);
            data.varNumberList.Add(theDatabase.var[i]);
        }
        for (int i = 0; i < theDatabase.switch_name.Length; i++)
        {
            data.swNameList.Add(theDatabase.switch_name[i]);
            data.swList.Add(theDatabase.switches[i]);
        }

        List<Item> itemList = theInven.SaveItem();

        for (int i = 0;i < itemList.Count;i++)
        {
            Debug.Log("인벤토리 아이템 저장 완료" + itemList[i].itemID);
            data.playerItemInventory.Add(itemList[i].itemID);
            data.playerItemInventoryCount.Add(itemList[i].itemCount);
        }

        for(int i = 0;i < theEquip.equipItemList.Length;i++)
        {
            data.playerEquipItem.Add(theEquip.equipItemList[i].itemID);
            Debug.Log("장착된 아이템 저장 완료: " + theEquip.equipItemList[i].itemID);
        }

        BinaryFormatter bf = new BinaryFormatter(); // 변환하는거
        FileStream file = File.Create(Application.dataPath + "/SaveFile.dat"); // 입출력하는거 (경로(에셋폴더),파일이름(확장자명은 내맘대로))

        bf.Serialize(file, data); // 파일에 기록하고 직렬화
        file.Close();

        Debug.Log(Application.dataPath + "의 위치에 저장했습니다.");
    }

    public void CallLoad()
    {
        BinaryFormatter bf = new BinaryFormatter(); // 변환하는거
        FileStream file = File.Open(Application.dataPath + "/SaveFile.dat", FileMode.Open); // 입출력하는거 (경로(에셋폴더),파일이름(확장자명은 내맘대로))

        if(file != null && file.Length > 0)
        {
            data = (Data)bf.Deserialize(file);

            theDatabase = FindObjectOfType<DatabaseManager>();
            thePlayer = FindObjectOfType<PlayerManager>();
            thePlayerStat = FindObjectOfType<PlayerStat>();
            theEquip = FindObjectOfType<Equipment>();
            theInven = FindObjectOfType<Inventory>();
            theFade = FindObjectOfType<FadeManager>();

            theFade.FadeOut();

            thePlayer.currentMapName = data.mapName;
            thePlayer.currentSceneName = data.sceneName;

            vector.Set(data.playerX, data.playerY, data.playerZ);
            thePlayer.transform.position = vector;

            thePlayerStat.character_Lv = data.playerLv;
            thePlayerStat.hp = data.playerHp;
            thePlayerStat.mp = data.playerMp;
            thePlayerStat.currentHp = data.playerCurrentHp;
            thePlayerStat.currentMp = data.playerCurrentMp;
            thePlayerStat.currentExp = data.playerCurrentExp;
            thePlayerStat.atk = data.playerAtk;
            thePlayerStat.def = data.playerDef;
            thePlayerStat.recover_hp = data.playerHpr;
            thePlayerStat.recover_mp = data.playerMpr;

            theEquip.added_atk = data.added_atk;
            theEquip.added_def = data.added_def;
            theEquip.added_hpr = data.added_hpr;
            theEquip.added_mpr = data.added_mpr;

            theDatabase.var = data.varNumberList.ToArray(); // 배열로 바꾸면 한번에 넣기 가능
            theDatabase.var_name = data.varNameList.ToArray();
            theDatabase.switches = data.swList.ToArray();
            theDatabase.switch_name = data.swNameList.ToArray();

            for(int i = 0;i < theEquip.equipItemList.Length;i++)
            {
                for(int x = 0;x < theDatabase.itemList.Count; x++)
                {
                    if(data.playerEquipItem[i] == theDatabase.itemList[x].itemID)
                    {
                        theEquip.equipItemList[i] = theDatabase.itemList[x];
                        Debug.Log("장착된 아이템을 로드했습니다." + theEquip.equipItemList[i].itemID);
                        break;
                    }
                } 
            }

            List<Item> itemList = new List<Item>();

            for (int i = 0; i < data.playerItemInventory.Count; i++)
            {
                for (int x = 0; x < theDatabase.itemList.Count; x++)
                {
                    if (data.playerItemInventory[i] == theDatabase.itemList[x].itemID)
                    {
                        itemList.Add(theDatabase.itemList[x]);
                        Debug.Log("인벤토리 아이템을 로드했습니다." + theDatabase.itemList[x].itemID);
                        break;
                    }
                }
            }
            for (int i = 0;i < data.playerItemInventoryCount.Count;i++)
            {
                itemList[i].itemCount = data.playerItemInventoryCount[i];
            }

            theInven.LoadItem(itemList);
            theEquip.ShowText(); // 이거안하면 8 (+3) 안뜸

            StartCoroutine(WaitCoroutine());
        }
        else
        {
            Debug.Log("저장된 세이브 파일이 없습니다.");
        }
        file.Close();
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(2f);

        GameManager theGM = FindObjectOfType<GameManager>();
        theGM.LoadStart();

        SceneManager.LoadScene(data.sceneName);
        thePlayer.transform.position = vector;
    }
}
