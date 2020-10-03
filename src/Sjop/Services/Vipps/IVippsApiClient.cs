using System.Threading;
using System.Threading.Tasks;
using Sjop.Models;
using static Sjop.Services.Vipps.VippsApiClient;

namespace Sjop.Services.Vipps
{
    public interface IVippsApiClient
    {
        Task<InitiatePaymentResponseOk> InitiatePayment(Order order, VippsSettings vippsSettings);
        Task<CapturePaymentResponseOk> CapturePayment(Order order, VippsSettings vippsSettings);
    }
}