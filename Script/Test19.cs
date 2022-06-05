using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test19 : MonoBehaviour
{
    [SerializeField]
    public Choice choice;

    private OrderManager theOrder;
    public bool flag;
    private ChoiceManager2 theChoice;

    // Start is called before the first frame update
    void Start()
    {
        theOrder = FindObjectOfType<OrderManager>();
        theChoice = FindObjectOfType<ChoiceManager2>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!flag)
        {
            StartCoroutine(ACoroutine());
        }
    }

    IEnumerator ACoroutine()
    {
        flag = true;
        theOrder.NotMove();
        theChoice.ShowChoice(choice);
        yield return new WaitUntil(() => !theChoice.choiceIng);
        theOrder.Move();
        Debug.LogWarning(theChoice.GetResult());
    }
}
