using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeRates.Data.Entities;

[Table("Currencies")]
public class CurrencyEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string Code { get; set; }
    
    public string Name { get; set; }
    
    public string? Provider { get; set; }
    
    public DateTime? EffectiveDate { get; set; }
    
    public ICollection<CurrencyRateEntity> Rates { get; set; }
    
    public ICollection<CurrencyRateEntity> FromRates { get; set; }
}