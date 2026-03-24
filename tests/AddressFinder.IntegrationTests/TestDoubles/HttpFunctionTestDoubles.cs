using System.Collections.Specialized;
using System.Collections;
using System.Net;
using System.Security.Claims;
using System.Text;
using Azure.Core.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AddressFinder.IntegrationTests.TestDoubles;

internal sealed class FakeFunctionContext : FunctionContext
{
    public FakeFunctionContext()
    {
        InstanceServices = new ServiceCollection()
            .AddSingleton(Options.Create(new WorkerOptions
            {
                Serializer = new JsonObjectSerializer()
            }))
            .BuildServiceProvider();
    }

    public override string InvocationId => Guid.NewGuid().ToString("N");

    public override string FunctionId => "ParseAddress";

    public override TraceContext TraceContext => null!;

    public override BindingContext BindingContext => null!;

    public override RetryContext RetryContext => null!;

    public override IServiceProvider InstanceServices { get; set; } = null!;

    public override FunctionDefinition FunctionDefinition => null!;

    public override IDictionary<object, object> Items { get; set; } = new Dictionary<object, object>();

    public override IInvocationFeatures Features { get; } = new FakeInvocationFeatures();
}

internal sealed class FakeInvocationFeatures : IInvocationFeatures
{
    private readonly Dictionary<Type, object> _features = [];

    public TFeature? Get<TFeature>()
    {
        return _features.TryGetValue(typeof(TFeature), out var feature)
            ? (TFeature)feature
            : default;
    }

    public void Set<TFeature>(TFeature instance)
    {
        _features[typeof(TFeature)] = instance!;
    }

    public IEnumerator<KeyValuePair<Type, object>> GetEnumerator()
    {
        return _features.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal sealed class FakeHttpRequestData : HttpRequestData
{
    private readonly MemoryStream _body;

    public FakeHttpRequestData(FunctionContext functionContext, string jsonBody)
        : base(functionContext)
    {
        _body = new MemoryStream(Encoding.UTF8.GetBytes(jsonBody));
        Headers = new HttpHeadersCollection
        {
            { "Content-Type", "application/json" }
        };
    }

    public override Stream Body => _body;

    public override HttpHeadersCollection Headers { get; }

    public override IReadOnlyCollection<IHttpCookie> Cookies => Array.Empty<IHttpCookie>();

    public override Uri Url => new("http://localhost:7071/api/address/parse");

    public override IEnumerable<ClaimsIdentity> Identities => [];

    public override string Method => "POST";

    public override NameValueCollection Query => new();

    public override HttpResponseData CreateResponse()
    {
        return new FakeHttpResponseData(FunctionContext);
    }
}

internal sealed class FakeHttpResponseData : HttpResponseData
{
    public FakeHttpResponseData(FunctionContext functionContext)
        : base(functionContext)
    {
        Headers = new HttpHeadersCollection();
        Body = new MemoryStream();
        Cookies = new FakeHttpCookies();
    }

    public override HttpStatusCode StatusCode { get; set; }

    public override HttpHeadersCollection Headers { get; set; }

    public override Stream Body { get; set; }

    public override HttpCookies Cookies { get; }
}

internal sealed class FakeHttpCookies : HttpCookies
{
    private readonly List<IHttpCookie> _cookies = [];

    public override void Append(string name, string value)
    {
        _cookies.Add(new FakeHttpCookie { Name = name, Value = value });
    }

    public override void Append(IHttpCookie cookie)
    {
        _cookies.Add(cookie);
    }

    public override IHttpCookie CreateNew()
    {
        return new FakeHttpCookie();
    }
}

internal sealed class FakeHttpCookie : IHttpCookie
{
    public string Domain { get; set; } = string.Empty;

    public DateTimeOffset? Expires { get; set; }

    public bool? HttpOnly { get; set; }

    public double? MaxAge { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public SameSite SameSite { get; set; }

    public bool? Secure { get; set; }

    public string Value { get; set; } = string.Empty;
}