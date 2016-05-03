using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using IdentityService.Models;

namespace IdentityService.Migrations.ClientConfiguration
{
    [DbContext(typeof(ClientConfigurationContext))]
    partial class ClientConfigurationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.Client<int>", b =>
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

                    b.HasAnnotation("Relational:TableName", "Clients");
                });

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.ClientClaim<int>", b =>
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

                    b.HasAnnotation("Relational:TableName", "ClientClaims");
                });

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.ClientCorsOrigin<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ClientId")
                        .IsRequired();

                    b.Property<string>("Origin")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 150);

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "ClientCorsOrigins");
                });

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.ClientCustomGrantType<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ClientId")
                        .IsRequired();

                    b.Property<string>("GrantType")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 250);

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "ClientCustomGrantTypes");
                });

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.ClientPostLogoutRedirectUri<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ClientId")
                        .IsRequired();

                    b.Property<string>("Uri")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 2000);

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "ClientPostLogoutRedirectUris");
                });

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.ClientProviderRestriction<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ClientId")
                        .IsRequired();

                    b.Property<string>("Provider")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "ClientProviderRestrictions");
                });

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.ClientRedirectUri<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ClientId")
                        .IsRequired();

                    b.Property<string>("Uri")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 2000);

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "ClientRedirectUris");
                });

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.ClientScope<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ClientId")
                        .IsRequired();

                    b.Property<string>("Scope")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "ClientScopes");
                });

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.ClientSecret<int>", b =>
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

                    b.HasAnnotation("Relational:TableName", "ClientSecrets");
                });

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.ClientClaim<int>", b =>
                {
                    b.HasOne("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.Client<int>")
                        .WithMany()
                        .HasForeignKey("ClientId");
                });

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.ClientCorsOrigin<int>", b =>
                {
                    b.HasOne("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.Client<int>")
                        .WithMany()
                        .HasForeignKey("ClientId");
                });

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.ClientCustomGrantType<int>", b =>
                {
                    b.HasOne("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.Client<int>")
                        .WithMany()
                        .HasForeignKey("ClientId");
                });

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.ClientPostLogoutRedirectUri<int>", b =>
                {
                    b.HasOne("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.Client<int>")
                        .WithMany()
                        .HasForeignKey("ClientId");
                });

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.ClientProviderRestriction<int>", b =>
                {
                    b.HasOne("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.Client<int>")
                        .WithMany()
                        .HasForeignKey("ClientId");
                });

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.ClientRedirectUri<int>", b =>
                {
                    b.HasOne("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.Client<int>")
                        .WithMany()
                        .HasForeignKey("ClientId");
                });

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.ClientScope<int>", b =>
                {
                    b.HasOne("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.Client<int>")
                        .WithMany()
                        .HasForeignKey("ClientId");
                });

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.ClientSecret<int>", b =>
                {
                    b.HasOne("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.Client<int>")
                        .WithMany()
                        .HasForeignKey("ClientId");
                });
        }
    }
}
