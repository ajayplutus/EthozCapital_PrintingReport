using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
    public class Security_Property
    {
        [Key]
        [Column(Order = 1)]
        public string ID { get; set; }
        public string PropertyAddress { get; set; }
        public string PropertyTypeLevel1 { get; set; }
        public string PropertyTypeLevel2 { get; set; }
        public string FirstThirdParty { get; set; }
        public Nullable<decimal> FormalValuation { get; set; }
        public Nullable<decimal> CreditLimit { get; set; }
        public Nullable<decimal> IndicativeValuation { get; set; }
        public  string TitleNumber { get; set; }
        public string MortgageNumber { get; set; }
        public string ChargeNumber { get; set; }
        public Nullable<DateTime> ChargeDate { get; set; }
        public string SecurityListLevel2 { get; set; }
        [StringLength(1)]
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }        
    }
}