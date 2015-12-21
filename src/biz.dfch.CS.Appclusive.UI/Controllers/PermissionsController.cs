﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using biz.dfch.CS.Appclusive.UI.Models;
using System.Data.Services.Client;

namespace biz.dfch.CS.Appclusive.UI.Controllers
{
    public class PermissionsController : CoreControllerBase
    {

        // GET: Permissions
        public ActionResult Index(int pageNr = 1)
        {
            try
            {
                QueryOperationResponse<Api.Core.Permission> items = CoreRepository.Permissions
                        .AddQueryOption("$inlinecount", "allpages")
                        .AddQueryOption("$top", PortalConfig.Pagesize)
                        .AddQueryOption("$skip", (pageNr - 1) * PortalConfig.Pagesize)
                        .Execute() as QueryOperationResponse<Api.Core.Permission>;

                ViewBag.Paging = new PagingInfo(pageNr, items.TotalCount);
                return View(AutoMapper.Mapper.Map<List<Models.Core.Permission>>(items));
            }
            catch (Exception ex)
            {
                ((List<AjaxNotificationViewModel>)ViewBag.Notifications).AddRange(ExceptionHelper.GetAjaxNotifications(ex));
                return View(new List<Models.Core.Permission>());
            }
        }

        #region Permission

        // GET: Permissions/Details/5
        public ActionResult Details(long id, int rId = 0, string rAction = null, string rController = null)
        {
            ViewBag.ReturnId = rId;
            ViewBag.ReturnAction = rAction;
            ViewBag.ReturnController = rController;
            try
            {
                var item = CoreRepository.Permissions.Expand("CreatedBy").Expand("ModifiedBy").Where(c => c.Id == id).FirstOrDefault();
                return View(AutoMapper.Mapper.Map<Models.Core.Permission>(item));
            }
            catch (Exception ex)
            {
                ((List<AjaxNotificationViewModel>)ViewBag.Notifications).AddRange(ExceptionHelper.GetAjaxNotifications(ex));
                return View(new Models.Core.Permission());
            }
        }

        // GET: Permissions/Create
        public ActionResult Create()
        {
            this.AddAclSeletionToViewBag();
            return View(new Models.Core.Permission());
        }

        // POST: Permissions/Create
        [HttpPost]
        public ActionResult Create(Models.Core.Permission permission)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(permission);
                }
                else
                {
                    var apiItem = AutoMapper.Mapper.Map<Api.Core.Permission>(permission);

                    CoreRepository.AddToPermissions(apiItem);
                    CoreRepository.SaveChanges();

                    return RedirectToAction("Details", new { id = apiItem.Id });
                }
            }
            catch (Exception ex)
            {
                ((List<AjaxNotificationViewModel>)ViewBag.Notifications).AddRange(ExceptionHelper.GetAjaxNotifications(ex));
                this.AddAclSeletionToViewBag();
                return View(permission);
            }
        }

        // GET: Permissions/Edit/5
        public ActionResult Edit(long id)
        {
            this.AddAclSeletionToViewBag();
            try
            {
                var apiItem = CoreRepository.Permissions.Expand("CreatedBy").Expand("ModifiedBy").Where(c => c.Id == id).FirstOrDefault();
                return View(AutoMapper.Mapper.Map<Models.Core.Permission>(apiItem));
            }
            catch (Exception ex)
            {
                ((List<AjaxNotificationViewModel>)ViewBag.Notifications).AddRange(ExceptionHelper.GetAjaxNotifications(ex));
                return View(new Models.Core.Permission());
            }
        }

        // POST: Permissions/Edit/5
        [HttpPost]
        public ActionResult Edit(long id, Models.Core.Permission permission)
        {
            this.AddAclSeletionToViewBag();
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(permission);
                }
                else
                {
                    var apiItem = CoreRepository.Permissions.Expand("CreatedBy").Expand("ModifiedBy").Where(c => c.Id == id).FirstOrDefault();

                    #region copy all edited properties

                    apiItem.Name = permission.Name;
                    apiItem.Description = permission.Description;

                    #endregion
                    CoreRepository.UpdateObject(apiItem);
                    CoreRepository.SaveChanges();
                    ((List<AjaxNotificationViewModel>)ViewBag.Notifications).Add(new AjaxNotificationViewModel(ENotifyStyle.success, "Successfully saved"));
                    return View(AutoMapper.Mapper.Map<Models.Core.Permission>(apiItem));
                }
            }
            catch (Exception ex)
            {
                ((List<AjaxNotificationViewModel>)ViewBag.Notifications).AddRange(ExceptionHelper.GetAjaxNotifications(ex));
                return View(permission);
            }
        }

        // GET: Permissions/Delete/5
        public ActionResult Delete(long id)
        {
            Api.Core.Permission apiItem = null;
            try
            {
                apiItem = CoreRepository.Permissions.Expand("CreatedBy").Expand("ModifiedBy").Where(c => c.Id == id).FirstOrDefault();
                CoreRepository.DeleteObject(apiItem);
                CoreRepository.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ((List<AjaxNotificationViewModel>)ViewBag.Notifications).AddRange(ExceptionHelper.GetAjaxNotifications(ex));
                this.AddAclSeletionToViewBag();
                return View("Details", AutoMapper.Mapper.Map<Models.Core.Permission>(apiItem));
            }
        }


        #endregion
        
    }
}
