using AutoMapper;
using PetMind.API.Models.DTOs.Cachorros;
using PetMind.API.Models.DTOs.Horarios;
using PetMind.API.Models.DTOs.PetShops;
using PetMind.API.Models.Entities;
using System.Globalization;

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
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => 
                    src.Data.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.GetCultureInfo("pt-BR"))))
                .ForMember(dest => dest.ValorTotal, opt => opt.MapFrom(src => src.ValorTotal))
                .ForMember(dest => dest.Cachorro, opt => opt.MapFrom(src => 
                    src.Cachorro != null ? src.Cachorro : null))
                .ForMember(dest => dest.PetShop, opt => opt.MapFrom(src => src.PetShop));

            // PetShop - MAPEAMENTOS SEPARADOS
            CreateMap<CreatePetShopDto, PetShop>();
            CreateMap<UpdatePetShopDto, PetShop>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            // Para lista geral (sem horários)
            CreateMap<PetShop, PetShopBasicDto>();
            
            // Para detalhes (com horários) - USANDO HorarioResponseDto
            CreateMap<PetShop, PetShopResponseDto>()
                .ForMember(dest => dest.Horarios, opt => opt.MapFrom(src => src.Horarios));

            // Mapeamentos auxiliares para DTOs aninhados
            CreateMap<Cachorro, CachorroInfoDto>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.NomeCachorro))
                .ForMember(dest => dest.NomeTutor, opt => opt.MapFrom(src => src.NomeTutor))
                .ForMember(dest => dest.ContatoTutor, opt => opt.MapFrom(src => src.ContatoTutor));

            CreateMap<PetShop, PetShopInfoDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.EnderecoPetShop, opt => opt.MapFrom(src => src.EnderecoPetShop));
        }
    }
}