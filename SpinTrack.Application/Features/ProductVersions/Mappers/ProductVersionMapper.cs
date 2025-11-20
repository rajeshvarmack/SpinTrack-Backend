using SpinTrack.Application.Features.ProductVersions.DTOs;
using SpinTrack.Core.Entities.ProductVersion;

namespace SpinTrack.Application.Features.ProductVersions.Mappers
{
    public static class ProductVersionMapper
    {
        public static ProductVersionDto ToProductVersionDto(ProductVersion pv)
        {
            return new ProductVersionDto
            {
                ProductVersionId = pv.ProductVersionId,
                ProductId = pv.ProductId,
                VersionNumber = pv.VersionNumber,
                ReleaseDate = pv.ReleaseDate,
                ReleaseNotes = pv.ReleaseNotes,
                IsCurrent = pv.IsCurrent,
                CreatedAt = pv.CreatedAt
            };
        }

        public static ProductVersionDetailDto ToProductVersionDetailDto(ProductVersion pv)
        {
            return new ProductVersionDetailDto
            {
                ProductVersionId = pv.ProductVersionId,
                ProductId = pv.ProductId,
                VersionNumber = pv.VersionNumber,
                ReleaseDate = pv.ReleaseDate,
                ReleaseNotes = pv.ReleaseNotes,
                IsCurrent = pv.IsCurrent,
                CreatedAt = pv.CreatedAt,
                ModifiedAt = pv.ModifiedAt
            };
        }

        public static ProductVersion ToEntity(CreateProductVersionRequest request)
        {
            return new ProductVersion
            {
                ProductVersionId = Guid.NewGuid(),
                ProductId = request.ProductId,
                VersionNumber = request.VersionNumber,
                ReleaseDate = request.ReleaseDate,
                ReleaseNotes = request.ReleaseNotes,
                IsCurrent = request.IsCurrent
            };
        }

        public static void UpdateEntity(ProductVersion pv, UpdateProductVersionRequest request)
        {
            pv.ReleaseDate = request.ReleaseDate;
            pv.ReleaseNotes = request.ReleaseNotes;
            pv.IsCurrent = request.IsCurrent;
        }
    }
}