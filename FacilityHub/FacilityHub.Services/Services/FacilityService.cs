using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacilityHub.Core.Entities;
using FacilityHub.Core.Contracts;
using FacilityHub.Services.Interfaces;
using System.Net;
using FacilityHub.Services.helper;

namespace FacilityHub.Services.Services
{
    public class FacilityService(IUnitOfWork unitOfWork) : IFacilityService
    {
        public async Task<ServiceResult<IReadOnlyList<Facility>>> GetAllFacilitiesAsync()
        {
            try
            {
            var facilities = await unitOfWork.FacilityRepository.GetAllAsync();
            return ServiceResult<IReadOnlyList<Facility>>.Success(facilities, "Facilities fetched successfully", HttpStatusCode.OK);
        
            }
            catch (System.Exception)
            {
                
              return ServiceResult<IReadOnlyList<Facility>>.Failed("An error occurred while fetching facilities", "FETCH_FACILITIES_ERROR", new string[] { "An error occurred while fetching facilities" }, HttpStatusCode.InternalServerError);
            }
   }

        public async Task<ServiceResult<Facility>> GetFacilityByIdAsync(Guid facilityId)
        {
            try
            {
                var facility = await unitOfWork.FacilityRepository.FindOneAsync(e => e.Id == facilityId);
                if (facility is null)
                    throw new ServiceException("Facility not found", "FACILITY_NOT_FOUND", new string[] { "Facility not found" }, HttpStatusCode.NotFound);
                return ServiceResult<Facility>.Success(facility, "Facility fetched successfully", HttpStatusCode.OK);
            }
            catch (ServiceException ex)
            {
                return ServiceResult<Facility>.Failed(ex.Message, ex.ErrorCode, ex.Errors, ex.StatusCode);
            }
            catch (System.Exception)
            {
                return ServiceResult<Facility>.Failed("An error occurred while fetching facility", "FETCH_FACILITY_ERROR", new string[] { "An error occurred while fetching facility" }, HttpStatusCode.InternalServerError);
            }
        }
    }
}