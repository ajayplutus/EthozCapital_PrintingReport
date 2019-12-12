﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
  public class Security_Vessel
  {
    [Key]
    [Column(Order = 1)]
    public string ID { get; set; }
    public string HullNumber { get; set; }
    public string VesselName { get; set; }
    public string CountryOfReg { get; set; }
    public string MortgageNumber { get; set; }
    public Nullable<decimal> FormalValuation { get; set; }
    public string ChargeNumber { get; set; }
    public Nullable<DateTime> ChargeDate { get; set; }
    public Nullable<decimal> CreditLimit { get; set; }
    public Nullable<decimal> IndicativeValuation { get; set; }
    public string SecurityListLevel2 { get; set; }
    [StringLength(1)]
    public string Status { get; set; }
    public string CreatedBy { get; set; }
    public Nullable<DateTime> CreatedDate { get; set; }
    public string UpdatedBy { get; set; }
    public Nullable<DateTime> UpdatedDate { get; set; }
  }
}