using AutoMapper;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Overwatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OWArcadeBackend.Persistence.Repositories.Interfaces;

namespace OWArcadeBackend.Persistence.Repositories
{
    public class ConfigRepository : Repository<Config>, IConfigRepository
    {
        private readonly IMapper _mapper;

        public ConfigRepository(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
        }


    }
}
