namespace SpinTrack.Application.Features.BusinessDays.DTOs
{
    public class UpdateBusinessDayRequest
    {
        public bool IsWorkingDay { get; set; } = true;
        public bool IsWeekend { get; set; } = false;
        public string? Remarks { get; set; }
    }
}