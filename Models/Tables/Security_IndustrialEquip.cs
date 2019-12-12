using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
  public class Security_IndustrialEquip
  {
    [Key]
    [Column(Order = 1)]
    public string ID { get; set; }
    public string EquipBrand { get; set; }
    public string EquipModel { get; set; }
    public string EquipDesc { get; set; }
    public string SerialNumber { get; set; }
    public Nullable<decimal> SecuredAmount { get; set; }
    public string YearOfManufacture { get; set; }
    public string EngineNumber { get; set; }
    public string ChargeNumber { get; set; }
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