using System.ComponentModel.DataAnnotations;

namespace RangoAgil.API.Models; 
public class RangoParaCriacaoDTO {
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public required string Nome { get; set; }

}
