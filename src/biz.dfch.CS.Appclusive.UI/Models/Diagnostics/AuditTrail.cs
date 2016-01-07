﻿/**
 * Copyright 2015 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using biz.dfch.CS.Appclusive.UI.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace biz.dfch.CS.Appclusive.UI.Models.Diagnostics
{
    public class AuditTrail : AppcusiveEntityViewModelBase
    {

        [Display(Name = "Current", ResourceType = typeof(GeneralResources))] 
        public string Current { get; set; }

        [Display(Name = "EntityId",ResourceType = typeof(GeneralResources))] 
        public string EntityId { get; set; }
        
        public string EntityState { get; set; }
        
        public string EntityType { get; set; }
        
        public string Original { get; set; }
        
    }
}