using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
  public class Security_Vehicle
  {
    [Key]
    [Column(Order = 1)]
    public string ID { get; set; }
    public string ChassisNumber { get; set; }
    public string RegNumber { get; set; }
    public string VehicleMake { get; set; }
    public string VehicleModel { get; set; }
    public string VehicleType { get; set; }
    public Nullable<DateTime> COE_ExpiryDate { get; set; }
    public string EngineNumber { get; set; }
    public string ChargeNumber { get; set; }
    public Nullable<DateTime> ChargeDate { get; set; }
    public string SecurityListLevel2 { get; set; }
    public Nullable<decimal> Value { get; set; }
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