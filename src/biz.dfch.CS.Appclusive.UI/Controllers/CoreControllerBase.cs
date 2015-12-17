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

using biz.dfch.CS.Appclusive.UI.Models;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace biz.dfch.CS.Appclusive.UI.Controllers
{
    public class CoreControllerBase : Controller
    {

        /// <summary>
        /// biz.dfch.CS.Appclusive.Api.Core.Core
        /// </summary>
        protected biz.dfch.CS.Appclusive.Api.Core.Core CoreRepository
        {
            get
            {
                if (coreRepository == null)
                {
                    coreRepository = new biz.dfch.CS.Appclusive.Api.Core.Core(new Uri(Properties.Settings.Default.AppculsiveApiBaseUrl + "Core"));
                    coreRepository.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                    coreRepository.IgnoreMissingProperties = true;
                    coreRepository.Format.UseJson();
                    coreRepository.SaveChangesDefaultOptions = SaveChangesOptions.PatchOnUpdate;
                }
                return coreRepository;
            }
        }
        private biz.dfch.CS.Appclusive.Api.Core.Core coreRepository;

        public CoreControllerBase()
            : base()
        {
            ViewBag.Notifications = new List<AjaxNotificationViewModel>();
        }

        #region selection lists

        protected void AddProductSeletionToViewBag()
        {
            try
            {
                var products = CoreRepository.Products.ToList();
                ViewBag.ProductSelection = new SelectList(products, "Id", "Name");
            }
            catch (Exception ex)
            {
                ((List<AjaxNotificationViewModel>)ViewBag.Notifications).AddRange(ExceptionHelper.GetAjaxNotifications(ex));
            }
        }
        
        protected void AddEntityKindSeletionToViewBag()
        {
            try
            {
                var entityKinds = CoreRepository.EntityKinds.ToList();
                ViewBag.EntityKindSelection = new SelectList(entityKinds, "Id", "Name");
            }
            catch (Exception ex)
            {
                ((List<AjaxNotificationViewModel>)ViewBag.Notifications).AddRange(ExceptionHelper.GetAjaxNotifications(ex));
            }
        }

        protected void AddManagementCredentialSelectionToViewBag()
        {
            try
            {
                List<Api.Core.ManagementCredential> creds = new List<Api.Core.ManagementCredential>();
                creds.Add(new Api.Core.ManagementCredential() { Name = "-" });
                creds.AddRange(CoreRepository.ManagementCredentials.ToList());

                ViewBag.ManagementCredentialSelection = new SelectList(creds.Select(u => { return new { Id = u.Id > 0 ? (long?)u.Id : null, Name = u.Name }; }), "Id", "Name");
            }
            catch (Exception ex)
            {
                ((List<AjaxNotificationViewModel>)ViewBag.Notifications).AddRange(ExceptionHelper.GetAjaxNotifications(ex));
            }
        }

        protected void AddTenantSeletionToViewBag(Api.Core.Tenant currentTenant)
        {
            try
            {
                List<Api.Core.Tenant> tenants = new List<Api.Core.Tenant>();
                tenants.Add(new Api.Core.Tenant());
                if (null == currentTenant || currentTenant.ParentId == currentTenant.Id)// special seed entry in DB
                {
                    tenants.AddRange(CoreRepository.Tenants);
                }
                else
                {
                    tenants.AddRange(CoreRepository.Tenants.Where(t => t.Id != currentTenant.Id));
                }

                ViewBag.TenantSelection = new SelectList(tenants, "Id", "DisplayName");
            }
            catch (Exception ex)
            {
                ((List<AjaxNotificationViewModel>)ViewBag.Notifications).AddRange(ExceptionHelper.GetAjaxNotifications(ex));
            }
        }

        #endregion
    }
}