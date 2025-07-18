﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HESMDMS.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class SmartMeter_ProdEntities : DbContext
    {
        public SmartMeter_ProdEntities()
            : base("name=SmartMeter_ProdEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__EFMigrationsHistory> C__EFMigrationsHistory { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<tbl_BackLogAPILogs> tbl_BackLogAPILogs { get; set; }
        public virtual DbSet<tbl_CommandBackLog> tbl_CommandBackLog { get; set; }
        public virtual DbSet<tbl_FirmwareHistoty> tbl_FirmwareHistoty { get; set; }
        public virtual DbSet<tbl_FirmwareStatus> tbl_FirmwareStatus { get; set; }
        public virtual DbSet<tbl_JioNotification> tbl_JioNotification { get; set; }
        public virtual DbSet<tbl_RegisteredComplain> tbl_RegisteredComplain { get; set; }
        public virtual DbSet<tbl_Response> tbl_Response { get; set; }
        public virtual DbSet<tbl_SGMAuditLogs> tbl_SGMAuditLogs { get; set; }
        public virtual DbSet<tbl_SimCardMaster> tbl_SimCardMaster { get; set; }
        public virtual DbSet<tbl_SmartMeterUser> tbl_SmartMeterUser { get; set; }
        public virtual DbSet<tbl_TicketTypeMaster> tbl_TicketTypeMaster { get; set; }
        public virtual DbSet<tbl_TransLogs> tbl_TransLogs { get; set; }
        public virtual DbSet<tbl_Users> tbl_Users { get; set; }
        public virtual DbSet<tbl_DataReception> tbl_DataReception { get; set; }
        public virtual DbSet<tbl_SMeterMaster> tbl_SMeterMaster { get; set; }
        public virtual DbSet<tbl_AssignSmartMeter> tbl_AssignSmartMeter { get; set; }
        public virtual DbSet<tbl_JioLogs> tbl_JioLogs { get; set; }
    
        [DbFunction("SmartMeter_ProdEntities", "fnSplit")]
        public virtual IQueryable<fnSplit_Result> fnSplit(string sInputList, string sDelimiter)
        {
            var sInputListParameter = sInputList != null ?
                new ObjectParameter("sInputList", sInputList) :
                new ObjectParameter("sInputList", typeof(string));
    
            var sDelimiterParameter = sDelimiter != null ?
                new ObjectParameter("sDelimiter", sDelimiter) :
                new ObjectParameter("sDelimiter", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fnSplit_Result>("[SmartMeter_ProdEntities].[fnSplit](@sInputList, @sDelimiter)", sInputListParameter, sDelimiterParameter);
        }
    
        [DbFunction("SmartMeter_ProdEntities", "fnSplitString")]
        public virtual IQueryable<fnSplitString_Result> fnSplitString(string @string, string delimiter)
        {
            var stringParameter = @string != null ?
                new ObjectParameter("string", @string) :
                new ObjectParameter("string", typeof(string));
    
            var delimiterParameter = delimiter != null ?
                new ObjectParameter("delimiter", delimiter) :
                new ObjectParameter("delimiter", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fnSplitString_Result>("[SmartMeter_ProdEntities].[fnSplitString](@string, @delimiter)", stringParameter, delimiterParameter);
        }
    
        public virtual int sp_alterdiagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_alterdiagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_creatediagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_creatediagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_dropdiagram(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_dropdiagram", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_Get15DayReport()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_Get15DayReport");
        }
    
        public virtual ObjectResult<sp_helpdiagramdefinition_Result> sp_helpdiagramdefinition(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagramdefinition_Result>("sp_helpdiagramdefinition", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagrams_Result> sp_helpdiagrams(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagrams_Result>("sp_helpdiagrams", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_MeterResponse_Result> sp_MeterResponse()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_MeterResponse_Result>("sp_MeterResponse");
        }
    
        public virtual ObjectResult<sp_OTAFetch_Result> sp_OTAFetch(string pld, Nullable<System.DateTime> lDate, Nullable<System.DateTime> mDate)
        {
            var pldParameter = pld != null ?
                new ObjectParameter("pld", pld) :
                new ObjectParameter("pld", typeof(string));
    
            var lDateParameter = lDate.HasValue ?
                new ObjectParameter("LDate", lDate) :
                new ObjectParameter("LDate", typeof(System.DateTime));
    
            var mDateParameter = mDate.HasValue ?
                new ObjectParameter("MDate", mDate) :
                new ObjectParameter("MDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_OTAFetch_Result>("sp_OTAFetch", pldParameter, lDateParameter, mDateParameter);
        }
    
        public virtual int sp_renamediagram(string diagramname, Nullable<int> owner_id, string new_diagramname)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var new_diagramnameParameter = new_diagramname != null ?
                new ObjectParameter("new_diagramname", new_diagramname) :
                new ObjectParameter("new_diagramname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_renamediagram", diagramnameParameter, owner_idParameter, new_diagramnameParameter);
        }
    
        public virtual ObjectResult<sp_ResponseSplited_Result> sp_ResponseSplited(string pld, Nullable<System.DateTime> lDate, Nullable<System.DateTime> mDate)
        {
            var pldParameter = pld != null ?
                new ObjectParameter("pld", pld) :
                new ObjectParameter("pld", typeof(string));
    
            var lDateParameter = lDate.HasValue ?
                new ObjectParameter("LDate", lDate) :
                new ObjectParameter("LDate", typeof(System.DateTime));
    
            var mDateParameter = mDate.HasValue ?
                new ObjectParameter("MDate", mDate) :
                new ObjectParameter("MDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_ResponseSplited_Result>("sp_ResponseSplited", pldParameter, lDateParameter, mDateParameter);
        }
    
        public virtual ObjectResult<sp_SmartUser_Result> sp_SmartUser()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SmartUser_Result>("sp_SmartUser");
        }
    
        public virtual int sp_upgraddiagrams()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_upgraddiagrams");
        }
    
        public virtual ObjectResult<sp_SmartMeterData_Result> sp_SmartMeterData()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SmartMeterData_Result>("sp_SmartMeterData");
        }
    
        public virtual ObjectResult<BillingCustomer_Result> BillingCustomer()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<BillingCustomer_Result>("BillingCustomer");
        }
    
        public virtual ObjectResult<sp_ResponseSplited_Billing_Result> sp_ResponseSplited_Billing(string pld, Nullable<System.DateTime> lDate, Nullable<System.DateTime> mDate)
        {
            var pldParameter = pld != null ?
                new ObjectParameter("pld", pld) :
                new ObjectParameter("pld", typeof(string));
    
            var lDateParameter = lDate.HasValue ?
                new ObjectParameter("LDate", lDate) :
                new ObjectParameter("LDate", typeof(System.DateTime));
    
            var mDateParameter = mDate.HasValue ?
                new ObjectParameter("MDate", mDate) :
                new ObjectParameter("MDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_ResponseSplited_Billing_Result>("sp_ResponseSplited_Billing", pldParameter, lDateParameter, mDateParameter);
        }
    }
}
