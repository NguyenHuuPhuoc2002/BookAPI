using EcommerceWeb.ViewModels;

namespace EcommerceWeb.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestMode model);

        //Dang key-value (IQueryCollection)
        VnPaymentResponseModel PaymentExecute(IQueryCollection collection);
    }
}
