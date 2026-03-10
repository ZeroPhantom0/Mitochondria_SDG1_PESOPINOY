using System;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;
using PesoPinoy.DAL.Data;
using PesoPinoy.BLL.Services;
using PesoPinoy.Models.Entities;
using PesoPinoy.UI.Forms;

namespace PesoPinoy.UI
{
    internal static class Program
    {
        public static IServiceProvider? ServiceProvider { get; private set; }
        public static User? CurrentUser { get; set; }
        public static IConfiguration? Configuration { get; private set; }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Load configuration
                Configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                // Setup DI
                var services = new ServiceCollection();
                ConfigureServices(services);
                ServiceProvider = services.BuildServiceProvider();

                // Verify ServiceProvider is not null
                if (ServiceProvider == null)
                    throw new Exception("Failed to initialize services");

                // Ensure database is created
                using (var scope = ServiceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    context.Database.EnsureCreated();

                    // Seed default admin if not exists
                    SeedDefaultAdmin(context);
                }

                Application.Run(new frmLogin());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Application failed to start: {ex.Message}", "Fatal Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration?.GetConnectionString("DefaultConnection")
                ?? "Data Source=..\\..\\INPUT_DATA\\pesopinoy.db";

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(connectionString));

            services.AddScoped<AuthenticationService>();
            services.AddScoped<LoanService>();
            services.AddScoped<RiskAnalysisService>();
            services.AddScoped<BorrowerService>();
            services.AddScoped<PaymentService>();
            services.AddScoped<SavingsService>();
            services.AddScoped<InsuranceService>();
            services.AddScoped<ReportService>();
            services.AddScoped<BackupService>();
        }

        private static void SeedDefaultAdmin(AppDbContext context)
        {
            if (!context.Users.Any())
            {
                var authService = new AuthenticationService(context);
                var admin = authService.CreateUserAsync("admin", "admin123", "System Administrator", "admin@pesopinoy.com").Result;
            }
        }
    }
}