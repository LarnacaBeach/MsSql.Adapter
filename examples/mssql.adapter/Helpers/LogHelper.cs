using Microsoft.Net.Http.Headers;
using Serilog;
using System.Web;

namespace mssql.adapter.helpers;

public static class LogHelper
{
    public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        var request = httpContext.Request;

        // Set all the common properties available for every request
        diagnosticContext.Set("Host", request.Host);
        diagnosticContext.Set("Protocol", request.Protocol);
        diagnosticContext.Set("Scheme", request.Scheme);

        // Only set it if available
        if (request.QueryString.HasValue)
        {
            diagnosticContext.Set("QueryString", request.QueryString.Value);
        }

        // Set the content-type of the Response at this point
        diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

        // Retrieve the IEndpointFeature selected for the request
        var endpoint = httpContext.GetEndpoint();

        if (endpoint is not null)
        {
            diagnosticContext.Set("EndpointName", endpoint.DisplayName);
        }

        // Parse baggage header according to the current working draft https://www.w3.org/TR/baggage/
        if (request.Headers.TryGetValue(HeaderNames.Baggage, out var baggages))
        {
            foreach (var baggageString in baggages)
            {
                var listMembers = baggageString.Split(',');

                foreach (var listMember in listMembers)
                {
                    var (key, value) = listMember.Split('=', 2) switch { var a => (a[0]?.Trim(), a[1]?.Trim()) };

                    if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                    {
                        continue;
                    }

                    diagnosticContext.Set(key, HttpUtility.UrlDecode(value));
                }
            }
        }
    }
}