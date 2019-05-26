using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reciever : MonoBehaviour
{
    private Vector3 position;
    private List<float> timeList;

    public void SetPosition(Vector3 newPos)
    {
        this.position = newPos;
        transform.position = newPos;
    }

    public float GetPosX()
    {
        return this.position.x;
    }

    public float GetPosY()
    {
        return this.position.z;
    }

    public void AddTime(float time)
    {
        if (timeList == null)
            timeList = new List<float>();

        timeList.Add(time);
    }

    public List<float> GetTimesList()
    {
        return this.timeList;
    }

    public float GetTime(int index)
    {
        return timeList[index];
    }

    public float GetDistanceToSource(int timeIndex)
    {
        return timeList[timeIndex] * 1000000; // time * speed (1000 km/s)
    }
}