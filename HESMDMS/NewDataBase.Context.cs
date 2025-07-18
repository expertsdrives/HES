﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HESMDMS
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class SmartMeterEntities1 : DbContext
    {
        public SmartMeterEntities1()
            : base("name=SmartMeterEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tbl_SGMReg> tbl_SGMReg { get; set; }
        public virtual DbSet<tbl_ATGLRates> tbl_ATGLRates { get; set; }
        public virtual DbSet<tbl_GasAMC> tbl_GasAMC { get; set; }
        public virtual DbSet<tbl_GasGCV> tbl_GasGCV { get; set; }
        public virtual DbSet<tbl_GasMinimumchargerate> tbl_GasMinimumchargerate { get; set; }
        public virtual DbSet<tbl_GasqtySlabs_1> tbl_GasqtySlabs_1 { get; set; }
        public virtual DbSet<tbl_GasqtySlabs_2> tbl_GasqtySlabs_2 { get; set; }
        public virtual DbSet<tbl_GasRateSlabs_1> tbl_GasRateSlabs_1 { get; set; }
        public virtual DbSet<tbl_GasRateSlabs_2> tbl_GasRateSlabs_2 { get; set; }
        public virtual DbSet<tbl_GasRental> tbl_GasRental { get; set; }
        public virtual DbSet<tbl_GasSurcharge> tbl_GasSurcharge { get; set; }
        public virtual DbSet<tbl_GasVAT> tbl_GasVAT { get; set; }
        public virtual DbSet<tbl_MeterGroup> tbl_MeterGroup { get; set; }
    
        public virtual ObjectResult<sp_Circle1Temp_Result> sp_Circle1Temp(Nullable<System.DateTime> startdate, Nullable<System.DateTime> enddate)
        {
            var startdateParameter = startdate.HasValue ?
                new ObjectParameter("startdate", startdate) :
                new ObjectParameter("startdate", typeof(System.DateTime));
    
            var enddateParameter = enddate.HasValue ?
                new ObjectParameter("enddate", enddate) :
                new ObjectParameter("enddate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_Circle1Temp_Result>("sp_Circle1Temp", startdateParameter, enddateParameter);
        }
    
        public virtual ObjectResult<sp_ClusterWiseReport_Result> sp_ClusterWiseReport(Nullable<System.DateTime> startdate, Nullable<System.DateTime> enddate)
        {
            var startdateParameter = startdate.HasValue ?
                new ObjectParameter("startdate", startdate) :
                new ObjectParameter("startdate", typeof(System.DateTime));
    
            var enddateParameter = enddate.HasValue ?
                new ObjectParameter("enddate", enddate) :
                new ObjectParameter("enddate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_ClusterWiseReport_Result>("sp_ClusterWiseReport", startdateParameter, enddateParameter);
        }
    
        public virtual ObjectResult<sp_FetchConsumptionData_Result> sp_FetchConsumptionData(Nullable<System.DateTime> start, Nullable<System.DateTime> end)
        {
            var startParameter = start.HasValue ?
                new ObjectParameter("start", start) :
                new ObjectParameter("start", typeof(System.DateTime));
    
            var endParameter = end.HasValue ?
                new ObjectParameter("end", end) :
                new ObjectParameter("end", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_FetchConsumptionData_Result>("sp_FetchConsumptionData", startParameter, endParameter);
        }
    
        public virtual ObjectResult<sp_FetchATGLBalanceTransaction_Result> sp_FetchATGLBalanceTransaction()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_FetchATGLBalanceTransaction_Result>("sp_FetchATGLBalanceTransaction");
        }
    }
}
