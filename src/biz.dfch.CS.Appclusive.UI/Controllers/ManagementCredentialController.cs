﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using biz.dfch.CS.Appclusive.UI.Models;

namespace biz.dfch.CS.Appclusive.UI.Controllers
{
    public class ManagementCredentialsController : Controller
    {
        private biz.dfch.CS.Appclusive.Api.Core.Core CoreRepository
        {
            get
            {
                if (coreRepository == null)
                {
                    coreRepository = new biz.dfch.CS.Appclusive.Api.Core.Core(new Uri(Properties.Settings.Default.AppculsiveApiCoreUrl));
                    coreRepository.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                }
                return coreRepository;
            }
        }
        private biz.dfch.CS.Appclusive.Api.Core.Core coreRepository;

        // GET: ManagementCredentials
        public ActionResult Index()
        {
            var items = CoreRepository.ManagementCredentials.Take(PortalConfig.Pagesize).ToList();
            return View(AutoMapper.Mapper.Map<List<Models.Core.ManagementCredential>>(items));
        }

        #region ManagementCredential 

        // GET: ManagementCredentials/Details/5
        public ActionResult Details(int id)
        {
            var item = CoreRepository.ManagementCredentials.Expand("ManagementUris").Where(c => c.Id == id).FirstOrDefault();
            return View(AutoMapper.Mapper.Map<Models.Core.ManagementCredential>(item));
        }

        // GET: ManagementCredentials/Create
        public ActionResult Create()
        {
            return View(new Models.Core.ManagementCredential());
        }

        // POST: ManagementCredentials/Create
        [HttpPost]
        public ActionResult Create(Models.Core.ManagementCredential managementCredential)
        {
            try
            {
                var apiItem = AutoMapper.Mapper.Map<Api.Core.ManagementCredential>(managementCredential);
                
                CoreRepository.AddToManagementCredentials(apiItem);
                CoreRepository.SaveChanges();

                return RedirectToAction("Details", new { id = apiItem.Id });
            }
            catch(Exception ex)
            {
                managementCredential.ErrorText = ex.Message;
                return View(managementCredential);
            }
        }

        // GET: ManagementCredentials/Edit/5
        public ActionResult Edit(int id)
        {
            var apiItem = CoreRepository.ManagementCredentials.Where(c => c.Id == id).FirstOrDefault();
            return View(AutoMapper.Mapper.Map<Models.Core.ManagementCredential>(apiItem));
        }

        // POST: ManagementCredentials/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Models.Core.ManagementCredential managementCredential)
        {
            try
            {
                var apiItem = CoreRepository.ManagementCredentials.Expand("ManagementUris").Where(c => c.Id == id).FirstOrDefault();

                #region copy all edited properties

                // TODO: set edited properties
                //apiItem.Name = ManagementCredential.Name;
                //apiItem.Description = ManagementCredential.Description;
                //apiItem.Status = ManagementCredential.Status;
                //apiItem.Version = ManagementCredential.Version;

                #endregion
                CoreRepository.UpdateObject(apiItem);
                CoreRepository.SaveChanges();

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                managementCredential.ErrorText = ex.Message;
                return View(managementCredential);
            }
        }

        // GET: ManagementCredentials/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }


        #endregion

    }
}
