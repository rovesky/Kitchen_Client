using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RttData
{
    public uint sequence;

    public long sendTime;

    public long Rtt;
}

public class NetStatistics 
{
    public List<RttData> rtts = new List<RttData>();
    private uint lastAck = 0;
    // Start is called before the first frame update

    public void Send(uint sequence, long time)
    {
       
        rtts.Add(new RttData { sequence = sequence, sendTime = time, Rtt = -1 });

     //   FSLog.Log($"send:{ sequence},rtts:{rtts.Count}");
    }

    // Update is called once per frame
    public void Recv(uint sequence, long time)
    {
        if (rtts.Count == 0)
            return;
        //      var data  = rtts.Find(s => { return s.sequence == sequence; });       
    //    FSLog.Log($"recv:{ sequence},rtts:{rtts.Count}" );
        var data = rtts[(int)sequence];
        data.Rtt = time - data.sendTime;
        lastAck = sequence;
    }


    public double CaculateRtt(int tickCount = 60 * 10)
    {
        if (rtts.Count == 0)
            return 0;

        long total = 0;

        int count = 0;
        for (int i = (int)lastAck; i >= Mathf.Max(lastAck - tickCount, 0); --i)
        {
            count++;
            total += rtts[i].Rtt;
        }

        if(count > 0)
            return ((double)total / count) / 10000;

        if (rtts.Count > 60 * 100)
            rtts.Clear();

        return 0;
    }
}
