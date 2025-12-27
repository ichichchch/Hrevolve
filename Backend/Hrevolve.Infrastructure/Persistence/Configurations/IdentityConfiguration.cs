using Hrevolve.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrevolve.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Username)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(u => u.Email)
            .HasMaxLength(256)
            .IsRequired();
        
        builder.Property(u => u.Phone)
            .HasMaxLength(20);
        
        builder.Property(u => u.PasswordHash)
            .HasMaxLength(500);
        
        builder.Property(u => u.TotpSecret)
            .HasMaxLength(500);
        
        builder.Property(u => u.RecoveryCodes)
            .HasMaxLength(2000);
        
        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(u => u.LastLoginIp)
            .HasMaxLength(50);
        
        // 索引
        builder.HasIndex(u => new { u.TenantId, u.Username })
            .IsUnique();
        
        builder.HasIndex(u => new { u.TenantId, u.Email })
            .IsUnique();
        
        builder.HasIndex(u => new { u.TenantId, u.EmployeeId });
        
        // 外部登录
        builder.HasMany(u => u.ExternalLogins)
            .WithOne()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // 用户角色
        builder.HasMany(u => u.UserRoles)
            .WithOne()
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // 受信任设备
        builder.HasMany(u => u.TrustedDevices)
            .WithOne()
            .HasForeignKey(td => td.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Ignore(u => u.DomainEvents);
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(r => r.Code)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(r => r.Description)
            .HasMaxLength(500);
        
        // 索引
        builder.HasIndex(r => new { r.TenantId, r.Code })
            .IsUnique();
        
        // 角色权限
        builder.HasMany(r => r.Permissions)
            .WithOne()
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Ignore(r => r.DomainEvents);
    }
}

public class ExternalLoginConfiguration : IEntityTypeConfiguration<ExternalLogin>
{
    public void Configure(EntityTypeBuilder<ExternalLogin> builder)
    {
        builder.ToTable("ExternalLogins");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Provider)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(e => e.ProviderKey)
            .HasMaxLength(256)
            .IsRequired();
        
        builder.HasIndex(e => new { e.Provider, e.ProviderKey })
            .IsUnique();
    }
}

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");
        
        builder.HasKey(ur => ur.Id);
        
        builder.HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique();
    }
}

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");
        
        builder.HasKey(rp => rp.Id);
        
        builder.Property(rp => rp.PermissionCode)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.HasIndex(rp => new { rp.RoleId, rp.PermissionCode })
            .IsUnique();
    }
}

public class TrustedDeviceConfiguration : IEntityTypeConfiguration<TrustedDevice>
{
    public void Configure(EntityTypeBuilder<TrustedDevice> builder)
    {
        builder.ToTable("TrustedDevices");
        
        builder.HasKey(td => td.Id);
        
        builder.Property(td => td.DeviceId)
            .HasMaxLength(256)
            .IsRequired();
        
        builder.Property(td => td.DeviceName)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(td => td.UserAgent)
            .HasMaxLength(500);
        
        builder.HasIndex(td => new { td.UserId, td.DeviceId })
            .IsUnique();
    }
}
