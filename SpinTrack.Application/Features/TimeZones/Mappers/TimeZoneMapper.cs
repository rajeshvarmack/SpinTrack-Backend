using SpinTrack.Application.Features.TimeZones.DTOs;
using SpinTrack.Core.Entities.TimeZone;

namespace SpinTrack.Application.Features.TimeZones.Mappers
{
    public static class TimeZoneMapper
    {
        public static TimeZoneDto ToTimeZoneDto(TimeZoneEntity tz)
        {
            return new TimeZoneDto
            {
                TimeZoneId = tz.TimeZoneId,
                TimeZoneName = tz.TimeZoneName,
                GMTOffset = tz.GMTOffset,
                SupportsDST = tz.SupportsDST,
                CreatedAt = tz.CreatedAt
            };
        }

        public static TimeZoneDetailDto ToTimeZoneDetailDto(TimeZoneEntity tz)
        {
            return new TimeZoneDetailDto
            {
                TimeZoneId = tz.TimeZoneId,
                TimeZoneName = tz.TimeZoneName,
                GMTOffset = tz.GMTOffset,
                SupportsDST = tz.SupportsDST,
                CreatedAt = tz.CreatedAt,
                ModifiedAt = tz.ModifiedAt
            };
        }

        public static TimeZoneEntity ToEntity(CreateTimeZoneRequest request)
        {
            return new TimeZoneEntity
            {
                TimeZoneId = Guid.NewGuid(),
                TimeZoneName = request.TimeZoneName,
                GMTOffset = request.GMTOffset,
                SupportsDST = request.SupportsDST
            };
        }

        public static void UpdateEntity(TimeZoneEntity tz, UpdateTimeZoneRequest request)
        {
            tz.GMTOffset = request.GMTOffset;
            tz.SupportsDST = request.SupportsDST;
        }
    }
}