using SpinTrack.Application.Features.Products.DTOs;
using SpinTrack.Core.Entities.Product;

namespace SpinTrack.Application.Features.Products.Mappers
{
    public static class ProductMapper
    {
        public static ProductDto ToProductDto(Product p)
        {
            return new ProductDto
            {
                ProductId = p.ProductId,
                ProductCode = p.ProductCode,
                ProductName = p.ProductName,
                Description = p.Description,
                CurrentVersion = p.CurrentVersion,
                ReleaseDate = p.ReleaseDate,
                TechnologyStack = p.TechnologyStack,
                CreatedAt = p.CreatedAt
            };
        }

        public static ProductDetailDto ToProductDetailDto(Product p)
        {
            return new ProductDetailDto
            {
                ProductId = p.ProductId,
                ProductCode = p.ProductCode,
                ProductName = p.ProductName,
                Description = p.Description,
                CurrentVersion = p.CurrentVersion,
                ReleaseDate = p.ReleaseDate,
                TechnologyStack = p.TechnologyStack,
                CreatedAt = p.CreatedAt,
                ModifiedAt = p.ModifiedAt
            };
        }

        public static Product ToEntity(CreateProductRequest request)
        {
            return new Product
            {
                ProductId = Guid.NewGuid(),
                ProductCode = request.ProductCode,
                ProductName = request.ProductName,
                Description = request.Description,
                CurrentVersion = request.CurrentVersion,
                ReleaseDate = request.ReleaseDate,
                TechnologyStack = request.TechnologyStack
            };
        }

        public static void UpdateEntity(Product p, UpdateProductRequest request)
        {
            p.ProductName = request.ProductName;
            p.Description = request.Description;
            p.CurrentVersion = request.CurrentVersion;
            p.ReleaseDate = request.ReleaseDate;
            p.TechnologyStack = request.TechnologyStack;
        }
    }
}