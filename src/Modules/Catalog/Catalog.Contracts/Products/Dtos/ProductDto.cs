using Microsoft.AspNetCore.Http;

namespace Catalog.Contracts.Products.Dtos;

public record ProductDto(
    Guid? Id,
    string Name,
    List<string> Category,
    string Description,
    //string ImageFile,
    decimal Price
    );

public record ProductQueryDto(
    Guid? Id,
    string Name,
    List<string> Category,
    string Description,
    decimal Price,
    string? ImageUrl
    );

