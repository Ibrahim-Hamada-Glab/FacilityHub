using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacilityHub.Core.Entities;
using FacilityHub.Services.Dtos;
using FacilityHub.Services.helper;

namespace FacilityHub.Services.Interfaces
{
    public interface IFacilityService
    {
        /// <summary>
        /// Get all facilities
        /// </summary>
        /// <returns>List of facilities</returns>
        Task<ServiceResult<IReadOnlyList<Facility>>> GetAllFacilitiesAsync();

        /// <summary>
        /// Get facility by id
        /// </summary>
        /// <param name="facilityId">Facility id</param>
        /// <returns>Facility</returns>
        Task<ServiceResult<Facility>> GetFacilityByIdAsync(Guid facilityId);


        /// <summary>
        /// Create facility
        /// </summary>
        /// <param name="facility">Facility</param>
        /// <returns>Facility</returns>
        Task<ServiceResult<FacilityViewDto>> CreateFacilityAsync(CreateFacilityDto dto, string createdById, CancellationToken cancellationToken);

         
    }
}