using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RangoAgil.API.Entities;
public class Ingrediente {
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(200)]
    public required string Nome { get; set; }
    public ICollection<Rango> Rangos { get; set; } = new List<Rango>(); //tras todos os rangos que tem esse ingrediente. mesmo que colocar []
    public Ingrediente() {
           
    }
    [SetsRequiredMembers] //É um atributo que indica que um construtor (ou método) preenche todas as propriedades required de uma classe.
    public Ingrediente(int id, string nome) {
        Id = id;
        Nome = nome;            
    }
}

