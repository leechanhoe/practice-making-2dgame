using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : MonoBehaviour
{
    public static PlayerStat instance;

    public int character_Lv;
    public int[] needExp;
    public int currentExp;

    public int hp;
    public int currentHp;
    public int mp;
    public int currentMp;

    public int atk;
    public int def;

    public string dmgSound;
    public GameObject prefabs_Floating_text;
    public GameObject parent;

    public int recover_hp; // 초당 회복력
    public int recover_mp;
    public string dmg_sound;

    public float time;
    private float current_time;

    public Slider hpSlider;
    public Slider mpSlider;

    private void Awake() // 파괴방지
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
            Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        current_time = time;
        currentHp = hp;
        currentMp = mp;
    }

    public void Hit(int _enemyAtk)
    {
        int dmg;

        if (def >= _enemyAtk)
            dmg = 1;
        else
            dmg = _enemyAtk - def;

        currentHp -= dmg;

        if (currentHp <= 0)
            Debug.Log("체력 0 이하");

        AudioManager.instance.Play(dmgSound);

        Vector3 vector = this.transform.position;
        vector.y += 60;

        GameObject clone = Instantiate(prefabs_Floating_text, vector, Quaternion.Euler(Vector3.zero)); // instantiate 함수는 prefab을 생성하는거
        clone.GetComponent<FloatingText>().text.text = dmg.ToString();
        clone.GetComponent<FloatingText>().text.color = Color.red;
        clone.GetComponent<FloatingText>().text.fontSize = 25;
        clone.transform.SetParent(parent.transform);
        StopAllCoroutines();
        StartCoroutine(HitCoroutine());
    }

    IEnumerator HitCoroutine()
    {
        Color color = GetComponent<SpriteRenderer>().color;
        color.a = 0;
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.1f);
        color.a = 1f;
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.1f);
        color.a = 0f;
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.1f);
        color.a = 1f;
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.1f);
        color.a = 0f;
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.1f);
        color.a = 1f; 
        GetComponent<SpriteRenderer>().color = color;
    }

        // Update is called once per frame
    void Update()
    {
        hpSlider.maxValue = hp;
        mpSlider.maxValue = mp;

        hpSlider.value = currentHp;
        mpSlider.value = currentMp;

        if(currentExp >= needExp[character_Lv])
        {
            character_Lv++;
            hp += character_Lv * 2;
            mp += character_Lv + 2;

            currentHp = hp;
            currentMp = mp;
            atk++;
            def++;
        }
        current_time -= Time.deltaTime;

        if(current_time <= 0)
        {
            if(recover_hp > 0)
            {
                if (currentHp + recover_hp <= hp)
                    currentHp += recover_hp;
                else
                    currentHp = hp;
            }
            current_time = time;
        }
    }
}
