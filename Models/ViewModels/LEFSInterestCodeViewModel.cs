using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.ViewModels
{
    public class LEFSInterestCodeViewModel
    {
        private static ILog glog = log4net.LogManager.GetLogger(typeof(LEFSInterestCodeViewModel));

        [Key]
        public int Id { get; set; }

        [Required]
        public string InterestCode { get; set; }

        public string InterestType { get; set; }

        public string SubContractType { get; set; }

        public string EffectiveDate { get; set; }

        public string Description { get; set; }

        public decimal BankRate { get; set; }

        public decimal CoyRate { get; set; }

        public decimal RiskSpring { get; set; }

        public decimal RiskEthoz { get; set; }

        public int RepaymentPeriodFrom { get; set; }

        public int? RepaymentPeriodTo { get; set; }

        public string Remarks { get; set; }

        public string ActiveStatus { get; set; }
        public string DeactivationRemarks { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public DateTime? OriginalCreateDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
    }
    public class LEFSInterestCodeListViewModel
    {
        public LEFSInterestCodeListViewModel()
        {
            LEFSInterestCodeList = new List<LEFSInterestCodeList>();
        }
        [Display(Name = "LEFSInterestCodeList")]
        public List<LEFSInterestCodeList> LEFSInterestCodeList { get; set; }

        public string Action { get; set; }
    }


    public class LEFSInterestCodeList
    {
        public int Id { get; set; }

        public string Status { get; set; }
        public string ContractType { get; set; }
        public string InterestType { get; set; }
        public string InterestCode { get; set; }

        public string EffectiveDate { get; set; }

        public string Description { get; set; }

        public decimal BankRate { get; set; }

        public decimal CoyRate { get; set; }

        public decimal RiskSpring { get; set; }

        public decimal RiskEthoz { get; set; }

        public int RepaymentPeriodFrom { get; set; }

        public int? RepaymentPeriodTo { get; set; }

        public string Remarks { get; set; }

        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int IsPreContract { get; set; }
    }
    public class LEFSInterestCodeParam
    {
        public string Status { get; set; }
        public string ContractType { get; set; }
        public string InterestType { get; set; }
        public string InterestCode { get; set; }
    }
}