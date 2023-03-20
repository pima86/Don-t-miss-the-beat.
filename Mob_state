using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Mob_state : MonoBehaviour
{
    public Animator heat_anim;

    public Animator main_anim;
    public RuntimeAnimatorController idle_anim;

    [Header("몹")]
    [SerializeField] SpriteRenderer main;
    [SerializeField] Sprite[] mob_sprite;
    [SerializeField] AudioClip Dead_clip;
    [SerializeField] AudioClip Heat_clip;

    [Header("몹 레벨")]
    [SerializeField] SpriteRenderer[] Number;
    [SerializeField] Sprite[] Number_sprite;

    [Header("몹 레벨")]
    [SerializeField] GameObject[] Heat_line;
    #region LV
    public int lv = 10;
    public int LV
    {
        set
        {
            if (value > 999)
                value = 999;

            if (value > 9)
            {
                if (value > 99)
                {
                    Number[0].enabled = true;
                    Number[1].enabled = true;
                    Number[2].enabled = true;

                    Number[3].enabled = false;
                    Number[4].enabled = false;

                    Number[0].sprite = Number_sprite[(value % 1000) / 100];
                    Number[1].sprite = Number_sprite[(value % 100) / 10];
                    Number[2].sprite = Number_sprite[(value % 10)];
                }
                else
                {
                    Number[0].enabled = false;
                    Number[1].enabled = false;
                    Number[2].enabled = false;

                    Number[3].enabled = true;
                    Number[4].enabled = true;

                    Number[3].sprite = Number_sprite[(value % 100) / 10];
                    Number[4].sprite = Number_sprite[(value % 10)];
                }
            }
            else
            {
                Number[0].enabled = false;
                Number[1].enabled = true;
                Number[2].enabled = false;

                Number[3].enabled = false;
                Number[4].enabled = false;

                Number[1].sprite = Number_sprite[(value % 10)];
            }

            //글자 색갈
            Color color = new Color();
            if (value > Player_Move.Inst.LV)
                color = new Color(1, 0, 0);
            else
                color = new Color(1, 1, 1);

            for (int i = 0; i < Number.Length; i++)
            {
                Number[i].color = color;
            }

            lv = value;
        }
        get
        {
            return lv;
        }
    }
    #endregion

    private void Update()
    {
        Move();
        //Eat();
    }

    #region 셋업이랑 히트
    //안면 가져와!!
    public void SetUp()
    {
        LV = LV;

        main_anim.runtimeAnimatorController = null;

        if (LV <= 10)
            main.sprite = mob_sprite[0];
        else if (LV >= 11 && LV <= 30)
            main.sprite = mob_sprite[1];
        else if (LV >= 31 && LV <= 50)
            main.sprite = mob_sprite[2];
        else if (LV >= 51 && LV <= 100)
            main.sprite = mob_sprite[3];
        else if (LV >= 101 && LV <= 150)
            main.sprite = mob_sprite[4];
        else if (LV >= 151 && LV <= 200)
            main.sprite = mob_sprite[5];
        else if (LV >= 201 && LV <= 250)
            main.sprite = mob_sprite[6];
        else if (LV >= 251 && LV <= 300)
            main.sprite = mob_sprite[7];
        else if (LV >= 301)
            main.sprite = mob_sprite[8];

        main_anim.runtimeAnimatorController = idle_anim;
    }

    //개같이 맞았어!!
    public void Heat()
    {
        if (Player_Move.Inst.LV >= LV)
        {
            if(false)
                Player_Move.Inst.level_up(Player_Move.Inst.LV + LV);

            Mob_spawn.Inst.mob_Destroy(gameObject);
        }
        else if (Player_Move.Inst.LV < LV)
            Player_Move.Inst.HP -= 0.5f;

        SoundManager.Inst.SFXPlay("Dead", Dead_clip);
        heat_anim.Play("effect_1", -1, 0);
    }

    public void Heat_Line_Check()
    {
        Vector3 pos = new Vector3();

        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0: //동
                    pos = new Vector3(1f, 0.5f, 0);
                    break;
                case 1: //서
                    pos = new Vector3(-1f, 0.5f, 0);
                    break;
                case 2: //남
                    pos = new Vector3(0, -0.5f, 0);
                    break;
                case 3: //북
                    pos = new Vector3(0, 1.5f, 0);
                    break;
            }


            if (lv > Player_Move.Inst.LV)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.localPosition + pos, transform.forward, 1, LayerMask.GetMask("Object"));
                RaycastHit2D hit_move = Physics2D.Raycast(transform.localPosition + pos, transform.forward, 1, LayerMask.GetMask("Move_Layer"));
                
                if (hit.collider == null && hit_move.collider != null)
                    Heat_line[i].SetActive(true);
                else if (hit.collider != null)
                {
                    if (hit.collider.tag == "Item")
                        Heat_line[i].SetActive(true);
                    else
                        Heat_line[i].SetActive(false);
                }
                else
                    Heat_line[i].SetActive(false);
            }
            else
                Heat_line[i].SetActive(false);
        }
    }
    #endregion
    #region 이동
    bool Move_stay = false;
    void Move()
    {
        if (Player_Move.Inst.HP == 0)
            return;

        //BitManager.Inst.mob_bouns && 
        if (!BitManager.Inst.obj_bouns)
        {
            if (!Move_stay)
            {
                LV++;
                SetUp();

                //Move_To_Player();
                Heat_Line_Check();
                Move_stay = true;
            }
        }
        else
            Move_stay = false;
    }
    void Eat()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.localPosition, transform.forward, 1, LayerMask.GetMask("Object"));
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Mob")
            {
                if (hit.collider.gameObject != gameObject)
                    Mob_spawn.Inst.mob_Evole(hit.collider.gameObject, gameObject);
            }
            /*
            else if (hit.collider.tag == "Item")
                Destroy(hit.collider.gameObject);
            */
        }
    }

    Vector3 mob_pos;
    
    public void Move_To_Player()
    {
        Vector3 player_pos = Player_Move.Inst.main_point.position;
        mob_pos = transform.localPosition;

        int x_point; //몬스터가 오른쪽에 있는가?
        if (mob_pos.x > player_pos.x)
            x_point = 1;
        else if (mob_pos.x == player_pos.x)
            x_point = 0;
        else
            x_point = -1;

        int y_point; //몬스터가 위에 있는가?
        if (mob_pos.y > player_pos.y)
            y_point = 1;
        else if (mob_pos.y == player_pos.y)
            y_point = 0;
        else
            y_point = -1;

        Vector3 pos = new Vector3();
        //가로 이동할지 세로 이동할지
        int num = 0;


        #region 랜덤 이동
        num = Random.Range(0, 2);
        for (int i = 0; i < 2; i++)
        {
            if (num == 0)
            {
                if (y_point == 1)
                {
                    num = 2;
                    break;
                }
                else if (y_point == -1)
                {
                    num = 3;
                    break;
                }
            }
            else
            {
                if (x_point == 1)
                {
                    num = 0;
                    break;
                }
                else if (x_point == -1)
                {
                    num = 1;
                    break;
                }
            }

            switch (num)
            {
                case 0:
                    num = 1;
                    break;
                case 1:
                    num = 0;
                    break;
            }
        }
        #endregion
        switch (num)
        {
            case 0: //동
                pos = new Vector3(-1f, 0, 0);
                break;
            case 1: //서
                pos = new Vector3(1f, 0, 0);
                break;
            case 2: //남
                pos = new Vector3(0, -0.5f, 0);
                break;
            case 3: //북
                pos = new Vector3(0, 1.5f, 0);
                break;
        }

        RaycastHit2D hit = Physics2D.Raycast(mob_pos + pos, transform.forward, 1, LayerMask.GetMask("Spawn_Layer"));
        RaycastHit2D hit2 = Physics2D.Raycast(mob_pos + pos, transform.forward, 1, LayerMask.GetMask("Object"));
        RaycastHit2D hit3 = Physics2D.Raycast(mob_pos + pos, transform.forward, 1, LayerMask.GetMask("Player"));
        if (hit2.collider != null)
        {
            //if (hit2.collider.tag == "Box" || hit2.collider.tag == "Item")
                return;
        }

        if (hit3.collider != null)
        {
            if (hit3.collider.tag == "Player")
                return;
        }

        if (hit.collider != null)
        {
            switch (num)
            {
                case 0: //동
                    transform.localPosition += new Vector3(-1f, 0, 0);
                    main.flipX = true;
                    break;
                case 1: //서
                    transform.localPosition += new Vector3(1f, 0, 0);
                    main.flipX = false;
                    break;
                case 2: //남
                    transform.localPosition += new Vector3(0, -1f, 0);
                    break;
                case 3: //북
                    transform.localPosition += new Vector3(0, 1f, 0);
                    break;
            }
        }
    }

    #endregion
}
