using System;

namespace Sirius2Ch
{
    public static class Util
    {
        public static string TimeAgo(this DateTime d)
        {
            var s = DateTime.Now.Subtract(d);

            var dayDiff = s.TotalDays;
            var secDiff = s.TotalSeconds;
            var monDiff = dayDiff / 31;
            
            if (dayDiff < 0)
                return "Not yet";
            
            if ((int)dayDiff == 0)
            {
                if (secDiff < 20)
                    return "just now";
                if (secDiff < 60)
                    return $"{(int)secDiff} seconds ago";
                if (secDiff < 120)
                    return "1 minute ago";
                if (secDiff < 3600)
                    return $"{Math.Floor(secDiff / 60)} minutes ago";
                if (secDiff < 7200)
                    return "1 hour ago";
                if (secDiff < 86400)
                    return $"{Math.Floor(secDiff / 3600)} hours ago";
            }
            
            if ((int)dayDiff == 1)
                return "yesterday";
            if (dayDiff < 7)
                return $"{(int)dayDiff} days ago";
            if (dayDiff < 31)
                return $"{Math.Ceiling(dayDiff / 7)} weeks ago";

            if (monDiff < 12)
            {
                if ((int) monDiff == 1)
                    return "month ago";
                return $"{(int) monDiff} months ago";
            }

            return $"{(int)(dayDiff / 365)} years ago";
        }
    }
}