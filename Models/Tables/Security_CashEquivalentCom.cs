using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
  public class Security_CashEquivalentCom
  {
    [Key]
    [Column(Order = 1)]
    public string ID { get; set; }
    public string Refundable { get; set; }
    public string GuaranteeBondsType { get; set; }
    public decimal Amount { get; set; }
    public string BillToCustomer { get; set; }
    public string BillToAddress { get; set; }
    public string BillToDept { get; set; }
    public string BillToConPerson { get; set; }
    public string InvoiceNumber { get; set; }
    public Nullable<DateTime> InvoiceDate { get; set; }
    public string SecurityListLevel2 { get; set; }
    [StringLength(1)]
    public string Status { get; set; }
    public string CreatedBy { get; set; }
    public Nullable<DateTime> CreatedDate { get; set; }
    public string UpdatedBy { get; set; }
    public Nullable<DateTime> UpdatedDate { get; set; }
    public Nullable<DateTime> DischargeDate { get; set; }
    public string DischargeBy { get; set; }
  }
}