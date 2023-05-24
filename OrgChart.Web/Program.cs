var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddMvc(options =>
{
    options.Filters.Add<OrgChart.Web.AuthorizeUserFilter>();
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(14);
});

builder.Services.AddAuthentication("OrgChart_CookieAuth")
                .AddCookie("OrgChart_CookieAuth", options =>
                {
                    options.LogoutPath = new PathString("/Index");
                    options.ExpireTimeSpan = TimeSpan.FromDays(14);
                    options.LoginPath = new PathString("/Index");
                    options.Cookie.Name = "OrgChart_CookieAuth";
                    options.AccessDeniedPath = new PathString("/Error");
                });
builder.Services.AddAutoMapper(typeof(OrgChart.Web.AutoMapperConfig.UserProfile));
builder.Services.AddMemoryCache();

#region Dependancy Injection
//Managers
builder.Services.AddSingleton<OrgChart.Business.IHierarchyManager, OrgChart.Business.HierarchyManager>();
builder.Services.AddSingleton<OrgChart.Business.IBistrainerManager, OrgChart.Business.BistrainerManager>();
//Repositories
builder.Services.AddSingleton<OrgChart.Repository.IEmployeeRepository, OrgChart.Repository.EmployeeRepository>();
builder.Services.AddSingleton<OrgChart.Repository.IDepartmentRepository, OrgChart.Repository.DepartmentRepository>();
builder.Services.AddSingleton<OrgChart.Repository.ISubDepartmentRepository, OrgChart.Repository.SubDepartmentRepository>();
builder.Services.AddSingleton<OrgChart.Repository.IJobRepository, OrgChart.Repository.JobRepository>();
builder.Services.AddSingleton<OrgChart.Repository.IHierarchyRepository, OrgChart.Repository.HierarchyRepository>();
builder.Services.AddSingleton<OrgChart.Repository.IBistrainerLocationRepository, OrgChart.Repository.BistrainerLocationRepository>();
builder.Services.AddSingleton<OrgChart.Repository.IBistrainerUserRepository, OrgChart.Repository.BistrainerUserRepository>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();
app.UseSession();
app.UseCookiePolicy();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
