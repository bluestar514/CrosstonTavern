using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldTime : IComparable
{
    public int day;
    public int month;
    public int year;
    public int hour;
    public int minute;

    public static int MinInHour = 60;
    public static int HourInDay = 24;
    public static int DayInMonth = 28;
    public static int MonthInYear = 4; //using seasons rather than months in the HarvestMoon Tradition


    public static WorldTime DayZeroEightAM = new WorldTime(1, 1, 1, 8, 0);


    public WorldTime(int year, int month, int day, int hour, int minute)
    {
        this.day = day;
        this.month = month;
        this.year = year;
        this.hour = hour;
        this.minute = minute;
    }

    public static WorldTime Date(int year, int month, int day)
    {
        return new WorldTime(year, month, day, 0, 0);
    }

    public static WorldTime Time(int hour, int minute)
    {
        return new WorldTime(-1, -1, -1, hour, minute);
    }

    public override string ToString()
    {
        return "<" + year + "-" + month + "-" + day + " " + hour + ":" + minute + ">";
    }


    public void Tick(int i = 1)
    {
        minute += i;

        if(minute >= MinInHour) {
            hour += minute / MinInHour;
            minute = minute % MinInHour;
        }
        if(hour >= HourInDay) {
            if(day >= 0) day += hour / HourInDay;
            hour = hour % HourInDay;
        }
        if(day > DayInMonth) {
            if (month >= 0) month += day / DayInMonth;
            day = day % DayInMonth;
        }
        if(month >= MonthInYear) {
            if (year >= 0) year += month / MonthInYear;
            month = month % MonthInYear;
        }
    }

    public void AdvanceHour(int h = 1)
    {
        Tick(MinInHour*h);
    }

    public void AdvanceDay(int d = 1)
    {
        AdvanceHour(HourInDay*d);
    }

    public void AdvanceMonth(int m = 1)
    {
        AdvanceDay(DayInMonth*m);
    }
    public void AdvanceYear(int y = 1)
    {
        AdvanceMonth(MonthInYear*y);
    }

    public void AdvanceToStartOfHour(int h = 1)
    {
        Tick((MinInHour * h) - minute);
    }
    public void AdvanceToStartOfDay(int d = 1)
    {
        AdvanceToStartOfHour((HourInDay * d) - hour);
    }

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;

        WorldTime other = obj as WorldTime;
        if(other != null) {
            if (this.year < other.year      && this.year>=0)        return -1;
            if (this.year > other.year      && other.year >= 0)     return  1;
            if (this.month < other.month    && this.month >= 0)     return -1;
            if (this.month > other.month    && other.month >= 0)    return  1;
            if (this.day < other.day        && this.day >= 0)       return -1;
            if (this.day > other.day        && other.day >= 0)      return  1;
            if (this.hour < other.hour      && this.hour >= 0)      return -1;
            if (this.hour > other.hour      && other.hour >= 0)     return  1;
            if (this.minute < other.minute  && this.minute >= 0)    return -1;
            if (this.minute > other.minute  && other.minute >= 0)   return  1;

            return 0;
        }else{
            throw new ArgumentException("Object is not a WorldTime");
        }
    }

    public override bool Equals(object obj)
    {
        return this.CompareTo(obj) == 0;
    }

    public override int GetHashCode()
    {
        var hashCode = 1551791206;
        hashCode = hashCode * -1521134295 + day.GetHashCode();
        hashCode = hashCode * -1521134295 + month.GetHashCode();
        hashCode = hashCode * -1521134295 + year.GetHashCode();
        hashCode = hashCode * -1521134295 + hour.GetHashCode();
        hashCode = hashCode * -1521134295 + minute.GetHashCode();
        return hashCode;
    }

    public static bool operator <(WorldTime a, WorldTime b) => a.CompareTo(b) < 0;
    public static bool operator >(WorldTime a, WorldTime b) => a.CompareTo(b) > 0;
    public static bool operator ==(WorldTime a, WorldTime b) => a.CompareTo(b) == 0;
    public static bool operator !=(WorldTime a, WorldTime b) => a.CompareTo(b) != 0;
    public static bool operator <=(WorldTime a, WorldTime b) => a.CompareTo(b) <= 0;
    public static bool operator >=(WorldTime a, WorldTime b) => a.CompareTo(b) >= 0;
}
