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
    using System.Collections.Generic;
    
    public partial class tbl_AdminCredentials
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int RoleID { get; set; }
        public string FullName { get; set; }
        public Nullable<int> AccessFailedCount { get; set; }
        public string MobileNumber { get; set; }
        public Nullable<System.DateTime> LastLoggedInDate { get; set; }
        public Nullable<bool> SendOTP { get; set; }
    
        public virtual tbl_RoleMaster tbl_RoleMaster { get; set; }
    }
}
