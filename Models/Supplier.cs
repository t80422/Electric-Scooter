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
    
    public partial class Supplier
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Supplier()
        {
            this.Product = new HashSet<Product>();
        }
    
        public int s_Id { get; set; }
        public string s_Name { get; set; }
        public string s_InchargePerson { get; set; }
        public string s_Phone1 { get; set; }
        public string s_Phone1Type { get; set; }
        public string s_Phone2 { get; set; }
        public string s_Phone2Type { get; set; }
        public string s_Phone3 { get; set; }
        public string s_Phone3Type { get; set; }
        public string s_Email { get; set; }
        public string s_LineId { get; set; }
        public string s_PostalCode { get; set; }
        public string s_City { get; set; }
        public string s_Address { get; set; }
        public string s_Sales { get; set; }
        public string s_SalesPhone1 { get; set; }
        public string s_SalesPhone1Type { get; set; }
        public string s_SalesPhone2 { get; set; }
        public string s_SalesPhone2Type { get; set; }
        public string s_Memo { get; set; }
        public bool s_State { get; set; }
        public Nullable<System.DateTime> s_CreateDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Product { get; set; }
    }
}