using MessagePack;
using System;

[Serializable]
[MessagePackObject(keyAsPropertyName: true)]
public class Age
{
    public int tm = 0;
    public int h = 0;
    public int d = 0;
    public int m = 0;
    public int y = 0;
    public bool mt = false;
    public bool a;
    public static Age zero => new Age(0,0,0,0,0);

    public Age()
    {

    }
    public Age(int totalTicks)
    {
        int years = totalTicks / (12 * 30 * 24 * 6);
        totalTicks -= (years * 12 * 30 * 24 * 6);
        int months = totalTicks / (30 * 24 * 6);
        totalTicks -= (months * 30 * 24 * 6);
        int days = totalTicks / (24 * 6);
        totalTicks -= (days * 24 * 6);
        int hours = totalTicks / 6;
        totalTicks -= (hours * 6);
        y = years;
        m = months;
        d = days;
        h = hours;
        tm = totalTicks;
    }
    public int totalTicks()
    {
        int total = tm;
        total += h * 6;
        total += d * 24 * 6;
        total += m * 30 * 24 * 6;
        total += y * 12 * 30 * 24 * 6;
        return total;
    }
    public void Reset()
    {
        tm = 0;
        h = 0;
        d = 0;
        m = 0;
        y = 0;
    }
    public void Activate()
    {
        if (!a)
        {
            Game.main.tenMinTick.AddListener(TenMinTick);
        }
    }
    public void DeActivate()
    {
        if (a)
        {
            Game.main.tenMinTick.RemoveListener(TenMinTick);
        }
    }
    public void Set(Age age)
    {
        tm = age.tm;
        h = age.h;
        d = age.d;
        m = age.m;
        y = age.y;
    }
    public Age(Age clone)
    {        
        tm = clone.tm;
        h = clone.h;
        d = clone.d;
        m = clone.m;
        y = clone.y;
        mt = false;
        if (clone.a)
        {
            Game.main.tenMinTick.AddListener(TenMinTick);
        }
        a = clone.a;
    }
    public int NumDays()
    {
        return d + m * 30 + y * 360;
    }
    public Age(int TenMins = 0,int Hours = 0,int Days = 0,int Months = 0, int Years = 0, bool mainTime = false)
    {
        Game.main.tenMinTick.AddListener(TenMinTick);
        tm = TenMins;
        h = Hours;  
        d = Days;
        m = Months;
        y = Years;
        mt = mainTime;
        a = true;
    }
    public void TenMinTick()
    {
        AddTime(1);
    }
    public string ToString(bool includeY = false, bool includeM = false, bool includeD = false, bool includeH = false, bool includeTM = false)
    {
        string time = "";
        if (includeY)
        {
            time += y.ToString() + " Years ";
        }
        if (includeM)
        {
            time += m.ToString() + " Months ";
        }
        if (includeD)
        {
            time += d.ToString() + " Days ";
        }
        if (includeH)
        {
            time += h.ToString() + " Hours ";
        }
        if (includeTM)
        {
            time += tm.ToString() + "0  mins ";
        }
        return time;
    }
    public string ToDate()
    {
        string val = "";
        val += (h % 12) + 1 + ":"+tm+"0" + (h > 11 ? "PM " : "AM ");
        val += d + 1 + GetDaySuffix(d + 1) +" ";
        val += ToMonth(m) + " ";
        val += y + " AD";
        return val;
    }
    public string GetDaySuffix(int day)
    {
        int lastNum = day % 10;
        string suffix = "th";
        if(lastNum == 1 && day != 11)
        {
            suffix = "st";
        }
        else if (lastNum == 2 && day != 12)
        {
            suffix = "nd";
        }
        else if (lastNum == 3 && day != 13)
        {
            suffix = "rd";
        }
        return suffix;
    }
    public string ToMonth(int month)
    {
        if(month == 0)
        {
            return "January";
        }
        else if (month == 1)
        {
            return "February";
        }
        else if (month == 2)
        {
            return "March";
        }
        else if (month == 3)
        {
            return "April";
        }
        else if (month == 4)
        {
            return "May";
        }
        else if (month == 5)
        {
            return "June";
        }
        else if (month == 6)
        {
            return "July";
        }
        else if (month == 7)
        {
            return "August";
        }
        else if (month == 8)
        {
            return "September";
        }
        else if (month == 9)
        {
            return "October";
        }
        else if (month == 10)
        {
            return "November";
        }
        else
        {
            return "December";
        }
    }
    public void AddTime(int TenMins = 0,int Hours = 0, int Days = 0, int Months = 0, int Years = 0)
    {
        tm += TenMins;
        if (tm >= 6)
        {
            h++;
            if (mt)
            {
                Game.main.hourTick.Invoke();
            }
            tm -= 6;
        }
        h += Hours;
        if (h >= 24)
        {
            d++;
            if (mt)
            {
                Game.main.dayTick.Invoke();
            }
            h -= 24;
        }
        d += Days;
        if (d >= 30)
        {
            m++;
            if (mt)
            {
                Game.main.monthTick.Invoke();
            }
            d -= 30;
        }
        m += Months;
        if (m >= 12)
        {
            y++;
            if (mt)
            {
                Game.main.yearTick.Invoke();
            }
            m -= 12;
        }
        y += Years;                       
    }
}
