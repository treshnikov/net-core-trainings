using System.Collections.Generic;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class JoggingDbContext : DbContext
    {
        public DbSet<JoggingData> JoggingData { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<ServerSettings> ServerSettings { get; set; }
        
        public JoggingDbContext(DbContextOptions options) : base(options)
        {
            
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleToRolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });  

            modelBuilder.Entity<RoleToRolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.Permissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RoleToRolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(rp => rp.PermissionId);
            
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            var manageUsersPermission = new RolePermission
            {
                Id = 1,
                Name = "ManageUsers"
            };

            var manageOwnJoggingDataPermission = new RolePermission
            {
                Id = 2,
                Name = "ManageOwnJoggingData"
            };

            var manageOthersJoggingDataPermission = new RolePermission
            {
                Id = 3,
                Name = "ManageOthersJoggingData"
            };

            var user = new UserRole
            {
                Id = 1,
                Name = "User",
                IsSuperUser = false
            };

            var manager = new UserRole
            {
                Id = 2,
                Name = "Manager",
                IsSuperUser = false
            };

            var admin = new UserRole
            {
                Id = 3,
                Name = "Administrator",
                IsSuperUser = true
            };

            modelBuilder.Entity<UserRole>().HasData(user, manager, admin);
            modelBuilder.Entity<RolePermission>().HasData(
                manageUsersPermission,
                manageOwnJoggingDataPermission,
                manageOthersJoggingDataPermission);

            AddPermission(modelBuilder, user, manageOwnJoggingDataPermission);
            AddPermission(
                modelBuilder, 
                manager, 
                manageOwnJoggingDataPermission, 
                manageUsersPermission);
            AddPermission(
                modelBuilder, 
                admin,
                manageOwnJoggingDataPermission,
                manageOthersJoggingDataPermission,
                manageUsersPermission);

            modelBuilder.Entity<ServerSettings>().HasData(new ServerSettings
            {
                Id = 1,
                Name = "DefaultUserRole",
                Value = user.Id.ToString()
            });
        }

        private static void AddPermission(ModelBuilder modelBuilder, UserRole role, params RolePermission[] permissions)
        {
            foreach (var permission in permissions)
            {
                modelBuilder.Entity<RoleToRolePermission>().HasData(new RoleToRolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permission.Id
                });
            }
        }
    }
}