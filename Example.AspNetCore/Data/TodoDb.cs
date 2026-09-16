using Example.AspNetCore.Models;
using Microsoft.EntityFrameworkCore;

namespace Example.AspNetCore.Data;

public class TodoDb(DbContextOptions<TodoDb> options) : DbContext(options)
{
    public DbSet<Todo> Todos => Set<Todo>();
}
