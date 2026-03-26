using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Mvc;

namespace TheHouseBebidas.WineReviews.Api.Controllers;

[ApiController]
[Route("api/images")]
public sealed class ImagesController : ControllerBase
{
    private static readonly HashSet<string> AllowedHosts = new(StringComparer.OrdinalIgnoreCase)
    {
        "drive.google.com",
        "drive.usercontent.google.com",
        "lh3.googleusercontent.com",
        "images.unsplash.com"
    };

    private readonly IHttpClientFactory _httpClientFactory;

    public ImagesController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("proxy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
    public async Task<IActionResult> ProxyAsync([FromQuery] string? url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return BadRequest("Missing required query parameter: url");
        }

        if (!TryValidateImageUrl(url, out var targetUri, out var validationError))
        {
            return BadRequest(validationError);
        }

        if (!await IsDnsPublicAsync(targetUri.Host, cancellationToken))
        {
            return BadRequest("Blocked host.");
        }

        var client = _httpClientFactory.CreateClient("ImageProxyClient");
        using var request = new HttpRequestMessage(HttpMethod.Get, targetUri);
        request.Headers.Accept.ParseAdd("image/*");

        HttpResponseMessage upstreamResponse;

        try
        {
            upstreamResponse = await client.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return StatusCode(StatusCodes.Status504GatewayTimeout, "Image upstream timeout.");
        }
        catch (HttpRequestException)
        {
            return StatusCode(StatusCodes.Status502BadGateway, "Image upstream failed.");
        }

        if (!upstreamResponse.IsSuccessStatusCode)
        {
            upstreamResponse.Dispose();
            return StatusCode((int)upstreamResponse.StatusCode, "Image upstream returned an error.");
        }

        var contentType = upstreamResponse.Content.Headers.ContentType?.MediaType;
        if (string.IsNullOrWhiteSpace(contentType) || !contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            upstreamResponse.Dispose();
            return StatusCode(StatusCodes.Status502BadGateway, "Upstream response is not an image.");
        }

        var imageStream = await upstreamResponse.Content.ReadAsStreamAsync(cancellationToken);
        HttpContext.Response.RegisterForDispose(upstreamResponse);
        Response.Headers.CacheControl = "public,max-age=300";

        return File(imageStream, contentType);
    }

    private static bool TryValidateImageUrl(string rawUrl, out Uri uri, out string error)
    {
        uri = default!;
        error = string.Empty;

        if (!Uri.TryCreate(rawUrl, UriKind.Absolute, out var parsedUri))
        {
            error = "Invalid image URL.";
            return false;
        }

        if (!string.Equals(parsedUri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(parsedUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        {
            error = "Only http and https URLs are allowed.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(parsedUri.Host))
        {
            error = "Image URL host is required.";
            return false;
        }

        if (IsBlockedHost(parsedUri.Host))
        {
            error = "Blocked host.";
            return false;
        }

        if (!AllowedHosts.Contains(parsedUri.Host))
        {
            error = "Host not allowed.";
            return false;
        }

        if (IPAddress.TryParse(parsedUri.Host, out var hostAddress) && !IsPublicAddress(hostAddress))
        {
            error = "Blocked host.";
            return false;
        }

        uri = parsedUri;
        return true;
    }

    private static bool IsBlockedHost(string host)
    {
        return string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase)
            || host.EndsWith(".localhost", StringComparison.OrdinalIgnoreCase)
            || host.EndsWith(".local", StringComparison.OrdinalIgnoreCase)
            || string.Equals(host, "127.0.0.1", StringComparison.OrdinalIgnoreCase)
            || string.Equals(host, "::1", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<bool> IsDnsPublicAsync(string host, CancellationToken cancellationToken)
    {
        try
        {
            var addresses = await Dns.GetHostAddressesAsync(host, cancellationToken);
            return addresses.Length > 0 && addresses.All(IsPublicAddress);
        }
        catch (SocketException)
        {
            return false;
        }
    }

    private static bool IsPublicAddress(IPAddress address)
    {
        if (IPAddress.IsLoopback(address))
        {
            return false;
        }

        if (address.AddressFamily == AddressFamily.InterNetwork)
        {
            var bytes = address.GetAddressBytes();

            if (bytes[0] == 10 || bytes[0] == 127)
            {
                return false;
            }

            if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
            {
                return false;
            }

            if (bytes[0] == 192 && bytes[1] == 168)
            {
                return false;
            }

            if (bytes[0] == 169 && bytes[1] == 254)
            {
                return false;
            }
        }
        else if (address.AddressFamily == AddressFamily.InterNetworkV6)
        {
            if (address.IsIPv6LinkLocal || address.IsIPv6Multicast || address.IsIPv6SiteLocal || address.IsIPv6Teredo)
            {
                return false;
            }

            var bytes = address.GetAddressBytes();
            if ((bytes[0] & 0xFE) == 0xFC)
            {
                return false;
            }
        }

        return true;
    }
}
