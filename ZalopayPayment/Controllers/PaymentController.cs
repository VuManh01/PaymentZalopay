using Microsoft.AspNetCore.Mvc; // Thêm namespace cho Controller và IActionResult
using ZalopayPayment.Services;  // Thêm namespace cho ZaloPayService

namespace ZalopayPayment.Controllers
{
    [ApiController] // Đánh dấu class là API Controller
    [Route("api/[controller]")] // Định nghĩa route cho controller
    public class PaymentController : Controller
    {
        // Khai báo ZaloPayService
        private readonly ZaloPayService _zaloPayService;

        // Constructor inject ZaloPayService
        public PaymentController(ZaloPayService zaloPayService)
        {
            _zaloPayService = zaloPayService;
        }

        // Phương thức HttpPost để tạo đơn hàng
        [HttpPost("create-order")]
        public IActionResult CreateOrder([FromBody] CreateOrderRequest request)
        {
            // Gọi service để tạo đơn hàng ZaloPay
            var result = _zaloPayService.CreateOrder(request.Amount, request.Description);
            
            // Trả về kết quả dưới dạng JSON
            return Content(result, "application/json");
        }
    }

    // Lớp để nhận dữ liệu từ request body
    public class CreateOrderRequest
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
