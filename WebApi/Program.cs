using ApplicationCore.GenericServices.GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.OpenApi.Models;
using ApplicationCore.Data.GenericServise;
using ApplicationCore.Services.IService;
using WebApplication1.Repositories.Implementations;
using AutoMapper;
using ApplicationCore.Services;
using WebApi;
using WebApi.Models;

using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using Microsoft.OData.Edm;

var builder = WebApplication.CreateBuilder(args);

// --- Chaîne de connexion ---
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(defaultConnection))
    throw new InvalidOperationException("La chaîne de connexion est vide.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(defaultConnection));

// --- AutoMapper & Services ---
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));


builder.Services.AddAutoMapper(typeof(AutomapperProfiles));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// --- OData Configuration ---
builder.Services.AddControllers()
    .AddOData(opt =>
        opt.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100)
            .AddRouteComponents("odata", GetEdmModel())
    );
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
// --- Swagger, Auth, CORS ---
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// --- OData EDM Model ---
IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<Employee>("Employees");
    builder.EntitySet<LeaveRequest>("LeaveRequests");
    return builder.GetEdmModel();
}
