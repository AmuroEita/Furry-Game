using JTUtility;
using JTUtility.Bezier;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class WayPoints : MonoBehaviour
{
    public Map map;
    public GameObject startPoint;
    public GameObject[] wayPoints;

    private int wayPointIndex;
    void Start()
    {
        wayPointIndex = 0;
        map = GetComponent<Map>();
        if (map == null)
        {
            Debug.LogError("WayPoints can't find Map");
            return;
        }
        startPoint = map.GetComponentInChildren<GameObject>();
        if (startPoint == null)
        {
            Debug.LogError("WayPoints can't find StartPoint on Map");
            return;
        }
        wayPoints = startPoint.GetComponentsInChildren<GameObject>();
        if (wayPoints == null)
        {
            Debug.LogError("Can't find WayPoints on StartPoint");
        }
    }

    Vector3 GetSpawnPosition()
    {
        //获得一个起点附近的随机位置，如果合适，怪会放置在这个位置上
        return startPoint.transform.position;
    }

    Vector3 Direction(Vector3 curPos)
    {
        //传入当前怪的位置，给出接下来前进的方向
        Vector3 direction;
        //寻找下一个路径点的方向
        direction = (wayPoints[wayPointIndex].transform.position - curPos).normalized;
        //找到最近的路径点
        Vector3 closetPoint = ClosetPoint(curPos);
        //为下一路径点方向加入最近点移动偏移量
        direction = (direction + (closetPoint - curPos).normalized).normalized;
        return direction;
    }

    Vector3 ClosetPoint(Vector3 curPos)
    {
        Vector3 midPoint = (startPoint.transform.position + wayPoints[0].transform.position) / 2;
        float minDistance;
        Vector3 closetPoint = Bezier.GetClosestPoint(startPoint.transform.position, midPoint, wayPoints[0].transform.position, curPos, out minDistance);
        for (int i = 0; i < wayPoints.Length - 1; i++)
        {
            Vector3 p1 = wayPoints[i].transform.position;
            Vector3 p2 = wayPoints[i + 1].transform.position;
            midPoint = (p1 + p2) / 2;
            float distance;
            Vector3 point = Bezier.GetClosestPoint(p1, midPoint, p2, curPos, out distance);
            if (distance < minDistance)
            {
                closetPoint = point;
            }
        }
        return closetPoint;
    }

    void OnReachWayPoint()
    {
        wayPointIndex++;
        if (wayPointIndex >= wayPoints.Length)
        {
            //OnReachedEnd?.Invoke(character);
        }
    }

    public event JTUtility.Action<Character> OnReachedEnd;
}
