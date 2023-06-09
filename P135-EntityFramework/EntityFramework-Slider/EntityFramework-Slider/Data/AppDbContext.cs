using EntityFramework_Slider.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Data
{
    public class AppDbContext:IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<SliderInfo> SliderInfos { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<BlogHeader> BlogHeaders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasQueryFilter(m=>!m.SoftDelete);
            modelBuilder.Entity<Blog>().HasQueryFilter(m => !m.SoftDelete);
            modelBuilder.Entity<BlogHeader>().HasQueryFilter(m => !m.SoftDelete);
            modelBuilder.Entity<Category>().HasQueryFilter(m => !m.SoftDelete);

            modelBuilder.Entity<Setting>()
             .HasData(
                  new Setting
                  {
                      Id = 1,
                      Key = "HeaderLogo",
                      Value = "logo.png"
                  },
                  new Setting
                  {
                     Id = 2,
                     Key = "Phone",
                     Value = "34564356545"
                  },
                  new Setting
                  {
                     Id = 3,
                     Key = "Email",
                     Value = "p135@code.edu.az"
                  }
             );

            modelBuilder.Entity<BlogHeader>()
                  .HasData(
                       new BlogHeader
                       {
                           Id = 1,
                           Title = "Hello P135",
                           Description = "How are you ?",

                       },
                       new BlogHeader
                       {
                           Id = 2,
                           Title = "Hello P414",
                           Description = "How are you ?"
                       }
                  );
        }
    }




}

