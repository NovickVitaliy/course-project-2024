using DatingAgencyMS.Domain.Models.Business;

namespace DatingAgencyMS.Application.DTOs.Meetings;

public record MeetingDto(
    int MeetingId,
    DateTime Date,
    int InviterId,
    int InviteeId,
    string Location,
    MeetingResult Result);