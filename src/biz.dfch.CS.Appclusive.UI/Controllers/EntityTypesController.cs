﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using biz.dfch.CS.Appclusive.UI.Models;

namespace biz.dfch.CS.Appclusive.UI.Controllers
{
    public class EntityTypesController : CoreControllerBase
    {

        // GET: EntityTypes
        public ActionResult Index()
        {
            try
            {
                var items = CoreRepository.EntityTypes.Take(PortalConfig.Pagesize).ToList();
                return View(AutoMapper.Mapper.Map<List<Models.Core.EntityType>>(items));
            }
            catch (Exception ex)
            {
                ((List<AjaxNotificationViewModel>)ViewBag.Notifications).AddRange(ExceptionHelper.GetAjaxNotifications(ex));
                return View(new List<Models.Core.EntityType>());
            }
        }

        #region EntityType

        // GET: EntityTypes/Details/5
        public ActionResult Details(int id)
        {
            var item = CoreRepository.EntityTypes.Where(c => c.Id == id).FirstOrDefault();
            return View(AutoMapper.Mapper.Map<Models.Core.EntityType>(item));
        }

        // GET: EntityTypes/Create
        public ActionResult Create()
        {
            return View(new Models.Core.EntityType());
        }

        // POST: EntityTypes/Create
        [HttpPost]
        public ActionResult Create(Models.Core.EntityType entityType)
        {
            try
            {
                var apiItem = AutoMapper.Mapper.Map<Api.Core.EntityType>(entityType);

                CoreRepository.AddToEntityTypes(apiItem);
                CoreRepository.SaveChanges();

                return RedirectToAction("Details", new { id = apiItem.Id });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorText = ex.Message;
                return View(entityType);
            }
        }

        // GET: EntityTypes/Edit/5
        public ActionResult Edit(int id)
        {
            var apiItem = CoreRepository.EntityTypes.Where(c => c.Id == id).FirstOrDefault();
            return View(AutoMapper.Mapper.Map<Models.Core.EntityType>(apiItem));
        }

        // POST: EntityTypes/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Models.Core.EntityType entityType)
        {
            try
            {
                var apiItem = CoreRepository.EntityTypes.Where(c => c.Id == id).FirstOrDefault();

                #region copy all edited properties

                // TODO: set edited properties
                //apiItem.Name = EntityType.Name;
                //apiItem.Description = EntityType.Description;
                //apiItem.Status = EntityType.Status;
                //apiItem.Version = EntityType.Version;

                #endregion
                CoreRepository.UpdateObject(apiItem);
                CoreRepository.SaveChanges();
                ViewBag.InfoText = "Successfully saved";
                return View(AutoMapper.Mapper.Map<Models.Core.EntityType>(apiItem));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorText = ex.Message;
                return View(entityType);
            }
        }

        // GET: EntityTypes/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var apiItem = CoreRepository.EntityTypes.Where(c => c.Id == id).FirstOrDefault();
                CoreRepository.DeleteObject(apiItem);
                return Index();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorText = ex.Message;
                return View();
            }
        }


        #endregion

    }
}