using Ecommerce.Domain.Enums;

namespace Ecommerce.Application.Jerseys.Dtos;

public record JerseySizeStockDto(
    JerseySize Size,
    int StockQuantity
);
