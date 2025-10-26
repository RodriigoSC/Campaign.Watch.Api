using Campaign.Watch.Infra.IoC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Campaign.Watch.Api
{
    /// <summary>
    /// Classe de inicialização da API, responsável por configurar os serviços e o pipeline de requisições HTTP.
    /// </summary>
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        /// <summary>
        /// Inicializa uma nova instância da classe Startup.
        /// </summary>
        /// <param name="configuration">A configuração da aplicação injetada pelo host.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Obtém a configuração da aplicação.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Este método é chamado pelo runtime para adicionar serviços ao contêiner de injeção de dependência.
        /// </summary>
        /// <param name="services">A coleção de serviços para configurar.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Centraliza a injeção de dependência das outras camadas da aplicação.
            Bootstrap.StartIoC(services, Configuration);

            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      // Permite a origem específica do frontend em desenvolvimento
                                      builder.WithOrigins("http://localhost:5173")
                                             .AllowAnyHeader() // Permite qualquer cabeçalho na requisição
                                             .AllowAnyMethod(); // Permite qualquer método HTTP (GET, POST, PUT, DELETE, etc.)
                                      // Para produção, você pode querer ser mais restritivo:
                                      // builder.WithOrigins("https://seu-dominio-de-producao.com")
                                      //        .WithMethods("GET", "POST", "PUT", "DELETE") // Métodos específicos
                                      //        .WithHeaders("Content-Type", "Authorization"); // Cabeçalhos específicos
                                  });
            });

            services.AddControllers();

            // Configura o Swagger/OpenAPI para documentação da API.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Campaign.Watch.Api", Version = "v1" });
            });
        }

        /// <summary>
        /// Este método é chamado pelo runtime para configurar o pipeline de requisições HTTP.
        /// </summary>
        /// <param name="app">O construtor da aplicação para configurar o pipeline de middleware.</param>
        /// <param name="env">O ambiente de hospedagem da aplicação.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Em ambiente de desenvolvimento, habilita a página de exceções detalhadas e a UI do Swagger.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Campaign.Watch.Api v1"));
            }

            // Redireciona requisições HTTP para HTTPS.
            app.UseHttpsRedirection();

            // Habilita o roteamento.
            app.UseRouting();

            //Habilita CORS
            app.UseCors(MyAllowSpecificOrigins);

            // Habilita a autorização.
            app.UseAuthorization();

            // Mapeia os controllers para os endpoints da API.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}