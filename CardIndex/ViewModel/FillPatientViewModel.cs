using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using CardIndex.Model;
using CardIndexDal;
using Unity;

namespace CardIndex.ViewModel
{
   public class FillPatientViewModel : BaseViewModel
   {
      private readonly CardIndexDbContext _dbContext;
      private IMapper _mapper;


      public FillPatientViewModel(IUnityContainer container)
      {
         var dbContext = container.Resolve<CardIndexDbContext>();
         _mapper = container.Resolve<IMapper>();
         _dbContext = dbContext;
            //         _dbContext.PatientCards.Add(new PatientCard()
            //            {Address = "Asd", DateOfBirth = DateTime.Now, Fio = "Fiodf", Gender = Gender.Female, Phone = ""});
            //         _dbContext.SaveChanges();
            /* PatientCards =
                new ObservableCollection<PatientCard>(
                   mapper.Map<List<PatientCard>>(_dbContext.PatientCards.ToList()));*/
      }
   }
}
