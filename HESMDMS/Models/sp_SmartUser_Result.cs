//------------------------------------------------------------------------------
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
    
    public partial class sp_SmartUser_Result
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int RoleID { get; set; }
        public string FullName { get; set; }
        public Nullable<int> AccessFailedCount { get; set; }
        public string MeterID { get; set; }
        public int UserID { get; set; }
    }
}
