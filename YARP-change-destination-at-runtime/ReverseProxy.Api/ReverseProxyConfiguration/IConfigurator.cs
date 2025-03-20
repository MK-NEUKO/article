namespace ReverseProxy.Api.ReverseProxyConfiguration;

internal interface IConfigurator
{
    void ChangeDestination(string address);
}