using CardIndexDal.Models;
using Microsoft.EntityFrameworkCore;

namespace CardIndexDal
{
    public class CardIndexDbContext : DbContext
    {
        public CardIndexDbContext(DbContextOptions options) : base(options)
        {
        }

        public CardIndexDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PatientCard>()
                .HasMany(i => i.Visits)
                .WithOne(i => i.PatientCard)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<VisitCard>()
                .HasOne(i => i.PatientCard)
                .WithMany(c => c.Visits)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }

        public DbSet<PatientCard> PatientCards { get; set; }
        public DbSet<VisitCard> VisitCards { get; set; }
    }
}


