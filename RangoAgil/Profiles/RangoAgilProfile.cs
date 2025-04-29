using AutoMapper;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;

namespace RangoAgil.API.Profiles;
public class RangoAgilProfile : Profile {
    public RangoAgilProfile() {
        CreateMap<Rango, RangoDTO>().ReverseMap(); //cria o mapa entre Rango e RangoDTO
        CreateMap<Rango, RangoParaCriacaoDTO>().ReverseMap();
        CreateMap<Rango, RangoParaAtualizacaoDTO>().ReverseMap();

        CreateMap<Ingrediente, IngredienteDTO>()//cria o mapa entre Ingrediente e IngredienteDTO
            .ForMember(
            d => d.RangoId,//Está dizendo que você quer configurar como preencher a propriedade RangoId no DTO
            o => o.MapFrom(s => s.Rangos.First().Id)); //Define de onde vem o valor.
    }
}
