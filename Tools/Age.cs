using System;

[Serializable]
public class Age
{
    public int tenMins = 0;
    public int hours = 0;
    public int days = 0;
    public int months = 0;
    public int years = 0;
    public bool MainTime = false;
    public bool active;
    public static Age zero => new Age(0,0,0,0,0);

    public int totalTicks()
    {
        int total = tenMins;
        total += hours * 6;
        total += days * 24 * 6;
        total += months * 30 * 24 * 6;
        total += years * 12 * 30 * 24 * 6;
        return total;
    }
    public void Reset()
    {
        tenMins = 0;
        hours = 0;
        days = 0;
        months = 0;
        years = 0;
    }
    public void Activate()
    {
        if (!active)
        {
            Game.main.tenMinTick.AddListener(TenMinTick);
        }
    }
    public void DeActivate()
    {
        if (active)
        {
            Game.main.tenMinTick.RemoveListener(TenMinTick);
        }
    }
    public Age(Age clone)
    {        
        tenMins = clone.tenMins;
        hours = clone.hours;
        days = clone.days;
        months = clone.months;
        years = clone.years;
        MainTime = false;
        if (clone.active)
        {
            Game.main.tenMinTick.AddListener(TenMinTick);
        }
        active = clone.active;
    }

    public Age(int TenMins = 0,int Hours = 0,int Days = 0,int Months = 0, int Years = 0, bool mainTime = false)
    {
        Game.main.tenMinTick.AddListener(TenMinTick);
        tenMins = TenMins;
        hours = Hours;  
        days = Days;
        months = Months;
        years = Years;
        MainTime = mainTime;
        active = true;
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
            time += years.ToString() + " Years ";
        }
        if (includeM)
        {
            time += months.ToString() + " Months ";
        }
        if (includeD)
        {
            time += days.ToString() + " Days ";
        }
        if (includeH)
        {
            time += hours.ToString() + " Hours ";
        }
        if (includeTM)
        {
            time += tenMins.ToString() + "0  mins ";
        }
        return time;
    }
    public string ToDate()
    {
        string val = "";
        val += (hours % 12) + 1 + ":"+tenMins+"0" + (hours > 11 ? "PM " : "AM ");
        val += days + 1 + GetDaySuffix(days + 1) +" ";
        val += ToMonth(months) + " ";
        val += years + " AD";
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
        tenMins += TenMins;
        if (tenMins >= 6)
        {
            hours++;
            if (MainTime)
            {
                Game.main.hourTick.Invoke();
            }
            tenMins -= 6;
        }
        hours += Hours;
        if (hours >= 24)
        {
            days++;
            if (MainTime)
            {
                Game.main.dayTick.Invoke();
            }
            hours -= 24;
        }
        days += Days;
        if (days >= 30)
        {
            months++;
            if (MainTime)
            {
                Game.main.monthTick.Invoke();
            }
            days -= 30;
        }
        months += Months;
        if (months >= 12)
        {
            years++;
            if (MainTime)
            {
                Game.main.yearTick.Invoke();
            }
            months -= 12;
        }
        years += Years;                       
    }
}
