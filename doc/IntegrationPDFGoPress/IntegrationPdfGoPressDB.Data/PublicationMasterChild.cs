//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IntegrationPdfGoPressDB.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class PublicationMasterChild
    {
        public long PublicationMasterChildID { get; set; }
        public Nullable<long> PublicationMasterID { get; set; }
        public string PublicationChildName { get; set; }
        public string RepositorySource { get; set; }
        public Nullable<long> MediaID { get; set; }
        public string MediaName { get; set; }
    
        public virtual PublicationMaster PublicationMaster { get; set; }
    }
}
