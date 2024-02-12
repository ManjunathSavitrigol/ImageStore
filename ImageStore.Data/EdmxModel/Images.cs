//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ImageStore.Data.EdmxModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Images
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Size { get; set; }
        public int CategoryId { get; set; }
        public int ResolutionId { get; set; }
        public string FilePath { get; set; }
        public bool IsVerified { get; set; }
        public Nullable<int> VerifiedBy { get; set; }
        public System.DateTime UploadDate { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public int Likes { get; set; }
        public int Downloads { get; set; }
        public int UploaderId { get; set; }
        public string Tags { get; set; }
        public bool IsRejected { get; set; }
        public Nullable<System.DateTime> RejectedDate { get; set; }
        public Nullable<int> RejectedBy { get; set; }
        public string Reason { get; set; }
    }
}