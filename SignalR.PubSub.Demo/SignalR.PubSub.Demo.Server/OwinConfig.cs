namespace SignalR.PubSub.Demo.Server
{
    using Microsoft.AspNet.SignalR;
    using Microsoft.Owin.Cors;

    using Owin;

    /// <summary>
    /// Just OWIN configuration stuff. Nothing to see here. Move along.
    /// </summary>
    internal class OwinConfig
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map(
                "/signalr", 
                map => 
                {
                    map.UseCors(CorsOptions.AllowAll);

                    var hubConfiguration = new HubConfiguration { EnableDetailedErrors = true };
                    map.RunSignalR(hubConfiguration);
                });
        }
    }
}