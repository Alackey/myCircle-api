using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace myCircle_api
{
    public class Startup
    {
        private static readonly HttpClient _client = new HttpClient();
        public static IConfigurationRoot Configuration { get; set; }
        
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            
            // Shows UseCors with CorsPolicyBuilder.
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials() );
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // global policy - assign here or on each controller
            app.UseCors("CorsPolicy");

            // Middleware
            app.Use(async (context, next) =>
            {
                // Check if the ID token for the user is given. 
                StringValues idToken;
                if (context.Request.Headers.TryGetValue("User-ID-Token", out idToken))
                {
                    string result = await verifyIdToken(idToken);
                    if (result.Equals("Success"))
                    {
                        await next();
                    }
                    else
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync(result);
                    }
                }
                else
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("User-ID-Token Header Missing");
                }
            });
            
            app.UseMvc();
        }

        /// <summary>
        /// Verifies that the Firebase User ID Token is valid
        /// </summary>
        /// <param name="idToken"></param>
        /// <returns>String: Error message or Success</returns>
        private async Task<string> verifyIdToken(string idToken)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtInput = idToken;

            //Check if readable token (string is in a JWT format)
            var readableToken = jwtHandler.CanReadToken(jwtInput);

            if (readableToken)
            {
                // Get the firebase kid information
                HttpResponseMessage response = await _client.GetAsync("https://www.googleapis.com/robot/v1/metadata/x509/securetoken@system.gserviceaccount.com");
            
                if (!response.IsSuccessStatusCode) { return "www.googleapis.com Down"; }
                var resposneString = await response.Content.ReadAsStringAsync();
                var x509Data = JsonConvert.DeserializeObject<Dictionary<string, string>>(resposneString);
                
                // JWT Decoding
                var token = jwtHandler.ReadJwtToken(jwtInput);

                //Extract the headers of the JWT
                var headers = token.Header;
                
                if (!headers.Alg.Equals("RS256") || !x509Data.ContainsKey(headers.Kid))
                {
                    return "Encyption Algorithm Invalid or Kid Missing";
                }
                
                
                //Extract the payload of the JWT
                var claims = token.Claims.ToDictionary(c => c.Type, c => c.Value);
                foreach(var c in claims)
                {
                    Console.WriteLine(c);
                }
                
                // Check Expiration Time
                try
                {
                    long expUnix = Convert.ToInt64(claims["exp"]);
                    var expDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expUnix);
                    long expDiff = (long) DateTime.UtcNow.Subtract(expDateTime).TotalSeconds;
                    Console.WriteLine($"Exp: {expUnix}");
                    Console.WriteLine($"UTC: {expDiff}");
                    Console.WriteLine($"Exp diff: {expDiff}");
                    if (expDiff > 0)
                    {
                        Console.WriteLine("Error: exp");
                        return "Expiration Time Invalid";
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Error: exp");
                    return "Expiration Time Invalid Format";
                }
                
                // Check Issued-at time
                try
                {
                    long iatUnix = Convert.ToInt64(claims["iat"]);
                    var iatDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(iatUnix);
                    long iatDiff = (long) DateTime.UtcNow.Subtract(iatDateTime).TotalSeconds;
                    Console.WriteLine($"Iat: {iatUnix}");
                    Console.WriteLine($"UTC: {iatDiff}");
                    Console.WriteLine($"Iat diff: {iatDiff}");

                    if (iatDiff < 0)
                    {
                        Console.WriteLine("Error: iat");
                        return "Issued-At Time Invalid";
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Issued-At Time Invalid Format");
                    return "Issued-At Time Invalid Format";
                }
                
                // Check Audience
                Console.WriteLine($"Audience: {claims["aud"]}");
                if (!claims["aud"].Equals("mycircle-test"))
                {
                    Console.WriteLine("Error: aud");
                    return "Audience Invalid";
                }
                
                // Issuer
                Console.WriteLine($"Issuer: {claims["iss"]}");
                if (!claims["iss"].Equals("https://securetoken.google.com/mycircle-test"))
                {
                    Console.WriteLine("Error: iss");
                    return "Issuer Invalid";
                }
            }
            else
            {
                Console.WriteLine("Could not be read");
                return "Could Not Read User-ID-Token";
            }
            
            return "Success";
        }
    }
}