using Ecommerce.Domain.Enums;

namespace Ecommerce.Domain.Entities;

public class JerseySizeStock
{
    public Guid Id { get; init; } = Guid.NewGuid();
    
    public Guid JerseyId { get; set; }
    public Jersey? Jersey { get; set; }

    public JerseySize Size { get; set; }
    public int StockQuantity { get; set; }
}
