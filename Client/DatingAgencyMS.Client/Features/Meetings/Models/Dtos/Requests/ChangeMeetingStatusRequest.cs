using System.ComponentModel.DataAnnotations;
using DatingAgencyMS.Client.Features.Meetings.Models.Enum;

namespace DatingAgencyMS.Client.Features.Meetings.Models.Dtos.Requests;

public class ChangeMeetingStatusRequest
{
    public required int MeetingId { get; init; }
    [Required(ErrorMessage = "Статус зустрічі не може бути пустим")]
    public MeetingStatus MeetingStatus { get; set; }
    public bool? InviterVisited { get; set; } = null;
    public bool? InviteeVisited { get; set; } = null;
}