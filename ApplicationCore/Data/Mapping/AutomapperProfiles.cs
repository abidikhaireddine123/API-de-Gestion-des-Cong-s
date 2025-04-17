using AutoMapper;
using WebApi.Models;



namespace WebApi
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            // Configuration des mappings
            CreateMap<LeaveRequest, LeaveRequestDto>();
            CreateMap<LeaveRequestDto, LeaveRequest>();

            // Ajoutez d'autres mappings ici si nécessaire
        }
    }
}