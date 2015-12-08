﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using biz.dfch.CS.Appclusive.UI.Models;

namespace biz.dfch.CS.Appclusive.UI.Controllers
{
    public class OrdersController : CoreControllerBase
    {
        // GET: Orders
        public ActionResult Index()
        {
            try
            {
                var items = CoreRepository.Orders.Take(PortalConfig.Pagesize).ToList();
                return View(AutoMapper.Mapper.Map<List<Models.Core.Order>>(items));
            }
            catch (Exception ex)
            {
                ((List<AjaxNotificationViewModel>)ViewBag.Notifications).AddRange(ExceptionHelper.GetAjaxNotifications(ex));
                return View(new List<Models.Core.Order>());
            }
        }

        #region Order

        // GET: Orders/Details/5
        public ActionResult Details(int id, int rId = 0, string rAction = null, string rController = null)
        {
            ViewBag.ReturnId = rId;
            ViewBag.ReturnAction = rAction;
            ViewBag.ReturnController = rController;
            try
            {
                var item = CoreRepository.Orders.Expand("OrderItems").Where(c => c.Id == id).FirstOrDefault();
                return View(AutoMapper.Mapper.Map<Models.Core.Order>(item));
            }
            catch (Exception ex)
            {
                ((List<AjaxNotificationViewModel>)ViewBag.Notifications).AddRange(ExceptionHelper.GetAjaxNotifications(ex));
                return View(new Models.Core.Order());
            }
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int id)
        {
            Api.Core.Order apiItem = null;
            try
            {
                apiItem = CoreRepository.Orders.Where(c => c.Id == id).FirstOrDefault();
                CoreRepository.DeleteObject(apiItem);
                CoreRepository.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorText = ex.Message;
                return View("Details", AutoMapper.Mapper.Map<Models.Core.Order>(apiItem));
            }
        }

        #endregion

        #region edit OrderItems

        public ActionResult ItemDetails(int id)
        {
            var item = CoreRepository.OrderItems.Expand("Order").Where(c => c.Id == id).FirstOrDefault();
            return View(AutoMapper.Mapper.Map<Models.Core.OrderItem>(item));
        }

        // GET: Orders/Delete/5
        public ActionResult ItemDelete(int id)
        {
            Api.Core.OrderItem apiItem = null;
            try
            {
                apiItem = CoreRepository.OrderItems.Expand("Order").Where(c => c.Id == id).FirstOrDefault();
                CoreRepository.DeleteObject(apiItem);
                CoreRepository.SaveChanges();
                return RedirectToAction("Details", new { id = apiItem.OrderId });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorText = ex.Message;
                return View("ItemDetails", AutoMapper.Mapper.Map<Models.Core.OrderItem>(apiItem));
            }
        }

        #endregion
    }
}