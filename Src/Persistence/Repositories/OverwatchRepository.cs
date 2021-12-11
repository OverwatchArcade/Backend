using AutoMapper;
using OWArcadeBackend.Models.Overwatch;
using OWArcadeBackend.Persistence.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using OWArcadeBackend.Dtos.Overwatch;
using OWArcadeBackend.Models.Constants;

namespace OWArcadeBackend.Persistence.Repositories
{
    public class OverwatchRepository : Repository<ArcadeMode>, IOverwatchRepository
    {
        private readonly IMapper _mapper;

        public OverwatchRepository(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
        }

        public List<ArcadeModeDto> GetArcadeModes(Game overwatchType)
        {
            List<ArcadeMode> modes = MUnitOfWork.Context.ArcadeModes.Where(mode => mode.Game == overwatchType).ToList();
            return _mapper.Map<List<ArcadeModeDto>>(modes);
        }
        
        public List<Label> GetLabels()
        {
            List<Label> labels = MUnitOfWork.Context.Labels.ToList();
            return labels;
        }
    }
}
