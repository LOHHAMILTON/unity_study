using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    private Dictionary<int, WaitForSeconds> _waitForSecondsDict = new Dictionary<int, WaitForSeconds>();
    private int _initTick;
    private DateTime _initDateTime;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(Init());
    }
    IEnumerator Init()
    {
        Instance = this;
        var isInit = false;
        _initTick = Environment.TickCount;
        _initDateTime = DateTime.UtcNow.AddHours(9);
        while (!isInit)
        {
            Debug.Log("Try Init TimeManager");
            using (var request = UnityWebRequest.Get("www.naver.com"))//���̹� �����ð��� �����´�.
            {
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var date = request.GetResponseHeader("date");
                    var parseDate = DateTime.Parse(date);
                    _initTick = Environment.TickCount;
                    _initDateTime = parseDate;
                    isInit = true;
                    Debug.Log("Init TimeManager");
                }
                else //��Ʈ��ũ�� �Ҿ����� ��� �����Ҷ����� �ݺ��Ѵ�.
                {
                    Debug.LogWarning(request.error);
                    yield return GetWaitForSeconds(1000);
                }
            }
        }
    }
    public WaitForSeconds GetWaitForSeconds(int milliSeconds)
    {
        if (!_waitForSecondsDict.ContainsKey(milliSeconds))
        {
            _waitForSecondsDict.Add(milliSeconds, new WaitForSeconds((float)milliSeconds * 0.001f));
        }
        return _waitForSecondsDict[milliSeconds];
    }
    public int GetTimeElapseMilliSce()
    {
        return Environment.TickCount - _initTick;
    }

    public DateTime GetTickUTCNow()
    {
        return _initDateTime.AddMilliseconds(GetTimeElapseMilliSce());
    }
}