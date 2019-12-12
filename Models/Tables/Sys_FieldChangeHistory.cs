using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
  public class Sys_FieldChangeHistory
  {
    [Key]
    [Column(Order = 1)]
    public int ID { get; set; }
    public string TableName { get; set; }
    public string FieldName { get; set; }
    public string PKFieldName1 { get; set; }
    public string PKFieldValue1 { get; set; }
    public string PKFieldName2 { get; set; }
    public string PKFieldValue2 { get; set; }
    public string FieldValue { get; set; }
    [StringLength(1)]
    public string Status { get; set; }
    public string UpdatedBy { get; set; }
    public Nullable<DateTime> UpdatedDate { get; set; }
  }
}