using Microsoft.EntityFrameworkCore;

namespace Quyen15.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<DoctorReview> DoctorReviews { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<TestResult> TestResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Payment>()
    .Property(p => p.Amount)
    .HasPrecision(18, 2);

            // Patient - UserAccount
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.UserAccount)
                .WithMany()
                .HasForeignKey(p => p.IdUser)
                .OnDelete(DeleteBehavior.SetNull);

            // Doctor - UserAccount
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.UserAccount)
                .WithMany()
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.SetNull);

            // Patient - Appointment
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.IdPatient)
                .OnDelete(DeleteBehavior.Restrict);

            // Doctor - Appointment
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.IdDoctor)
                .OnDelete(DeleteBehavior.Restrict);

            // Patient - MedicalRecord
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Patient)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(m => m.IdPatient)
                .OnDelete(DeleteBehavior.Restrict);

            // Doctor - MedicalRecord
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Doctor)
                .WithMany(d => d.MedicalRecords)
                .HasForeignKey(m => m.IdDoctor)
                .OnDelete(DeleteBehavior.Restrict);

            // Appointment - MedicalRecord
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Appointment)
                .WithOne(a => a.MedicalRecord)
                .HasForeignKey<MedicalRecord>(m => m.IdAppointment)
                .OnDelete(DeleteBehavior.Restrict);

            // Doctor - Review
            modelBuilder.Entity<DoctorReview>()
                .HasOne(r => r.Doctor)
                .WithMany(d => d.DoctorReviews)
                .HasForeignKey(r => r.IdDoctor)
                .OnDelete(DeleteBehavior.Restrict);

            // Patient - Review
            modelBuilder.Entity<DoctorReview>()
                .HasOne(r => r.Patient)
                .WithMany(p => p.DoctorReviews)
                .HasForeignKey(r => r.IdPatient)
                .OnDelete(DeleteBehavior.Restrict);

            // Appointment - Payment
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Appointment)
                .WithOne(a => a.Payment)
                .HasForeignKey<Payment>(p => p.IdAppointment)
                .OnDelete(DeleteBehavior.Restrict);

            // Patient - TestResult
            modelBuilder.Entity<TestResult>()
                .HasOne(t => t.Patient)
                .WithMany(p => p.TestResults)
                .HasForeignKey(t => t.IdPatient)
                .OnDelete(DeleteBehavior.Restrict);

            // Doctor - TestResult
            modelBuilder.Entity<TestResult>()
                .HasOne(t => t.Doctor)
                .WithMany()
                .HasForeignKey(t => t.IdDoctor)
                .OnDelete(DeleteBehavior.Restrict);

            // Appointment - TestResult
            modelBuilder.Entity<TestResult>()
                .HasOne(t => t.Appointment)
                .WithMany(a => a.TestResults)
                .HasForeignKey(t => t.IdAppointment)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}