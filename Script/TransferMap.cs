using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferMap : MonoBehaviour
{

    public string transferMapName;
    PlayerManager thePlayer;
    CameraManager theCamera;

    public Transform target;
    public BoxCollider2D targetBound;

    public bool flag;
    private FadeManager theFade;
    private OrderManager theOrder;

    public Animator anim_1;
    public Animator anim_2;

    public int door_count;

    [Tooltip("UP, DOWN, LEFT, RIGHT")]
    public string direction; // 캐릭이 바라보는 방향
    private Vector2 vector; // getfloat("DirX")

    [Tooltip("문이 열린다:true, 문이 없으면 false")]
    public bool door; // 문이 있냐 없냐 체크

    void Start()
    {   if (!flag)
            theCamera = FindObjectOfType<CameraManager>();
        thePlayer = FindObjectOfType<PlayerManager>();
        theFade = FindObjectOfType<FadeManager>();
        theOrder = FindObjectOfType<OrderManager>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!door)
        {
            if (collision.gameObject.name == "Player")
            {
                if (flag)
                {
                    thePlayer.currentMapName = transferMapName;
                    SceneManager.LoadScene(transferMapName);
                }
                else
                {
                    StartCoroutine(TransperCoroutine());
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(door)
        {
            if (collision.gameObject.name == "Player")
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    vector.Set(thePlayer.animator.GetFloat("DirX"), thePlayer.animator.GetFloat("DirY"));
                    switch (direction)
                    {
                        case "UP":
                            if (vector.y == 1f)
                                StartCoroutine(TransperCoroutine());
                            break;
                        case "DOWN":
                            if (vector.y == -1f)
                                StartCoroutine(TransperCoroutine());
                            break;
                        case "RIGHT":
                            if (vector.x == 1f)
                                StartCoroutine(TransperCoroutine());
                            break;
                        case "LEFT":
                            if (vector.x == -1f)
                                StartCoroutine(TransperCoroutine());
                            break;
                        default:
                            StartCoroutine(TransperCoroutine());
                            break;
                    }
                }
            }
        }
    }

    IEnumerator TransperCoroutine()
    {
        theOrder.PreLoadCharacter();
        theOrder.NotMove();
        theFade.FadeOut();
        if (door)
        {
            anim_1.SetBool("Open", true);
            if (door_count == 2)
                anim_2.SetBool("Open", true);
        }

        yield return new WaitForSeconds(0.5f);

        theOrder.SetUnTransparent("Player");

        if (door)
        {
            anim_1.SetBool("Open", false);
            if (door_count == 2)
                anim_2.SetBool("Open", false);
        }
        yield return new WaitForSeconds(0.5f);
        thePlayer.currentMapName = transferMapName;
        thePlayer.transform.position = target.transform.position;
        theCamera.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, theCamera.transform.position.z);
        theCamera.SetBound(targetBound);
        theFade.FadeIn();
        yield return new WaitForSeconds(0.5f);
        theOrder.Move();
    }
}
