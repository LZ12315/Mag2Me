using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{

    [Header("音乐")]
    [SerializeField] AK.Wwise.Event MainMenu;
    [SerializeField] AK.Wwise.Event Combat;
    [SerializeField] AK.Wwise.Event Rest;
    [SerializeField] AK.Wwise.Event End;
    [SerializeField] AK.Wwise.Event Boss;

    [Header("音效")]
    [SerializeField] AK.Wwise.Event Boss_Attack;
    [SerializeField] AK.Wwise.Event Boss_Dash;
    [SerializeField] AK.Wwise.Event Boss_Fight;
    [SerializeField] AK.Wwise.Event Boss_Start;
    [SerializeField] AK.Wwise.Event Enemy_Died;
    [SerializeField] AK.Wwise.Event Enemy_TakeDamage;
    [SerializeField] AK.Wwise.Event Main_Button;
    [SerializeField] AK.Wwise.Event Player_Absorb;
    [SerializeField] AK.Wwise.Event Player_Died;
    [SerializeField] AK.Wwise.Event Player_Shoot;
    [SerializeField] AK.Wwise.Event Player_TakeDamage;
    [SerializeField] AK.Wwise.Event Player_Win; 
    [SerializeField] AK.Wwise.Event Price_Normal;
    [SerializeField] AK.Wwise.Event Price_Special;
    [SerializeField] AK.Wwise.Event Switch_RestOver;

    [Header("配音")]
    [SerializeField] AK.Wwise.Event Buff_EquippableLimit;
    [SerializeField] AK.Wwise.Event Buff_HP;
    [SerializeField] AK.Wwise.Event Buff_Special;

    [Header("声音源物体")]
    [SerializeField] GameObject VocalManager;
    [SerializeField] GameObject MusicManager;
    [SerializeField] GameObject SoundManager;



    private static AudioManager _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // 使该物体在场景切换时不被销毁
            SceneManager.sceneLoaded += OnSceneLoaded; // 订阅场景加载事件
        }
        else
        {
            Destroy(gameObject); // 如果已经存在一个实例，则销毁自身
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }



    private void Start()
    {
        AddEvent();
        TriggerMusicEvent(MainMenu);

        TriggerMusicEvent(Combat);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 在这里调用你想要执行的函数
        AddEvent();
    }


    private void TriggerVocalEvent(AK.Wwise.Event wwiseEvent)
    {
        if (wwiseEvent != null)
        {
            // 使用当前游戏对象作为发射体
            wwiseEvent.Post(VocalManager);
        }
        else
        {
            Debug.LogWarning("Wwise事件未分配！");
        }
    }

    private void TriggerMusicEvent(AK.Wwise.Event wwiseEvent)
    {
        if (wwiseEvent != null)
        {
            // 使用当前游戏对象作为发射体
            wwiseEvent.Post(MusicManager);
        }
        else
        {
            Debug.LogWarning("Wwise事件未分配！");
        }
    }

    private void TriggerSoundEvent(AK.Wwise.Event wwiseEvent)
    {
        if (wwiseEvent != null)
        {
            // 使用当前游戏对象作为发射体
            wwiseEvent.Post(SoundManager);
        }
        else
        {
            Debug.LogWarning("Wwise事件未分配！");
        }
    }


    private void AddEvent()
    {
        #region Sound
        EventCenter.Instance.AddEventListener("BOSS攻击", () => TriggerSoundEvent(Boss_Attack));
        EventCenter.Instance.AddEventListener("BOSS冲刺", () => TriggerSoundEvent(Boss_Dash));
        EventCenter.Instance.AddEventListener("BOSS战斗", () => TriggerSoundEvent(Boss_Fight));
        EventCenter.Instance.AddEventListener("BOSS战开始", () => TriggerSoundEvent(Boss_Start));
        EventCenter.Instance.AddEventListener("敌人死亡", () => TriggerSoundEvent(Enemy_Died));
        EventCenter.Instance.AddEventListener("敌人受伤", () => TriggerSoundEvent(Enemy_TakeDamage));
        EventCenter.Instance.AddEventListener("开始游戏", () => TriggerSoundEvent(Main_Button));
        EventCenter.Instance.AddEventListener("玩家吸收开始", () => TriggerSoundEvent(Player_Absorb));
        EventCenter.Instance.AddEventListener("玩家吸收结束", () => Player_Absorb.Stop(SoundManager));

        EventCenter.Instance.AddEventListener("玩家死亡", () => TriggerSoundEvent(Player_Died));
        EventCenter.Instance.AddEventListener("玩家攻击", () => TriggerSoundEvent(Player_Shoot));
        EventCenter.Instance.AddEventListener("玩家受伤", () => TriggerSoundEvent(Player_TakeDamage));
        EventCenter.Instance.AddEventListener("胜利", () => TriggerSoundEvent(Player_Win));
        EventCenter.Instance.AddEventListener("普通奖励", () => TriggerSoundEvent(Price_Normal));
        EventCenter.Instance.AddEventListener("特殊奖励", () => TriggerSoundEvent(Price_Special));
        EventCenter.Instance.AddEventListener("休息开始", () => TriggerSoundEvent(Switch_RestOver));

        #endregion



        #region Music
        EventCenter.Instance.AddEventListener("主界面", () => TriggerMusicEvent(MainMenu));
        EventCenter.Instance.AddEventListener("战斗", () => TriggerMusicEvent(Combat));
        EventCenter.Instance.AddEventListener("休息", () => TriggerMusicEvent(Rest));
        EventCenter.Instance.AddEventListener("结束", () => TriggerMusicEvent(End));
        EventCenter.Instance.AddEventListener("BOSS", () => TriggerMusicEvent(Boss));

        #endregion

        #region Vocal
        EventCenter.Instance.AddEventListener("血量增加", () => TriggerVocalEvent(Buff_HP));
        EventCenter.Instance.AddEventListener("组件增加", () => TriggerVocalEvent(Buff_EquippableLimit));
        EventCenter.Instance.AddEventListener("乌萨奇特殊奖励", () => TriggerVocalEvent(Buff_Special));


        #endregion
    }
}
