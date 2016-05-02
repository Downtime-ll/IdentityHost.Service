using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using TwentyTwenty.IdentityServer3.EntityFramework7.DbContexts;

namespace IdentityService.Migrations
{
    [DbContext(typeof(OperationalContext))]
    [Migration("20160502112815_AddOperationalModels")]
    partial class AddOperationalModels
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.Consent", b =>
                {
                    b.Property<string>("SubjectId")
                        .HasAnnotation("MaxLength", 200);

                    b.Property<string>("ClientId")
                        .HasAnnotation("MaxLength", 200);

                    b.Property<string>("Scopes")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 2000);

                    b.HasKey("SubjectId", "ClientId");

                    b.HasAnnotation("Relational:TableName", "Consents");
                });

            modelBuilder.Entity("TwentyTwenty.IdentityServer3.EntityFramework7.Entities.Token", b =>
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

                    b.HasAnnotation("Relational:TableName", "Tokens");
                });
        }
    }
}
