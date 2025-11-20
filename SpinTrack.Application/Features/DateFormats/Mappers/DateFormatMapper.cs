using SpinTrack.Application.Features.DateFormats.DTOs;
using SpinTrack.Core.Entities.DateFormat;

namespace SpinTrack.Application.Features.DateFormats.Mappers
{
    public static class DateFormatMapper
    {
        public static DateFormatDto ToDateFormatDto(DateFormat df)
        {
            return new DateFormatDto
            {
                DateFormatId = df.DateFormatId,
                FormatString = df.FormatString,
                Description = df.Description,
                IsDefault = df.IsDefault,
                CreatedAt = df.CreatedAt
            };
        }

        public static DateFormatDetailDto ToDateFormatDetailDto(DateFormat df)
        {
            return new DateFormatDetailDto
            {
                DateFormatId = df.DateFormatId,
                FormatString = df.FormatString,
                Description = df.Description,
                IsDefault = df.IsDefault,
                CreatedAt = df.CreatedAt,
                ModifiedAt = df.ModifiedAt
            };
        }

        public static DateFormat ToEntity(CreateDateFormatRequest request)
        {
            return new DateFormat
            {
                DateFormatId = Guid.NewGuid(),
                FormatString = request.FormatString,
                Description = request.Description,
                IsDefault = request.IsDefault
            };
        }

        public static void UpdateEntity(DateFormat df, UpdateDateFormatRequest request)
        {
            df.Description = request.Description;
            df.IsDefault = request.IsDefault;
        }
    }
}