using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using OverwatchArcade.API.Dtos.Contributor;
using OverwatchArcade.API.Dtos.Overwatch;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.ContributorInformation;
using OverwatchArcade.Domain.Models.Overwatch;

namespace OverwatchArcade.API
{
    [ExcludeFromCodeCoverage]
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Daily, DailyDto>()
                .ForMember(dest => dest.Modes, opt => opt.MapFrom(src => src.TileModes));

            CreateMap<CreateDailyDto, Daily>();
            CreateMap<CreateTileModeDto, TileMode>();
            
            CreateMap<Daily, CreateDailyDto>();

            CreateMap<ContributorProfileDto, ContributorProfile>();

            CreateMap<ArcadeMode, ArcadeModeDto>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.OwArcadeFolder + src.Image));
            
            CreateMap<Contributor, ContributorDto>()
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.ProfileFolder + src.Avatar));

            CreateMap<ContributorStats, ContributorStatsDto>();
            
            CreateMap<TileMode, TileModeDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ArcadeMode.Name))
                .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.ArcadeMode.Players))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ArcadeMode.Description))
                .ForMember(dest => dest.Image,opt => opt.MapFrom(src => Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.OwArcadeFolder + src.ArcadeMode.Image))
                .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Label.Value));

            CreateMap<TileMode, CreateTileModeDto>();
        }
    }
}