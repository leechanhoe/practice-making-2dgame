using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test14 : MonoBehaviour
{
    public Dialouge dialouge_1;
    public Dialouge dialouge_2;

    private DialogueManager TheDM;
    private OrderManager theOrder;
    private PlayerManager thePlayer; // 위를 바라볼때

    private bool flag;
    private FadeManager theFade;

    // Start is called before the first frame update
    void Start()
    {
        theOrder = FindObjectOfType<OrderManager>();
        TheDM = FindObjectOfType<DialogueManager>();
        thePlayer = FindObjectOfType<PlayerManager>();
        theFade = FindObjectOfType<FadeManager>();
    }

    private void OnTriggerStay2D(Collider2D collision) // 콜라이더 안에 있으면 계속 실행되는거 
    {
        if(!flag && Input.GetKey(KeyCode.Z) && thePlayer.animator.GetFloat("DirY") == 1f)
        {
            flag = true;
            StartCoroutine(EventCoroutine());
        }
    }

    IEnumerator EventCoroutine()
    {
        theOrder.PreLoadCharacter();

        theOrder.NotMove();

        TheDM.ShowDialogue(dialouge_1);

        yield return new WaitUntil(() => !TheDM.talking); // 대화가 끝날때까지 기다림

        theOrder.Move("Player", "RIGHT");
        theOrder.Move("Player", "RIGHT");
        theOrder.Move("Player", "UP");// 이거 한번 부를떄마다 큐값 하나 쌓임

        yield return new WaitUntil(() => thePlayer.queue.Count == 0);

        theFade.Flash();
       TheDM.ShowDialogue(dialouge_2);
        yield return new WaitUntil(() => !TheDM.talking);

        theOrder.Move();
    }
}
