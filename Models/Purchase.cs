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
    
    public partial class Purchase
    {
        public int pu_Id { get; set; }
        public Nullable<int> pu_p_Id { get; set; }
        public Nullable<bool> pu_IsVehicleInspected { get; set; }
        public Nullable<bool> pu_HasFactoryCertificate { get; set; }
        public Nullable<bool> pu_IsQuality { get; set; }
        public Nullable<int> pu_Quantity { get; set; }
        public Nullable<int> pu_Price { get; set; }
        public Nullable<bool> pu_IsSettled { get; set; }
        public Nullable<bool> pu_IsPaymentReceived { get; set; }
        public Nullable<bool> pu_IsInstallment { get; set; }
        public Nullable<System.DateTime> pu_StockInDate { get; set; }
        public Nullable<System.DateTime> pu_StockOutDate { get; set; }
        public Nullable<System.DateTime> pu_CreateDate { get; set; }
    
        public virtual Product Product { get; set; }
    }
}