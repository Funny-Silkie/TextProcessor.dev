using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Radzen;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace TextProcessor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();
            foreach (Type type in Assembly.GetExecutingAssembly().DefinedTypes)
            {
                string name = type.Name;
                InjectionRangeAttribute? injectionRange = type.GetCustomAttribute<InjectionRangeAttribute>();
                if (injectionRange is null) continue;
                switch (injectionRange.InjectionType)
                {
                    case InjectionType.Singleton:
                        builder.Services.AddSingleton(type);
                        break;

                    case InjectionType.Scoped:
                        builder.Services.AddScoped(type);
                        break;

                    case InjectionType.Transient:
                        builder.Services.AddTransient(type);
                        break;

                    default: throw new InvalidOperationException();
                }
            }

            await builder.Build().RunAsync();
        }
    }
}
