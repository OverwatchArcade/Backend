using AutoMapper;
using OWArcadeBackend.Models;
using OWArcadeBackend.Persistence.Repositories.Interfaces;


namespace OWArcadeBackend.Persistence.Repositories
{
    public class AuthRepository : Repository<Contributor>, IAuthRepository
    {
        private readonly IMapper _mapper;

        public AuthRepository(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
        }
        
    }
}
