using Npgsql;

namespace Example.AspNetCore;

public static class Configurations
{
    public static string BuildDbConnectionString(IConfiguration configuration)
    {
        var builder = new NpgsqlConnectionStringBuilder()
        {
            Host = configuration["Database:Host"],
            Port = configuration.GetValue<int>("Database:Port"),
            Database = configuration["Database:Name"],
            Username = configuration["Database:Username"],
            Password = configuration["Database:Password"],
        };

        return builder.ConnectionString;
    }
}
