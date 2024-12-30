using System;
using System.Text.Json;
using RestSharp;
using Shuttle.Core.Contract;

namespace Shuttle.OAuth;

public static class RestResponseExtensions
{
    public static dynamic AsDynamic(this RestResponse response)
    {
        Guard.AgainstNull(response);

        if (response.Content == null)
        {
            throw new InvalidOperationException(Resources.NullContentException);
        }

        return JsonSerializer.Deserialize<JsonElement>(response.Content);
    }
}