using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using IdentityService.Models;

namespace IdentityService.Migrations.IdentityScopeConfiguration
{
    [DbContext(typeof(IdentityScopeConfigurationContext))]
    [Migration("20160522121551_InitScopeConfigurationContext")]
    partial class InitScopeConfigurationContext
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rc2-20901")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.Scope", b =>
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

                    b.ToTable("Scopes");
                });

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.ScopeClaim", b =>
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

                    b.HasIndex("ScopeId");

                    b.ToTable("ScopeClaims");
                });

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.ScopeSecret", b =>
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

                    b.HasIndex("ScopeId");

                    b.ToTable("ScopeSecrets");
                });

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.ScopeClaim", b =>
                {
                    b.HasOne("IdentityServer3.EntityFrameworkCore.Entities.Scope")
                        .WithMany()
                        .HasForeignKey("ScopeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.ScopeSecret", b =>
                {
                    b.HasOne("IdentityServer3.EntityFrameworkCore.Entities.Scope")
                        .WithMany()
                        .HasForeignKey("ScopeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
