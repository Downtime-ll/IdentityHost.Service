using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using IdentityService.Models;

namespace IdentityService.Migrations.ScopeConfiguration
{
    [DbContext(typeof(ScopeConfigurationContext))]
    [Migration("20160503082448_AddScopeConfiguration")]
    partial class AddScopeConfiguration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.Scope<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimsRule")
                        .HasAnnotation("MaxLength", 200);

                    b.Property<string>("Description")
                        .HasAnnotation("MaxLength", 1000);

                    b.Property<string>("DisplayName")
                        .HasAnnotation("MaxLength", 200);

                    b.Property<bool>("Emphasize");

                    b.Property<bool>("Enabled");

                    b.Property<bool>("IncludeAllClaimsForUser");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.Property<bool>("Required");

                    b.Property<bool>("ShowInDiscoveryDocument");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "Scopes");
                });

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.ScopeClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AlwaysIncludeInIdToken");

                    b.Property<string>("Description")
                        .HasAnnotation("MaxLength", 1000);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.Property<int?>("ScopeId");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "ScopeClaims");
                });

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.ScopeSecret<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .HasAnnotation("MaxLength", 1000);

                    b.Property<DateTime?>("Expiration");

                    b.Property<int?>("ScopeId");

                    b.Property<string>("Type")
                        .HasAnnotation("MaxLength", 250);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 250);

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "ScopeSecrets");
                });

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.ScopeClaim<int>", b =>
                {
                    b.HasOne("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.Scope<int>")
                        .WithMany()
                        .HasForeignKey("ScopeId");
                });

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.ScopeSecret<int>", b =>
                {
                    b.HasOne("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.Scope<int>")
                        .WithMany()
                        .HasForeignKey("ScopeId");
                });
        }
    }
}
