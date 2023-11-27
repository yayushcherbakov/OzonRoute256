using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkshopApp.Dal.Extensions;
using WorkshopApp.Dal.Repositories.Interfaces;
using WorkshopApp.TestingInfrastructure;

namespace WorkshopApp.IntegrationTests.Fixtures
{
    public class TestFixture
    {
        public IUserRepository UserRepository { get; }
        
        public ITaskRepository TaskRepository { get; }
        
        public ITaskLogRepository TaskLogRepository { get; }
        
        public ITakenTaskRepository TakenTaskRepository { get; }
        
        public IUserScheduleRepository UserScheduleRepository { get; }

        public TestFixture()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddDalInfrastructure(config)
                        .AddDalRepositories();
                })
                .Build();
            
            ClearDatabase(host);
            host.MigrateUp();

            var serviceProvider = host.Services;
            UserRepository = serviceProvider.GetRequiredService<IUserRepository>();
            TaskRepository = serviceProvider.GetRequiredService<ITaskRepository>();
            TaskLogRepository = serviceProvider.GetRequiredService<ITaskLogRepository>();
            TakenTaskRepository = serviceProvider.GetRequiredService<ITakenTaskRepository>();
            UserScheduleRepository = serviceProvider.GetRequiredService<IUserScheduleRepository>();
            
            FluentAssertionOptions.UseDefaultPrecision();
        }

        private static void ClearDatabase(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateDown(20230301);
        }
    }
}