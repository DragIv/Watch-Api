using Serilog;

namespace Watch_API.Logging;

public static class SerilogExtensions
{
    public static void AddSerilogLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
        
        // стандартный логер на serilog
        builder.Host.UseSerilog(Log.Logger, dispose: true);
    }
}