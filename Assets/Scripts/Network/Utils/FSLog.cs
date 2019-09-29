using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FSLog 
{
    /// <summary>
    /// Log输出开关
    /// </summary>
    public static bool m_Log = true;
    public static bool m_LogDebug = false;

    public static string GetCurrTime()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
    }

    public static void Debug(object str)
    {    
        if (m_LogDebug)
        {
            UnityEngine.Debug.Log($"[{GetCurrTime()} ],[{Thread.CurrentThread.ManagedThreadId}] ==> {str}");
        }
    }

    public static void Info(object str)
    {    
        if (m_Log)
        {
            UnityEngine.Debug.Log($"[{GetCurrTime()} ],[{Thread.CurrentThread.ManagedThreadId}] ==> {str}");
        }
    }

    public static void Warning(object str)
    {
        if (m_Log)
        {
            UnityEngine.Debug.LogWarning($"[{GetCurrTime()} ],[{Thread.CurrentThread.ManagedThreadId}] ==> {str}");
        }
    }

    public static void Error(object str)
    {
        if (m_Log)
        {
            UnityEngine.Debug.LogError($"[{GetCurrTime()} ],[{Thread.CurrentThread.ManagedThreadId}] ==> {str}");
        }
    }

}
