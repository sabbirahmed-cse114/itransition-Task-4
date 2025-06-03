namespace TaskFour.Models
{
    public class TimeCalculator
    {
        public static string GetTimeAgo(DateTime? dateTime)
        {
            var timeSpan = DateTime.Now.Subtract(dateTime.Value);

            if (timeSpan.TotalSeconds < 1)
                return "Active";
            if (timeSpan.TotalSeconds < 60)
                return "less than a minute ago";
            if (timeSpan.TotalMinutes < 60)
                return $"{Math.Floor(timeSpan.TotalMinutes)} minutes ago";
            if (timeSpan.TotalHours < 2)
                return $"{Math.Floor(timeSpan.TotalHours)} hour ago";
            if (timeSpan.TotalHours < 24)
                return $"{Math.Floor(timeSpan.TotalHours)} hours ago";
            if (timeSpan.TotalDays < 2)
                return $"{Math.Floor(timeSpan.TotalDays)} day ago";
            if (timeSpan.TotalDays < 7)
                return $"{Math.Floor(timeSpan.TotalDays)} days ago";
            if (timeSpan.TotalDays < 14)
                return $"{Math.Floor(timeSpan.TotalDays / 7)} week ago";
            if (timeSpan.TotalDays < 30)
                return $"{Math.Floor(timeSpan.TotalDays / 7)} weeks ago";
            if (timeSpan.TotalDays < 60)
                return $"{Math.Floor(timeSpan.TotalDays / 30)} month ago";
            if (timeSpan.TotalDays < 365)
                return $"{Math.Floor(timeSpan.TotalDays / 30)} months ago";

            return $"{Math.Floor(timeSpan.TotalDays / 365)} years ago";
        }

    }
}
