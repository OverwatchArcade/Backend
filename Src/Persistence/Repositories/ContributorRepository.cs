using AutoMapper;
using OWArcadeBackend.Models;
using OWArcadeBackend.Persistence.Repositories.Interfaces;

namespace OWArcadeBackend.Persistence.Repositories
{
    public class ContributorRepository : Repository<Contributor>, IContributorRepository
    {
        private readonly IMapper _mapper;

        public ContributorRepository(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
        }
    }
}