//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Electric_Scooter.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer()
        {
            this.Orders = new HashSet<Orders>();
            this.Points = new HashSet<Points>();
            this.Users = new HashSet<Users>();
        }
    
        public int c_Id { get; set; }
        public string c_Name { get; set; }
        public Nullable<bool> c_Gender { get; set; }
        public Nullable<System.DateTime> c_Birthday { get; set; }
        public string c_IdCard { get; set; }
        public string c_EMail { get; set; }
        public string c_PostalCode { get; set; }
        public string c_City { get; set; }
        public string c_Address { get; set; }
        public Nullable<System.DateTime> c_CreateDate { get; set; }
        public string c_Memo { get; set; }
        public string c_TaxId { get; set; }
        public string c_Company { get; set; }
        public string c_Phone1 { get; set; }
        public string c_Phone2 { get; set; }
        public string c_Phone2_Type { get; set; }
        public string c_No { get; set; }
        public string c_Area { get; set; }
        public string c_CarFrameNo { get; set; }
        public string c_LicensePlate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Orders> Orders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Points> Points { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Users> Users { get; set; }
    }
}
