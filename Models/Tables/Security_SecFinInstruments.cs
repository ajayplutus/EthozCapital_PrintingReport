using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
  public class Security_SecFinInstruments
  {
    [Key]
    [Column(Order = 1)]
    public string ID { get; set; }
    public string Type { get; set; }
    public decimal Amount { get; set; }
    public string DocumentNumber { get; set; }
    public string BankFinancialCom { get; set; }
    public Nullable<DateTime> ChargeDate { get; set; }
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