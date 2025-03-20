using Yarp.ReverseProxy.Configuration;

namespace ReverseProxy.Api.ReverseProxyConfiguration;

internal sealed class Configurator(
    IProxyConfigProvider configurationProvider,
    InMemoryConfigProvider inMemoryConfigProvider)
    : IConfigurator
{
    public void ChangeDestination(string address)
    {
        // Instead of throwing an exception, a good alternative would be to return a corresponding result.
        // This can then be communicated via the endpoint.
        var currentConfiguration = configurationProvider.GetConfig() ?? 
                                   throw new ArgumentNullException("configurationProvider.GetConfig()");

        var newClusters = new List<ClusterConfig>();

        foreach (var cluster in currentConfiguration.Clusters)
        {
            // Here too, it would be better to return a corresponding result
            if (cluster.Destinations == null) continue;
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