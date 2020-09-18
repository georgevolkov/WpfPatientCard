using System.Configuration;
using System.Windows;
using CardIndex.Mapper;
using CardIndex.View;
using CardIndex.ViewModel;
using CardIndexDal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Unity;
using Unity.Injection;

namespace CardIndex
{
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application
   {
      private const string ConnectionStringName = "CardIndexConnectionString";

      protected override void OnStartup(StartupEventArgs e)
      {
          IUnityContainer container = new UnityContainer();

          container.RegisterType<MapperConfigurator, MapperConfigurator>();
          var opt = container.Resolve<DbContextOptionsBuilder<CardIndexDbContext>>();
          opt.UseSqlServer(ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString);

          container.RegisterType<CardIndexDbContext>(new InjectionConstructor(opt.Options));
          var serviceCollection = new ServiceCollection();
          serviceCollection.AddDbContext<CardIndexDbContext>(options =>
              options.UseSqlServer(
                  @"Data Source=.\SQLEXPRESS;Initial Catalog=CardIndex;User=user;Password=qwe123qwe123"), ServiceLifetime.Transient);

          var mapperConfiguration = container.Resolve<MapperConfigurator>().Configure();

          var mapper = mapperConfiguration.CreateMapper();
          container.RegisterInstance(mapper);

          container.RegisterType<FillPatientViewModel>();
          container.RegisterType<FillPatientCardView>();
          container.RegisterType<MainViewModel>();
          container.RegisterType<AddPatientCardView>();
          container.RegisterType<AddPatientCardViewModel>();
          container.RegisterType<FillVisitView>();

          var mainWindow = container.Resolve<MainWindow>();
          mainWindow.Show();
      }
   }
}
