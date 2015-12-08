﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace biz.dfch.CS.Appclusive.UI.Models.Core
{
    public class Job : ViewModelBase, IAppcusiveEntityBase
    {
        
        public string Condition { get; set; }
        
        public string ConditionParameters { get; set; }
        
        public DateTimeOffset Created { get; set; }
        
        public string CreatedBy { get; set; }
        
        public string Description { get; set; }
        
        public DateTimeOffset? EndTime { get; set; }
        
        public string Error { get; set; }
        
        public long Id { get; set; }
        
        public DateTimeOffset Modified { get; set; }
        
        public string ModifiedBy { get; set; }
        
        public string Name { get; set; }
        
        public string Parameters { get; set; }
        
        public Job Parent { get; set; }

        [Display(Name = "Parent Id")]
        public long? ParentId { get; set; }
        
        [Display(Name="Ref Id")]
        public string ReferencedItemId { get; set; }
        
        public byte[] RowVersion { get; set; }
        
        public string Status { get; set; }

        [Display(Name = "Tenant Id")]
        public string TenantId { get; set; }
        
        public string Tid { get; set; }
        
        public string Token { get; set; }
    }
}