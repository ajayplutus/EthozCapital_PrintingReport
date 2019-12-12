using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
    public class Security_PropertyMortgagor
    {
        [Key]
        [Column(Order = 1)]
        public string MasterID { get; set; }

        [Key]
        [Column(Order =2)]
        public int ItemNumber { get; set; }
        public string MainMortgagor { get; set; }
        public string MortgagorType { get; set; }
        public string Mortgagor { get; set; }
        public string MortgagorAddress { get; set; }
        public string MortgagorDept { get; set; }
        public string MortgagorConPerson { get; set; }
        [StringLength(1)]
    public string Status { get; set;}
        public string CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public string DeletedBy { get; set; }
        public Nullable<DateTime> DeletedDate { get; set; }
    }
}