﻿using OWArcadeBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using OWArcadeBackend.Dtos;

namespace OWArcadeBackend.Services.ContributorService
{
    public interface IContributorService
    {
        Task<ServiceResponse<List<ContributorDto>>> GetAllContributors();
        Task<ServiceResponse<ContributorDto>> GetContributorByUsername(string username, bool withStats = true);
    }
}
