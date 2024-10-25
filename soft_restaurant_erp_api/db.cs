using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

public class IdRecord
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Value { get; set; }
}

public class ERPInternalContext(string connectionString) : DbContext
{

    private string _connectionString = connectionString;
    public DbSet<IdRecord> OrderNumbers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(this._connectionString);
    }
}