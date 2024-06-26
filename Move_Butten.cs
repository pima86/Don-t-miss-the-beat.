using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Move_Butten : MonoBehaviour
{
    #region inst
    public static Move_Butten Inst { get; private set; }
    void Awake() => Inst = this;
    #endregion

    [SerializeField] GameObject player;
    [SerializeField] Bit_bouns bit_bouns;
    [SerializeField] Bit_bouns[] bouns_list;
    [Header("사운드")]
    [SerializeField] AudioClip move_clip;
    [SerializeField] AudioClip Open_clip;
    [SerializeField] AudioClip Gem_clip;
    [SerializeField] AudioClip Get_clip;
    [SerializeField] AudioClip Heal_clip;
    [SerializeField] AudioClip Debuf_clip;

    [SerializeField] AudioClip powerup_clip;

    [Header("mp3 버튼")]
    [SerializeField] GameObject previous_butten;
    [SerializeField] GameObject next_butten;

    #region 이동
    GameObject bit_first;
    public void Move(int num)
    {
        SoundManager.Inst.Create_note();

        if (game_obj[0].activeSelf)
            bit_bouns = bouns_list[0];
        else
            bit_bouns = bouns_list[1];

        if (Player_Move.Inst.HP == 0)
            return;

        #region 판정
        if (bit_bouns.bits.Count == 0)
        {
            //비트가 없는 상태에서의 행동 이펙트

            //BitManager.Inst.combo = 0;
            return;
        }
        if (bit_first == bit_bouns.bits[0].gameObject)
            return;
        else
            bit_first = bit_bouns.bits[0].gameObject;

        bit_bouns.bits[0].play = true;
        bit_bouns.bits[0].butten.color = new Color(1, 1, 1, 0);

        Skill_script.Inst.Item_Get(bit_bouns.bits[0].item_image.sprite.name);

        bit_bouns.bits[0].item_obj.SetActive(false);
        //Bit_bouns.Inst.bits[0].Judgment();
        bit_bouns.HitEffect();

        #endregion

        Vector3 pos = new();

        switch (num)
        {
            case 0: //동
                pos = new Vector3(1f, 0, 0);
                Player_Move.Inst.face.flipX = false;
                break;
            case 1: //서
                pos = new Vector3(-1f, 0, 0);
                Player_Move.Inst.face.flipX = true;
                break;
            case 2: //남
                pos = new Vector3(0, -1, 0);
                break;
            case 3: //북
                pos = new Vector3(0, 1, 0);
                break;
        }

        StartCoroutine(Move_ing(num, pos));
    }

    IEnumerator Move_ing(int num, Vector3 pos)
    {
        yield return StartCoroutine(Ing(num, pos));

        BitManager.Inst.obj_bouns = false;
    }

    IEnumerator Ing(int num, Vector3 pos)
    {
        if (Skill_script.Inst.X2)
            Player_Move.Inst.LV++;
        Player_Move.Inst.LV += 1;

        #region 벽과충돌
        if (Player_Move.Inst.move_point[num] == 0)
            SoundManager.Inst.SFXPlay("walk", move_clip);
        #endregion
        #region 캐릭터 이동
        else if (Player_Move.Inst.move_point[num] == 1)
        {
            //Camera.main.transform.localPosition += pos;
            player.transform.localPosition += pos;
        }
        #endregion
        #region 몬스터에게 공격
        else if (Player_Move.Inst.move_point[num] == 2)
        {
            RaycastHit2D hit = Physics2D.Raycast(player.transform.localPosition + pos, transform.forward, 1, LayerMask.GetMask("Object"));
            if (hit.collider != null)
            {
                hit.collider.GetComponent<Mob_state>().Heat();
                Player_Attact.Inst.Att_effect(num);
            }
            player.GetComponentInChildren<Bit_obj>().Motion_att(num);
        }
        #endregion
        #region 박스 오픈
        else if (Player_Move.Inst.move_point[num] == 3)
        {
            RaycastHit2D hit = Physics2D.Raycast(player.transform.localPosition + pos, transform.forward, 1, LayerMask.GetMask("Object"));
            Debug.DrawRay(player.transform.localPosition + pos, transform.forward, Color.red, 0.3f);
            if (hit.collider != null)
                hit.collider.GetComponent<Box_Open>().Touch_Box();

            SoundManager.Inst.SFXPlay("box", move_clip);
            player.GetComponentInChildren<Bit_obj>().Motion_att(num);
        }
        #endregion
        #region 아이템 획득
        else if (Player_Move.Inst.move_point[num] == 4)
        {
            RaycastHit2D hit = Physics2D.Raycast(player.transform.localPosition + pos, transform.forward, 1, LayerMask.GetMask("Object"));
            if (hit.collider != null)
            {
                //골드
                if (hit.collider.name == "tiles_packed_8")
                {
                    Player_Move.Inst.Gem += 10;
                    SoundManager.Inst.SFXPlay("Gem_Get", Gem_clip);
                }
                //골드
                else if (hit.collider.name == "tiles_packed_7")
                {
                    Player_Move.Inst.Coin += 100;
                    SoundManager.Inst.SFXPlay("Coin_Get", Get_clip);
                }
                //소형포션
                else if (hit.collider.name == "tilemap_127")
                {
                    Player_Move.Inst.HP += 0.5f;
                    SoundManager.Inst.SFXPlay("Heal_Get", Heal_clip);
                }
                //포션
                else if (hit.collider.name == "tilemap_115")
                {
                    Player_Move.Inst.HP += 1f;
                    SoundManager.Inst.SFXPlay("Heal_Get", Heal_clip);
                }
                //x
                else if (hit.collider.name == "tiles_packed_19")
                {
                    Player_Move.Inst.LV *= 2;
                    SoundManager.Inst.SFXPlay("Buff_Get", Heal_clip);
                }
                //%
                else if (hit.collider.name == "tiles_packed_20")
                {
                    if (Player_Move.Inst.LV != 1)
                        Player_Move.Inst.LV /= 2;
                    SoundManager.Inst.SFXPlay("Debuff_Get", Debuf_clip);
                }

                Mob_spawn.Inst.box_Destroy(hit.collider.gameObject);
            }


            //Camera.main.transform.localPosition += pos;
            player.transform.localPosition += pos;
        }
        #endregion

        yield break;
    }
    #endregion
    #region 게임 패널들
    public GameObject[] main_obj; //기본
    public GameObject[] game_obj; //게임중

    public GameObject default_obj; //기본
    public GameObject custom_obj; //커스텀
    public GameObject power_obj; //능력
    public GameObject option_obj; //옵션
    public GameObject music_obj; //노래선택
    #endregion
    #region 커스터마이징
    [SerializeField] Image[] custom_img;

    public void OnButten_Custom(string name)
    {
        default_obj.transform.localPosition = new Vector3(5000, 0, 0);

        custom_obj.SetActive(true);
        music_obj.SetActive(false);
        option_obj.SetActive(false);

        Player.Inst.First_Setting(name, custom_img);
    }

    public void OnButten_Change(int num)
    {
        if(Player.Inst.Character_Setting(num))
            custom_obj.SetActive(false); 
    }
    #endregion
    #region 능력
    public void OnButten_Power()
    {
        power_obj.SetActive(!power_obj.activeSelf);
        custom_obj.SetActive(false);
        option_obj.SetActive(false);
        music_obj.SetActive(false);

        if (power_obj.activeSelf)
            default_obj.transform.localPosition = new Vector3(5000, 0, 0);
        else
            default_obj.transform.localPosition = Vector3.zero;

        Skill_Up.Inst.level_setting();
    }

    public void OnButten_Levelup(int i)
    {
        Skill_Up.Inst.level_up(i);
        SoundManager.Inst.SFXPlay("powerup_Get", powerup_clip);
    }
    #endregion
    #region 옵션
    public void OnButten_Option()
    {
        custom_obj.SetActive(false);
        music_obj.SetActive(false);
        option_obj.SetActive(!option_obj.activeSelf);

        if (option_obj.activeSelf)
            default_obj.transform.localPosition = new Vector3(5000, 0, 0);
        else
            default_obj.transform.localPosition = Vector3.zero;

        //오브젝트 비활성화

        if (Move_Butten.Inst.game_obj[0].activeSelf)
        {
            for (int i = 0; i < Mob_spawn.Inst.mob_list.Count; i++)
                Mob_spawn.Inst.mob_list[i].SetActive(!option_obj.activeSelf);
            for (int i = 0; i < Mob_spawn.Inst.box_list.Count; i++)
                Mob_spawn.Inst.box_list[i].SetActive(!option_obj.activeSelf);
            Player_Move.Inst.body_obj[0].SetActive(!option_obj.activeSelf);
        }
    }
    #endregion
    #region 노래선택
    //mp3 화면 on
    public void OnButten_Music()
    {
        power_obj.SetActive(false);
        custom_obj.SetActive(false);
        option_obj.SetActive(false);
        music_obj.SetActive(!music_obj.activeSelf);

        if (music_obj.activeSelf)
            default_obj.transform.localPosition = new Vector3(5000, 0, 0);
        else
            default_obj.transform.localPosition = Vector3.zero;

        if (music_obj.activeSelf)
        {
            SoundManager.Inst.MP3_Setting(Player.Inst.play_music);
            SoundManager.Inst.Music_select(Player.Inst.play_music);
        }
    }

    public void OnButten_Next(int num)
    {
        Player.Inst.play_music += num;

        //일러스트 변경
        SoundManager.Inst.MP3_Setting(Player.Inst.play_music);
        SoundManager.Inst.Music_select(Player.Inst.play_music);
    }

    public void Next_set(int num)
    {
        if (num == 0)
        {
            previous_butten.SetActive(false);
            next_butten.SetActive(true);
        }
        else if (num == (SoundManager.Inst.music_clip.Length - 1))
        {
            previous_butten.SetActive(true);
            next_butten.SetActive(false);
        }
        else
        {
            previous_butten.SetActive(true);
            next_butten.SetActive(true);
        }
    }
    #endregion
    #region 재시작
    public void ReStart()
    {
        default_obj.transform.localPosition = Vector3.zero;

        for (int i = 0; i < main_obj.Length; i++)
            main_obj[i].SetActive(false);
        for (int i = 0; i < game_obj.Length; i++)
            game_obj[i].SetActive(true);

        //재화 획득
        option_obj.SetActive(false);

        bit_bouns = bouns_list[0];
        bit_bouns.text_effect.text = "";

        Player.Inst.Zero_Money();
        SoundManager.Inst.ReStart();
        BitManager.Inst.ReStart();
        Player_Move.Inst.ReStart();
        Mob_spawn.Inst.ReStart();
    }

    public void Play_exit()
    {
        custom_obj.SetActive(false);
        music_obj.SetActive(false);
        option_obj.SetActive(false);

        for (int i = 0; i < main_obj.Length; i++)
            main_obj[i].SetActive(true);
        for (int i = 0; i < game_obj.Length; i++)
            game_obj[i].SetActive(false);

        bit_bouns = bouns_list[1];

        Player.Inst.Now_Money();
        SoundManager.Inst.ReStart();
        BitManager.Inst.ReStart();
        Player_Move.Inst.ReStart();
        Mob_spawn.Inst.ReStart();
    }
    #endregion
}
