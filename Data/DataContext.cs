global using Microsoft.EntityFrameworkCore;
using minimalAPI_webbutveckling_labb2.Models;

namespace minimalAPI_webbutveckling_labb2.Data
{
    public class DataContext : DbContext
    {

 
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Car> Cars => Set<Car>();

       
        
    }        
    
}