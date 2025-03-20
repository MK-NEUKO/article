using Yarp.ReverseProxy.Configuration;

namespace ReverseProxy.Api.ReverseProxyConfiguration;

internal sealed class Configurator(
    IProxyConfigProvider configurationProvider,
    InMemoryConfigProvider inMemoryConfigProvider)
    : IConfigurator
{
    public void ChangeDestination(string address)
    {
        var currentConfiguration = configurationProvider.GetConfig();
        var newClusters = new List<ClusterConfig>();

        foreach (var cluster in currentConfiguration.Clusters)
        {
            var newDestinations = new Dictionary<string, DestinationConfig>(cluster.Destinations.Count);
            foreach (var destination in cluster.Destinations)
            {
                newDestinations.Add(destination.Key, new DestinationConfig
                {
                    Address = address,
                    Health = destination.Value.Health,
                    Metadata = destination.Value.Metadata,
                    Host = destination.Value.Host,
                });
            }
            newClusters.Add(new ClusterConfig
            {
                ClusterId = cluster.ClusterId,
                Destinations = newDestinations,
                HealthCheck = cluster.HealthCheck,
                LoadBalancingPolicy = cluster.LoadBalancingPolicy,
                SessionAffinity = cluster.SessionAffinity,
                HttpRequest = cluster.HttpRequest,
                HttpClient = cluster.HttpClient,
                Metadata = cluster.Metadata,
            });
        }

        inMemoryConfigProvider.Update(currentConfiguration.Routes, newClusters);
    }
}