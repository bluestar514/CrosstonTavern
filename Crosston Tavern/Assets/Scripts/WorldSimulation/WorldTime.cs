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
    public static WorldTime Morning = new WorldTime(-1, -1, -1, 8, 0);
    public static WorldTime Noon = new WorldTime(-1, -1, -1, 12, 0);
    public static WorldTime Evening = new WorldTime(-1, -1, -1, 16, 0);
    public static WorldTime Night = new WorldTime(-1, -1, -1, 22, 0);


    public enum CasualTimeBlocks
    {
        today,
        yesterday,
        days,
        lastweek,
        weeks,
        lastmonth,
        months,
        lastyear,
        years,
        future
    }


    public WorldTime(int year, int month, int day, int hour, int minute)
    {
        this.day = day;
        this.month = month;
        this.year = year;
        this.hour = hour;
        this.minute = minute;
    }

    public WorldTime(WorldTime time):this(time.year, time.month, time.day, time.hour, time.minute){}

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

    public string GetDate()
    {
        return month + "/" + day + "/" + year;
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

    public int AdvanceToStartOfHour(int h = 1)
    {
        Tick((MinInHour * h) - minute);
        return (MinInHour * h) - minute;
    }
    public int AdvanceToStartOfDay(int d = 1)
    {
        return AdvanceToStartOfHour((HourInDay * d) - hour);
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

    public bool SameDay(WorldTime date)
    {
        if (this.year == date.year &&
            this.month == date.month &&
            this.day == date.day) return true;
        else return false;
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

    /// <summary>
    /// NOT SYMETRIC!!! 
    /// </summary>
    /// <param name="a">Full Date, no value in a can be -1 unless the same value in b is -1, or else strange things will happen!</param>
    /// <param name="b">can be a partial date</param>
    /// <returns></returns>
    public static WorldTime operator +(WorldTime a, WorldTime b)
    {
        WorldTime c = new WorldTime(a);
        
        if(b.year > 0) c.AdvanceYear(b.year);
        if (b.month > 0) c.AdvanceMonth(b.month);
        if (b.day > 0) c.AdvanceDay(b.day);
        if (b.hour > 0) c.AdvanceHour(b.hour);
        if (b.minute > 0) c.Tick(b.minute);

        return c;
    }


    public int ConvertToDayCount()
    {
        int count = year * MonthInYear;
        count += month;
        count *= DayInMonth;
        count += day;

        return count;
    }

    public int ConvertToMinuteCount()
    {
        int count = ConvertToDayCount();
        count *= HourInDay;
        count += hour;
        count *= MinInHour;
        count += minute;

        return count;
    }

    public CasualTimeBlocks GetGeneralDifference(WorldTime other)
    {
        int dayCount = ConvertToDayCount();
        int otherCount = other.ConvertToDayCount();

        int dif = dayCount - otherCount;

        if (dif > (MonthInYear * DayInMonth) * 2) return CasualTimeBlocks.years;
        if (dif >= (MonthInYear * DayInMonth)) return CasualTimeBlocks.lastyear;
        if (dif > DayInMonth * 2) return CasualTimeBlocks.months;
        if (dif >= DayInMonth) return CasualTimeBlocks.lastmonth;

        if (dif > 14) return CasualTimeBlocks.weeks;
        if (dif >= 7) return CasualTimeBlocks.lastweek;
        if (dif > 1) return CasualTimeBlocks.days;
        if (dif == 1) return CasualTimeBlocks.yesterday;
        if (dif == 0) return CasualTimeBlocks.today;

        return CasualTimeBlocks.future;
    }
}


public class TimeObligation
{
    public WorldTime start;
    public WorldTime end;

    public TimeObligation(WorldTime start, WorldTime end)
    {
        this.start = start;
        this.end = end;
    }


    public bool Overlapping(TimeObligation other)
    {
        if ((this.start <= other.start && other.start <= this.end) ||
            (other.start <= this.start && this.start <= other.end)) {
            return true;
        } else return false;
    }

    public bool HappensBefore(TimeObligation other)
    {
        if (this.end <= other.start) return true;
        else return false;
    }


}