using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

interface IEventInfo
{

}

class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;
    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}


class EventInfo : IEventInfo
{
    public UnityAction actions;

    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}



class EventCenter
{
    private static EventCenter instance;
    public static EventCenter Instance { get => instance ?? (instance = new EventCenter()); }

    private static Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();


    /// <summary>
    /// 添加泛型事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventName"></param>
    /// <param name="action"></param>
    public void AddEventListener<T>(string eventName, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T>).actions += action;
            Debug.Log("事件" + eventName + "新增委托");
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T>(action));
            Debug.Log("新建事件" + eventName);
        }
    }


    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="action"></param>
    public void AddEventListener(string eventName, UnityAction action)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo).actions += action;
            Debug.Log("事件" + eventName + "新增委托");
        }
        else
        {
            eventDic.Add(eventName, new EventInfo(action));
            Debug.Log("新建事件" + eventName);
        }
    }


    /// <summary>
    /// 触发泛型委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventName"></param>
    /// <param name="info"></param>
    public void EventTrigger<T>(string eventName, T info)
    {
        if (!eventDic.ContainsKey(eventName))
        {
            Debug.LogWarning("事件" + eventName + "不存在");
            return;
        }

        (eventDic[eventName] as EventInfo<T>).actions.Invoke(info);
    }



    /// <summary>
    /// 触发委托
    /// </summary>
    /// <param name="eventName"></param>
    public void EventTrigger(string eventName)
    {
        if (!eventDic.ContainsKey(eventName))
        {
            Debug.LogWarning("事件" + eventName + "不存在");
            return;
        }

        (eventDic[eventName] as EventInfo).actions.Invoke();
    }

    /// <summary>
    /// 清空事件,切换场景用
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }


}



