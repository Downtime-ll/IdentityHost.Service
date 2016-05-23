using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using IdentityService.Models;

namespace IdentityService.Migrations.IdentityClientConfiguration
{
    [DbContext(typeof(IdentityClientConfigurationContext))]
    [Migration("20160522121645_InitIdentityClientConfigurationContext")]
    partial class InitIdentityClientConfigurationContext
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rc2-20901")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AbsoluteRefreshTokenLifetime");

                    b.Property<int>("AccessTokenLifetime");

                    b.Property<int>("AccessTokenType");

                    b.Property<bool>("AllowAccessToAllGrantTypes");

                    b.Property<bool>("AllowAccessToAllScopes");

                    b.Property<bool>("AllowClientCredentialsOnly");

                    b.Property<bool>("AllowRememberConsent");

                    b.Property<bool>("AlwaysSendClientClaims");

                    b.Property<int>("AuthorizationCodeLifetime");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.Property<string>("ClientName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.Property<string>("ClientUri")
                        .HasAnnotation("MaxLength", 2000);

                    b.Property<bool>("EnableLocalLogin");

                    b.Property<bool>("Enabled");

                    b.Property<int>("Flow");

                    b.Property<int>("IdentityTokenLifetime");

                    b.Property<bool>("IncludeJwtId");

                    b.Property<string>("LogoUri");

                    b.Property<bool>("LogoutSessionRequired");

                    b.Property<string>("LogoutUri");

                    b.Property<bool>("PrefixClientClaims");

                    b.Property<int>("RefreshTokenExpiration");

                    b.Property<int>("RefreshTokenUsage");

                    b.Property<bool>("RequireConsent");

                    b.Property<int>("SlidingRefreshTokenLifetime");

                    b.Property<bool>("UpdateAccessTokenOnRefresh");

                    b.HasKey("Id");

                    b.HasIndex("ClientId")
                        .IsUnique();

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.ClientClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ClientId");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 250);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 250);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientClaims");
                });

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.ClientCorsOrigin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ClientId")
                        .IsRequired();

                    b.Property<string>("Origin")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 150);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientCorsOrigins");
                });

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.ClientCustomGrantType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ClientId")
                        .IsRequired();

                    b.Property<string>("GrantType")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 250);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientCustomGrantTypes");
                });

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.ClientIdPRestriction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ClientId")
                        .IsRequired();

                    b.Property<string>("Provider")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientProviderRestrictions");
                });

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.ClientPostLogoutRedirectUri", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ClientId")
                        .IsRequired();

                    b.Property<string>("Uri")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 2000);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientPostLogoutRedirectUris");
                });

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.ClientRedirectUri", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ClientId")
                        .IsRequired();

                    b.Property<string>("Uri")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 2000);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientRedirectUris");
                });

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.ClientScope", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ClientId")
                        .IsRequired();

                    b.Property<string>("Scope")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientScopes");
                });

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.ClientSecret", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ClientId")
                        .IsRequired();

                    b.Property<string>("Description")
                        .HasAnnotation("MaxLength", 2000);

                    b.Property<DateTime?>("Expiration");

                    b.Property<string>("Type")
                        .HasAnnotation("MaxLength", 250);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 250);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientSecrets");
                });

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.ClientClaim", b =>
                {
                    b.HasOne("IdentityServer3.EntityFrameworkCore.Entities.Client")
                        .WithMany()
                        .HasForeignKey("ClientId");
                });

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.ClientCorsOrigin", b =>
                {
                    b.HasOne("IdentityServer3.EntityFrameworkCore.Entities.Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.ClientCustomGrantType", b =>
                {
                    b.HasOne("IdentityServer3.EntityFrameworkCore.Entities.Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.ClientIdPRestriction", b =>
                {
                    b.HasOne("IdentityServer3.EntityFrameworkCore.Entities.Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.ClientPostLogoutRedirectUri", b =>
                {
                    b.HasOne("IdentityServer3.EntityFrameworkCore.Entities.Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.ClientRedirectUri", b =>
                {
                    b.HasOne("IdentityServer3.EntityFrameworkCore.Entities.Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.ClientScope", b =>
                {
                    b.HasOne("IdentityServer3.EntityFrameworkCore.Entities.Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.ClientSecret", b =>
                {
                    b.HasOne("IdentityServer3.EntityFrameworkCore.Entities.Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
