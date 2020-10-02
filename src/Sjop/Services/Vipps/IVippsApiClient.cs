using System.Threading;
using System.Threading.Tasks;

namespace Sjop.Services.Vipps
{
    public interface IVippsApiClient
    {
        Task<string> GetAccessToken(VippsSettings vippsSettings);

    }
}