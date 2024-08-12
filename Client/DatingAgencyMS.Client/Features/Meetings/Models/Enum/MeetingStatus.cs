using System.Text.RegularExpressions;
using DatingAgencyMS.Client.Models.Core;

namespace DatingAgencyMS.Client.Features.Meetings.Models.Enum;

public enum MeetingStatus
{
    Match,
    NoMatch,
    Canceled
}

public static class MeetingStatusHelper
{
    public static string ToUkrainianTranslation(MeetingStatus meetingStatus)
        => meetingStatus switch
        {
            MeetingStatus.Match => "Пари підійшли одне одному",
            MeetingStatus.NoMatch => "Пари не підійшли одне одному",
            MeetingStatus.Canceled => "Скасовано",
            _ => throw new ArgumentOutOfRangeException(nameof(meetingStatus), meetingStatus, null)
        };

    public static string ToUkrainianTranslationFromOldEnum(MeetingResult meetingResult)
        => meetingResult switch
        {
            MeetingResult.Pending => "Очікується",
            MeetingResult.Match => "Підійшли одне одному",
            MeetingResult.NoMatch => "Не підійшли одне одному",
            MeetingResult.Cancelled => "Скасовано",
            _ => throw new ArgumentOutOfRangeException(nameof(meetingResult), meetingResult, null)
        };
}