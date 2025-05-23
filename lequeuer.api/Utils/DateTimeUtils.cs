namespace lequeuer.api.Utils;

public static class DateTimeUtils
{
    public static DateTime JapanTime () => DateTime.UtcNow.AddHours(9);
}