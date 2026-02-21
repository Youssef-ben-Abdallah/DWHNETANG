using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetDwhProject.Core.Contracts;
using NetDwhProject.Core.Entities;
using NetDwhProject.Infrastructure;
namespace NetDwhProject.Api.Controllers;
[ApiController,Route("api/auth")]public class AuthController(IAuthService auth):ControllerBase{[HttpPost("login")]public async Task<IActionResult> Login(LoginRequest r){var x=await auth.LoginAsync(r);return x==null?Unauthorized():Ok(x);}[Authorize,HttpGet("me")]public async Task<IActionResult> Me(){var id=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??User.FindFirstValue(ClaimTypes.Name)??"0");var x=await auth.MeAsync(id);return x==null?NotFound():Ok(x);}}
[ApiController,Route("api/categories"),Authorize]public class CategoriesController(ICategoryRepository repo):ControllerBase{[HttpGet]public async Task<IActionResult> Get()=>Ok(await repo.GetAllAsync());[HttpPost,Authorize(Roles="Admin")]public async Task<IActionResult> Post(Category c)=>Ok(await repo.AddAsync(c));[HttpPut("{id}"),Authorize(Roles="Admin")]public async Task<IActionResult> Put(int id,Category c){c.Id=id;await repo.UpdateAsync(c);return NoContent();}[HttpDelete("{id}"),Authorize(Roles="Admin")]public async Task<IActionResult> Del(int id){await repo.DeleteAsync(id);return NoContent();}}
[ApiController,Route("api/subcategories"),Authorize]public class SubCategoriesController(ISubCategoryRepository repo):ControllerBase{[HttpGet]public async Task<IActionResult> Get([FromQuery]int? categoryId)=>Ok(await repo.GetAllAsync(categoryId));[HttpPost,Authorize(Roles="Admin")]public async Task<IActionResult> Post(SubCategory c)=>Ok(await repo.AddAsync(c));[HttpPut("{id}"),Authorize(Roles="Admin")]public async Task<IActionResult> Put(int id,SubCategory c){c.Id=id;await repo.UpdateAsync(c);return NoContent();}[HttpDelete("{id}"),Authorize(Roles="Admin")]public async Task<IActionResult> Del(int id){await repo.DeleteAsync(id);return NoContent();}}
[ApiController,Route("api/products"),Authorize]public class ProductsController(IProductRepository repo):ControllerBase{[HttpGet]public async Task<IActionResult> Get([FromQuery]int? categoryId,[FromQuery]int? subCategoryId)=>Ok(await repo.GetAllProjectedAsync(categoryId,subCategoryId));[HttpPost,Authorize(Roles="Admin")]public async Task<IActionResult> Post(Product c)=>Ok(await repo.AddAsync(c));[HttpPut("{id}"),Authorize(Roles="Admin")]public async Task<IActionResult> Put(int id,Product c){c.Id=id;await repo.UpdateAsync(c);return NoContent();}[HttpDelete("{id}"),Authorize(Roles="Admin")]public async Task<IActionResult> Del(int id){await repo.DeleteAsync(id);return NoContent();}}
[ApiController,Route("api/orders"),Authorize]public class OrdersController(IOrderRepository repo):ControllerBase{int Uid()=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??"0");[HttpPost]public async Task<IActionResult> Create(CreateOrderRequest r)=>Ok(await repo.CreateAsync(Uid(),r));[HttpGet("my")]public async Task<IActionResult> My()=>Ok(await repo.GetMyAsync(Uid()));[HttpGet("{id}")]public async Task<IActionResult> Get(int id){var o=await repo.GetByIdAsync(id);if(o==null)return NotFound();if(!User.IsInRole("Admin")&&o.UserId!=Uid())return Forbid();return Ok(o);}[HttpPut("{id}/status"),Authorize(Roles="Admin")]public async Task<IActionResult> Status(int id,[FromQuery]OrderStatus status){await repo.UpdateStatusAsync(id,status);return NoContent();}}
[ApiController,Route("api/analytics"),Authorize]public class AnalyticsController(IAnalyticsRepository repo):ControllerBase{
[HttpGet("executive/kpis")]public async Task<IActionResult> Kpis(DateTime? from,DateTime? to)=>Ok(await repo.GetExecutiveKpisAsync(from,to));
[HttpGet("executive/sales-by-month")]public async Task<IActionResult> M(DateTime? from,DateTime? to)=>Ok(await repo.GetSalesByMonthAsync(from,to));
[HttpGet("executive/sales-by-territory-group")]public async Task<IActionResult> Tg(DateTime? from,DateTime? to)=>Ok(await repo.GetSalesByTerritoryGroupAsync(from,to));
[HttpGet("executive/top-products")]public async Task<IActionResult> Tp(DateTime? from,DateTime? to,int top=10)=>Ok(await repo.GetTopProductsAsync(from,to,top));
[HttpGet("executive/online-offline-share")]public async Task<IActionResult> Oo(DateTime? from,DateTime? to)=>Ok(await repo.GetOnlineOfflineShareAsync(from,to));
[HttpGet("executive/map-sales")]public async Task<IActionResult> Mp(DateTime? from,DateTime? to)=>Ok(await repo.GetMapSalesAsync(from,to));
[HttpGet("executive/order-lines-snapshot")]public async Task<IActionResult> Os(DateTime? from,DateTime? to,int take=50)=>Ok(await repo.GetOrderLineSnapshotAsync(from,to,take));
[HttpGet("trends/sales-by-period")]public async Task<IActionResult> Sp(DateTime? from,DateTime? to,string granularity="day")=>Ok(await repo.GetSalesByPeriodAsync(from,to,granularity));
[HttpGet("trends/sales-yoy")]public IActionResult Yoy()=>Ok(new{message="Use sales-by-period grouped by month/year for YoY overlay."});
[HttpGet("trends/cumulative-sales")]public async Task<IActionResult> Cs(DateTime? from,DateTime? to,string granularity="day")=>Ok(await repo.GetCumulativeSalesAsync(from,to,granularity));
[HttpGet("trends/heatmap-dow-month")]public IActionResult Hm()=>Ok(Array.Empty<object>());
[HttpGet("trends/kpis-mtd-ytd")]public async Task<IActionResult> Km(DateTime? asOf)=>Ok(await repo.GetKpisMtdYtdAsync(asOf));
[HttpGet("products/treemap-category-subcategory")]public IActionResult Pt()=>Ok(Array.Empty<object>());
[HttpGet("products/top-profit")]public async Task<IActionResult> Ptp(DateTime? from,DateTime? to,int top=15)=>Ok(await repo.GetTopProductsAsync(from,to,top));
[HttpGet("products/scatter-units-margin")]public IActionResult Psm()=>Ok(Array.Empty<object>());
[HttpGet("products/matrix")]public IActionResult Pm()=>Ok(Array.Empty<object>());
[HttpGet("customers/top")]public async Task<IActionResult> Ct(DateTime? from,DateTime? to,int top=20)=>Ok(await repo.GetTopCustomersAsync(from,to,top));
[HttpGet("customers/order-value-histogram")]public IActionResult Ch()=>Ok(Array.Empty<object>());
[HttpGet("customers/new-per-month")]public async Task<IActionResult> Cn(DateTime? from,DateTime? to)=>Ok(await repo.GetSalesByMonthAsync(from,to));
[HttpGet("customers/matrix-territory")]public IActionResult Cm()=>Ok(Array.Empty<object>());
[HttpGet("customers/kpis")]public async Task<IActionResult> Ck(DateTime? from,DateTime? to)=>Ok(await repo.GetCustomerKpisAsync(from,to));
[HttpGet("salespersons/leaderboard")]public IActionResult Sl()=>Ok(Array.Empty<object>());
[HttpGet("salespersons/by-territory")]public IActionResult Sbt()=>Ok(Array.Empty<object>());
[HttpGet("shipping/sales-by-shipmethod")]public async Task<IActionResult> Ss(DateTime? from,DateTime? to)=>Ok(await repo.GetSalesByShipMethodAsync(from,to));
[HttpGet("shipping/delivery-kpis")]public IActionResult Sd()=>Ok(new{avgDeliveryDays=0,lateLines=0,latePct=0});
[HttpGet("shipping/delivery-by-shipmethod")]public async Task<IActionResult> Sdb(DateTime? from,DateTime? to)=>Ok(await repo.GetSalesByShipMethodAsync(from,to));
[HttpGet("shipping/late-shipments")]public IActionResult Sls()=>Ok(Array.Empty<object>());
[HttpGet("currency/avg-rate")]public IActionResult Ca()=>Ok(Array.Empty<object>());
[HttpGet("currency/sales-by-currency")]public IActionResult Csc()=>Ok(new{formula="baseSales=LineTotal only",data=Array.Empty<object>()});
}
