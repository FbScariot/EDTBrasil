using System;
using System.Data.Entity;

namespace WinoverScheduler.Models
{
    public partial class winoverEntities : DbContext
    {
        public winoverEntities()
            : base("name=winoverEntities")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }


        public virtual DbSet<Funcionario> Funcionarios { get; set; }
        public virtual DbSet<Pausa> Pausas { get; set; }
        public virtual DbSet<PausasLog> PausasLogs { get; set; }
        //public virtual DbSet<RelatoriosAfd> RelatoriosAfd { get; set; }
        //public virtual DbSet<pausa> pausas { get; set; }
        //public virtual DbSet<pausas_log> pausas_log { get; set; }
        //public virtual DbSet<relatorios_afd> relatorios_afd { get; set; }
        //public virtual DbSet<funcionario> funcionarios { get; set; }
    }

    //public partial class WinoverContext : DbContext
    //{
    //    public WinoverContext()
    //    {
    //    }

    //    public WinoverContext(DbContextOptions<WinoverContext> options)
    //        : base(options)
    //    {
    //    }

    //    public virtual DbSet<Funcionario> Funcionarios { get; set; }
    //    public virtual DbSet<Pausa> Pausas { get; set; }
    //    public virtual DbSet<PausasLog> PausasLogs { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    {
    //        if (!optionsBuilder.IsConfigured)
    //        {
    //        }
    //    }

    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        modelBuilder.Entity<Funcionario>(entity =>
    //        {
    //            entity.Property(e => e.HoraEntrada).HasDefaultValueSql("'00:00:00'");

    //            entity.Property(e => e.HoraSaida).HasDefaultValueSql("'00:00:00'");

    //            entity.Property(e => e.P1Fim).HasDefaultValueSql("'00:00:00'");

    //            entity.Property(e => e.P1Inicio).HasDefaultValueSql("'00:00:00'");

    //            entity.Property(e => e.P2Fim).HasDefaultValueSql("'00:00:00'");

    //            entity.Property(e => e.P2Inicio).HasDefaultValueSql("'00:00:00'");

    //            entity.Property(e => e.P3Fim).HasDefaultValueSql("'00:00:00'");

    //            entity.Property(e => e.P3Inicio).HasDefaultValueSql("'00:00:00'");

    //            entity.Property(e => e.TipoEscala).HasDefaultValueSql("'1'");
    //        });

    //        modelBuilder.Entity<Pausa>(entity =>
    //        {
    //            entity.Property(e => e.FixoVariavel).HasDefaultValueSql("'0'");

    //            entity.Property(e => e.Tempo).HasDefaultValueSql("'0'");

    //            entity.Property(e => e.Trava).HasDefaultValueSql("'0'");

    //            entity.Property(e => e.TravaSistema).HasDefaultValueSql("'0'");
    //        });

    //        OnModelCreatingPartial(modelBuilder);
    //    }

    //    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    //}
}