namespace DatingAgencyMS.Domain.Models.Business;

public enum MeetingResult
{
    Pending,
    Match,
    NoMatch,
    Cancelled
}

public static class MeetingResultHelper
{
    public static string ToUkrainian(MeetingResult meetingResult)
        => meetingResult switch
        {
            MeetingResult.Pending => "Очікується",
            MeetingResult.Match => "Підходять",
            MeetingResult.NoMatch => "Не підходять",
            MeetingResult.Cancelled => "Скасовано",
            _ => throw new ArgumentOutOfRangeException(nameof(meetingResult), meetingResult, null)
        };

    public static MeetingResult FromUkrainianToEnum(string ukrainianMeetingResult)
        => ukrainianMeetingResult switch
        {
            "Очікується" => MeetingResult.Pending,
            "Підходять" => MeetingResult.Match,
            "Не підходять" => MeetingResult.NoMatch,
            "Скасовано" => MeetingResult.Cancelled,
            _ => throw new ArgumentOutOfRangeException(nameof(ukrainianMeetingResult), ukrainianMeetingResult, null)
        };
}