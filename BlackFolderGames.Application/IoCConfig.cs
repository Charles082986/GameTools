using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Text;
using WorldBuilder.Data;

namespace BlackFolderGames.Application
{
    public class IoCConfig
    {
        public static void Configure(IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton(
                NeoServerConfiguration.GetConfiguration(
                    new Uri(config.GetConnectionString("WorldBuilder")), 
                    config["BlackFolderGames:Authentication:Neo4j:Username"], 
                    config["BlackFolderGames:Authentication:Neo4j:Password"]));
            services.AddSingleton<IGraphClientFactory, GraphClientFactory>();
            services.AddScoped<IWorldBuilderRepository, WorldBuilderRepository>();
            
        }
    }
}
