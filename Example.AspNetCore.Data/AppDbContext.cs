using Example.AspNetCore.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Example.AspNetCore.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos => Set<Todo>();
}
