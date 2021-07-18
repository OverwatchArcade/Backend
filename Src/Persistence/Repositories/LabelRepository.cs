using AutoMapper;
using OWArcadeBackend.Models.Overwatch;
using OWArcadeBackend.Persistence.Repositories.Interfaces;

namespace OWArcadeBackend.Persistence.Repositories
{
    public class LabelRepository : Repository<Label>, ILabelRepository
    {
        private readonly IMapper _mapper;

        public LabelRepository(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
        }
    }
}
