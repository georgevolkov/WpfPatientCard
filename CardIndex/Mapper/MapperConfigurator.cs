using AutoMapper;
using CardIndex.Model;
using CardIndexDal.Models;

namespace CardIndex.Mapper
{
    public class MapperConfigurator
    {
        public MapperConfiguration Configure()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PatientCard, PatientCardDto>();
                cfg.CreateMap<PatientCardDto, PatientCard>();
                cfg.CreateMap<VisitCard, VisitCardDto>();
                cfg.CreateMap<VisitCardDto, VisitCard>();
            });

            return configuration;
        }
    }
}
