using AutoMapper;
using OWArcadeBackend.Dtos;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Overwatch;
using System;

namespace OWArcadeBackend
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Daily, DailyDto>()
                .ForMember(dest => dest.Modes, opt => opt.MapFrom(src => src.TileModes));

            CreateMap<Contributor, ContributorDto>()
                .ForMember(dest => dest.Avatar,
                    opt => opt.MapFrom(src =>
                        Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.IMG_PROFILE_FOLDER + src.Avatar));

            CreateMap<TileMode, TileModeDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ArcadeMode.Name))
                .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.ArcadeMode.Players))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ArcadeMode.Description))
                .ForMember(dest => dest.Image,
                    opt => opt.MapFrom(src =>
                        Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.IMG_OW_ARCADE_FOLDER +
                        src.ArcadeMode.Image))
                .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Label.Value));

            CreateMap<ArcadeMode, ArcadeModeDto>()
                .ForMember(dest => dest.Image,
                    opt => opt.MapFrom(src =>
                        Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.IMG_OW_ARCADE_FOLDER + src.Image));
            
            CreateMap<ConfigOverwatchMap, ConfigOverwatchMap>()
                .ForMember(dest => dest.Image,
                    opt => opt.MapFrom(src =>
                        Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.IMG_OW_MAPS_FOLDER + src.Image));
            
            CreateMap<ConfigOverwatchHero, ConfigOverwatchHero>()
                .ForMember(dest => dest.Image,
                    opt => opt.MapFrom(src =>
                        Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.IMG_OW_HEROES_FOLDER + src.Image));
        }
    }
}