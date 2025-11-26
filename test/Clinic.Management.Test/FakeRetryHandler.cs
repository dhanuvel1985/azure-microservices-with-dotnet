using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class FakeRetryHandler : DelegatingHandler
{
    public int CallCount { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        CallCount++;

        // First 2 attempts fail
        if (CallCount < 3)
        {
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }

        // Success on 3rd attempt
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("Success")
        };
    }
}
