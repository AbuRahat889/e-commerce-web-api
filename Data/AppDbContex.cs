using System;
using System.Security.Cryptography.X509Certificates;
using e_commerce_web_api.Models;
using Microsoft.EntityFrameworkCore;

namespace e_commerce_web_api.Data;

public class AppDbContex : DbContext
{
    public AppDbContex(DbContextOptions<AppDbContex> options) : base(options) { }
    public DbSet<Category> Categories { get; set; }

}
