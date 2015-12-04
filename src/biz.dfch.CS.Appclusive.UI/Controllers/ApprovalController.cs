﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using biz.dfch.CS.Appclusive.UI.Models;

namespace biz.dfch.CS.Appclusive.UI.Controllers
{
    public class ApprovalsController : Controller
    {
        private biz.dfch.CS.Appclusive.Api.Core.Core CoreRepository
        {
            get
            {
                if (coreRepository == null)
                {
                    coreRepository = new biz.dfch.CS.Appclusive.Api.Core.Core(new Uri(Properties.Settings.Default.AppculsiveApiCoreUrl));
                    coreRepository.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                    coreRepository.IgnoreMissingProperties = true;
                }
                return coreRepository;
            }
        }
        private biz.dfch.CS.Appclusive.Api.Core.Core coreRepository;

        // GET: Approvals
        public ActionResult Index(int page = 1, string status = null)
        {
            IEnumerable<Api.Core.Approval> items;
            if (string.IsNullOrWhiteSpace(status))
            {
                int skipCount = page > 1 ? (page - 1) * PortalConfig.Pagesize : 0;
                items = CoreRepository.Approvals.Skip(skipCount).Take(PortalConfig.Pagesize).ToList();
                ViewBag.StatusFilter = status;
            }
            else
            {
                items = CoreRepository.Approvals.Where(a => a.Status == status);
            }
            switch (status)
            {
                case "Created": ViewBag.CreatedLinkClass = "active"; break;
                case "Approved": ViewBag.ApprovedLinkClass = "active"; break;
                case "Declined": ViewBag.DeclinedLinkClass = "active"; break;
                default: ViewBag.AllLinkClass = "active"; break;
            }
            return View(AutoMapper.Mapper.Map<List<Models.Core.Approval>>(items));
        }

        #region Approval 

        // GET: Approvals/Details/5
        public ActionResult Details(int id)
        {
            var item = CoreRepository.Approvals.Where(c => c.Id == id).FirstOrDefault();
            return View(AutoMapper.Mapper.Map<Models.Core.Approval>(item));
        }

        // GET: Approvals/Approve/5
        public ActionResult Approve(int id)
        {
            var apiItem = CoreRepository.Approvals.Where(c => c.Id == id).FirstOrDefault();
            Models.Core.Approval approval = AutoMapper.Mapper.Map<Models.Core.Approval>(apiItem);
            approval.Status = Models.Core.Approval.APPROVED_STATUS_CHANGE;
            approval.HelpText = "The request will be approved when you click the 'Approve' button. You can optionally add a explanation or reason for approval.";
            return View("Edit", approval);
        }

        // GET: Approvals/Decline/5
        public ActionResult Decline(int id)
        {
            var apiItem = CoreRepository.Approvals.Where(c => c.Id == id).FirstOrDefault();
            Models.Core.Approval approval = AutoMapper.Mapper.Map<Models.Core.Approval>(apiItem);
            approval.Status = Models.Core.Approval.DECLINED_STATUS_CHANGE;
            approval.HelpText = "The request will be declined when you click the 'Decline' button. You can optionally add a explanation or reason for approval.";
            return View("Edit", approval);
        }

        // POST: Approvals/Approve/5
        [HttpPost]
        public ActionResult Approve(int id, Models.Core.Approval approval)
        {
            return Edit(id, approval);
        }
        // POST: Approvals/Decline/5
        [HttpPost]
        public ActionResult Decline(int id, Models.Core.Approval approval)
        {
            return Edit(id, approval);
        }

        private ActionResult Edit(int id, Models.Core.Approval approval)
        {
            try
            {
                var apiItem = CoreRepository.Approvals.Where(c => c.Id == id).FirstOrDefault();
                
                #region copy all edited properties
                
                apiItem.Status = approval.Status;
                apiItem.Description = approval.Description;

                #endregion

                CoreRepository.UpdateObject(apiItem);
                CoreRepository.SaveChanges();

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewBag.ErrorText = ex.Message;
                return View("Edit", approval);
            }
        }


        #endregion

    }
}
