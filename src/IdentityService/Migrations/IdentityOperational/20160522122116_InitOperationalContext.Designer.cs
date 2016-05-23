using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using IdentityService.Models;

namespace IdentityService.Migrations.IdentityOperational
{
    [DbContext(typeof(IdentityOperationalContext))]
    [Migration("20160522122116_InitOperationalContext")]
    partial class InitOperationalContext
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rc2-20901")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.Consent", b =>
                {
                    b.Property<string>("SubjectId")
                        .HasAnnotation("MaxLength", 200);

                    b.Property<string>("ClientId")
                        .HasAnnotation("MaxLength", 200);

                    b.Property<string>("Scopes")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 2000);

                    b.HasKey("SubjectId", "ClientId");

                    b.ToTable("Consents");
                });

            modelBuilder.Entity("IdentityServer3.EntityFrameworkCore.Entities.Token", b =>
                {
                    b.Property<string>("Key");

                    b.Property<short>("TokenType");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.Property<DateTime>("Expiry");

                    b.Property<string>("JsonCode")
                        .IsRequired();

                    b.Property<string>("SubjectId")
                        .HasAnnotation("MaxLength", 200);

                    b.HasKey("Key", "TokenType");

                    b.ToTable("Tokens");
                });
        }
    }
}
