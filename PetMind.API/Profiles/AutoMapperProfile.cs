using AutoMapper;
using PetMind.API.Models.DTOs.Cachorros;
using PetMind.API.Models.DTOs.Horarios;
using PetMind.API.Models.DTOs.PetShops;
using PetMind.API.Models.Entities;

namespace PetMind.API.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Cachorro
            CreateMap<CreateCachorroDto, Cachorro>();
            CreateMap<UpdateCachorroDto, Cachorro>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Cachorro, CachorroResponseDto>();

            // Horario
            CreateMap<CreateHorarioDto, Horario>();
            CreateMap<UpdateHorarioDto, Horario>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<Horario, HorarioResponseDto>()
                .ForMember(dest => dest.Cachorro, opt => opt.MapFrom(src => 
                    src.Cachorros != null && src.Cachorros.Any() 
                        ? src.Cachorros.First() 
                        : null))
                .ForMember(dest => dest.PetShop, opt => opt.MapFrom(src => src.PetShop));

            // PetShop
            CreateMap<CreatePetShopDto, PetShop>();
            CreateMap<UpdatePetShopDto, PetShop>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<PetShop, PetShopResponseDto>();

            // Mapeamentos auxiliares para DTOs aninhados
            CreateMap<Cachorro, CachorroInfoDto>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.NomeCachorro))
                .ForMember(dest => dest.NomeTutor, opt => opt.MapFrom(src => src.NomeTutor))
                .ForMember(dest => dest.ContatoTutor, opt => opt.MapFrom(src => src.ContatoTutor));

            CreateMap<PetShop, PetShopInfoDto>();
        }
    }
}