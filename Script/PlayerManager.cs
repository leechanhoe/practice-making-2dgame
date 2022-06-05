using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MovingObject
{
    static public PlayerManager instance;

    public string currentMapName;
    public string currentSceneName;

    public float runSpeed;
    private float applyRunSpeed;
    private bool applyRunFlag = false;

    public string walkSound_1;
    public string walkSound_2;
    public string walkSound_3;
    public string walkSound_4;

    private AudioManager theAudio;
    private SaveNLoad thesaveNLoad;

    private bool canMove = true;

    public bool notMove = false;

    private bool attacking = false;
    public float attackDelay;
    private float currentAttackDelay;

    void Start()
    {
        // 맵이동하면 캐릭터 중복으로 생기는거 방지
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            queue = new Queue<string>();
            DontDestroyOnLoad(this.gameObject);
            animator = GetComponent<Animator>();
            boxCollider = GetComponent<BoxCollider2D>();
            theAudio = FindObjectOfType<AudioManager>();
            thesaveNLoad = FindObjectOfType<SaveNLoad>();
            instance = this;
        }
    }

    IEnumerator MoveCoroutine()
    {
        while (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0 && !notMove && !attacking)
        {
            //대쉬
            if (Input.GetKey(KeyCode.LeftShift))
            {
                applyRunSpeed = runSpeed;
                applyRunFlag = true;
            }
            else
            {
                applyRunSpeed = 0;
                applyRunFlag = false;
            }

            vector.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), transform.position.z);
            //버튼 두개 동시에 누를때 생기는 오류막음
            if (vector.x != 0)
                vector.y = 0;

            animator.SetFloat("DirX", vector.x);
            animator.SetFloat("DirY", vector.y);

            bool checkCollisionFlag = base.CheckCollision();
            if (checkCollisionFlag)
                break;

            animator.SetBool("Walking", true);


            int temp = Random.Range(1, 4);
            switch (temp)
            {
                case 1:
                    theAudio.Play(walkSound_1);
                    break;
                case 2:
                    theAudio.Play(walkSound_2);
                    break;
                case 3:
                    theAudio.Play(walkSound_3);
                    break;
                case 4:
                    theAudio.Play(walkSound_4);
                    break;
            }
            //theAudio.SetVolumn(walkSound_2, 0.5f); 볼륨수정

            boxCollider.offset = new Vector2(vector.x * 0.8f * speed * walkCount, vector.y * 0.8f * speed * walkCount);

            //1칸씩 움직이게 하기위함
            while (currentWalkCount < walkCount)
            {
                if (vector.x != 0)
                {
                    transform.Translate(vector.x * (speed + applyRunSpeed), 0, 0);
                }
                else if (vector.y != 0)
                {
                    transform.Translate(0, vector.y * (speed + applyRunSpeed), 0);
                }
                if (applyRunFlag)
                    currentWalkCount++;
                currentWalkCount++;
                if (currentWalkCount == 6)
                    boxCollider.offset = Vector2.zero;
                yield return new WaitForSeconds(0.01f);
            }
            currentWalkCount = 0;
        }
        animator.SetBool("Walking", false);
        canMove = true;
    }

    void Update()
    {   

        //한번에 한칸씩만 움직이게 하기위함
        if (canMove && !notMove && !attacking)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                canMove = false;
                StartCoroutine(MoveCoroutine());
            }
        }

        if(!notMove && !attacking)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                currentAttackDelay = attackDelay;
                attacking = true;
                animator.SetBool("Attacking", true);
            }
        }

        if(attacking)
        {
            currentAttackDelay -= Time.deltaTime;
            if(currentAttackDelay <= 0)
            {
                animator.SetBool("Attacking", false);
                attacking = false;
            }
        }
    }
}
