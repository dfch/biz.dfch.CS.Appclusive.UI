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
using biz.dfch.CS.Appclusive.UI.Config;
using biz.dfch.CS.Appclusive.UI.Models;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Web.Mvc;

namespace biz.dfch.CS.Appclusive.UI.Controllers
{
    public class NodesController : CoreControllerBase<Api.Core.Node, Models.Core.Node, Models.Core.Node>
    {
        protected override DataServiceQuery<Api.Core.Node> BaseQuery { get { return CoreRepository.Nodes.Expand("EntityKind"); } }

        // GET: Nodes/Details/5
        public ActionResult Details(long id, string rId = "0", string rAction = null, string rController = null)
        {
            ViewBag.ReturnId = rId;
            ViewBag.ReturnAction = rAction;
            ViewBag.ReturnController = rController;
            AddCheckNodePermissionObject(id);
            try
            {
                // load Node and Children
                Models.Core.Node modelItem = LoadDetailModel(id);
                return View(modelItem);
            }
            catch (Exception ex)
            {
                ((List<AjaxNotificationViewModel>)ViewBag.Notifications).AddRange(ExceptionHelper.GetAjaxNotifications(ex));
                return View(new Models.Core.Node());
            }
        }

        private Models.Core.Node LoadDetailModel(long id)
        {
            var item = CoreRepository.Nodes.Expand("Parent").Expand("EntityKind").Expand("CreatedBy").Expand("ModifiedBy").Where(c => c.Id == id).FirstOrDefault();
            Models.Core.Node modelItem = AutoMapper.Mapper.Map<Models.Core.Node>(item);
            if (null != modelItem)
            {
                try
                {
                    modelItem.Children = LoadNodeChildren(id, 1);
                    modelItem.ResolveSecurity(this.CoreRepository);
                    modelItem.ResolveJob(this.CoreRepository);
                    modelItem.ResolveReferencedEntityName(this.CoreRepository);
                }
                catch (Exception ex) { ((List<AjaxNotificationViewModel>)ViewBag.Notifications).AddRange(ExceptionHelper.GetAjaxNotifications(ex)); }
            }
            return modelItem;
        }

        #region Node-children list and search

        // GET: Nodes/ItemList
        public PartialViewResult ItemIndex(long parentId, int pageNr = 1, string itemSearchTerm = null, string orderBy = null)
        {
            ViewBag.ParentId = parentId;
            DataServiceQuery<Api.Core.Node> itemsBaseQuery = CoreRepository.Nodes;
            string itemsBaseFilter = "ParentId eq " + parentId;
            return base.ItemIndex<Api.Core.Node, Models.Core.Node>(itemsBaseQuery, itemsBaseFilter, pageNr, itemSearchTerm, orderBy);
        }

        private List<Models.Core.Node> LoadNodeChildren(long parentId, int pageNr)
        {
            QueryOperationResponse<Api.Core.Node> items = CoreRepository.Nodes
                    .AddQueryOption("$filter", "ParentId eq " + parentId)
                    .AddQueryOption("$inlinecount", "allpages")
                    .AddQueryOption("$top", PortalConfig.Pagesize)
                    .AddQueryOption("$skip", (pageNr - 1) * PortalConfig.Pagesize)
                    .Execute() as QueryOperationResponse<Api.Core.Node>;

            ViewBag.ParentId = parentId;
            ViewBag.AjaxPaging = new PagingInfo(pageNr, items.TotalCount);

            return AutoMapper.Mapper.Map<List<Models.Core.Node>>(items);
        }

        public ActionResult ItemSearch(long parentId, string term)
        {
            DataServiceQuery<Api.Core.Node> itemsBaseQuery = CoreRepository.Nodes;
            string itemsBaseFilter = "ParentId eq " + parentId;
            return base.ItemSearch(itemsBaseQuery, itemsBaseFilter, term);
        }

        #endregion Node-children list

        #region treeview

        public ActionResult Tree()
        {
            List<Models.Tree.Node> nodeList = new List<Models.Tree.Node>();
            #region order by

            Dictionary<string, string> ov = new Dictionary<string, string>();
            ov.Add("", GeneralResources.OrderByDefault);
            ov.Add("Name", "Name " + GeneralResources.OrderByAsc);
            ov.Add("Name desc", "Name " + GeneralResources.OrderByDesc);
            ViewBag.OrderBySelection = new SelectList(ov, "Key", "Value");

            #endregion
            try
            {
                // load root Node and Children
                long id = biz.dfch.CS.Appclusive.Contracts.Constants.SYSTEM_TENANT_ROOT_NODE;
                var item = CoreRepository.Nodes.Where(c => c.Id == id).FirstOrDefault();

                Models.Tree.Node root = new Models.Tree.Node()
                {
                    key = item.Id.ToString(),
                    title = item.Name,
                    tooltip = item.Description,
                    lazy = true,
                    expanded = false,
                    folder = true
                };

                nodeList.Add(root);
                return View(nodeList);
            }
            catch (Exception ex)
            {
                ((List<AjaxNotificationViewModel>)ViewBag.Notifications).AddRange(ExceptionHelper.GetAjaxNotifications(ex));
                return View(nodeList);
            }
        }

        public ActionResult TreeData(long parentId = 0, int pageNr = 1, string searchTerm = null, string orderBy = null, object _ = null)
        {
            List<Models.Tree.Node> nodeList = new List<Models.Tree.Node>();
            bool addParents = false;
            int pageSize = PortalConfig.Pagesize;
            try
            {
                #region query and load list

                DataServiceQuery<Api.Core.Node> query = this.BaseQuery;
                if (parentId <= 0)
                {
                    if (string.IsNullOrWhiteSpace(searchTerm))
                    {
                        //  load root Node
                        query = AddPagingOptions(query, pageNr);
                        query = query.AddQueryOption("$filter", string.Format("EntityKindId eq {0}", biz.dfch.CS.Appclusive.Contracts.Constants.EntityKindId.TenantRoot.GetHashCode()));
                    }
                    else
                    {
                        // search nodes
                        query = AddPagingOptions(query, pageNr, PortalConfig.SearchLoadSize);
                        query = AddSearchFilter(query, searchTerm);
                        query = AddOrderOptions(query, orderBy);
                        addParents = true;
                        pageSize = PortalConfig.SearchLoadSize;
                    }
                }
                else
                {
                    // load child list
                    query = AddPagingOptions(query, pageNr);
                    query = query.AddQueryOption("$filter", string.Format("ParentId eq {0}", parentId));
                    query = AddOrderOptions(query, orderBy);
                }

                QueryOperationResponse<Api.Core.Node> items = query.Execute() as QueryOperationResponse<Api.Core.Node>;
                PagingInfo pi = new PagingInfo(pageNr, items.TotalCount, pageSize);

                List<Models.Core.Node> modelItems = AutoMapper.Mapper.Map<List<Models.Core.Node>>(items);

                #endregion

                // find parents                
                if (addParents)
                {
                    List<Models.Core.Node> foundItems = modelItems.ToList();
                    foreach (Models.Core.Node child in foundItems)
                    {
                        FindParent(modelItems, child);
                    }
                    modelItems = modelItems.Where(n => n.Parent == null).ToList();// root nodes
                }

                // create fancytree-nodes
                foreach (var child in modelItems)
                {
                    if (child.Id != parentId)
                    {
                        Models.Tree.Node node = CreateTreeNode(child);
                        if (null != node)
                        {
                            node.pageInfo = pi;
                            nodeList.Add(node);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.Message);
            }
            return this.Json(nodeList, JsonRequestBehavior.AllowGet);
        }

        private static Models.Tree.Node CreateTreeNode(Models.Core.Node child)
        {
            // node
            Models.Tree.Node node = new Models.Tree.Node()
                {
                    key = child.Id.ToString(),
                    title = child.Name,
                    tooltip = child.Description,
                    lazy = true,
                    expanded = false,
                    folder = false
                };

            // icon
            if (null != child.EntityKind)
            {
                string icon = null;
                if (null != child.EntityKind.EntityType)
                {
                    icon = NavigationConfig.GetIcon(child.EntityKind.EntityType);
                }
                else
                {
                    icon = NavigationConfig.GetIcon(child.EntityKind.Name);
                }
                if (!string.IsNullOrWhiteSpace(icon))
                {
                    node.icon = "fa " + icon;
                }
            }

            // children
            foreach (var grandChild in child.Children)
            {
                Models.Tree.Node grandChildNode = CreateTreeNode(grandChild);
                if (null != grandChildNode)
                {
                    if (null == node.children)
                    {
                        node.children = new List<Models.Tree.Node>();
                        node.lazy = false;
                        node.expanded = true;
                    }
                    node.children.Add(grandChildNode);
                }
            }
            return node;
        }

        private void FindParent(List<Models.Core.Node> modelItems, Models.Core.Node child)
        {
            if (child.Parent == null && child.ParentId.HasValue && child.ParentId > 0 && child.Id != child.ParentId.Value)
            {
                var parent = modelItems.FirstOrDefault(n => n.Id == child.ParentId);
                if (null == parent)
                {
                    parent = AutoMapper.Mapper.Map<Models.Core.Node>(this.BaseQuery.Where(n => n.Id == child.ParentId.Value).FirstOrDefault());
                    if (null != parent)
                    {
                        modelItems.Add(parent);
                    }
                }
                if (null != parent)
                {
                    child.Parent = parent;
                    parent.Children.Add(child);
                    FindParent(modelItems, parent);
                }
            }
        }

        public PartialViewResult TreeDetails(long id)
        {
            AddCheckNodePermissionObject(id);
            try
            {
                // load Node and Children
                Models.Core.Node modelItem = LoadDetailModel(id);
                return PartialView(modelItem);
            }
            catch (Exception ex)
            {
                ((List<AjaxNotificationViewModel>)ViewBag.Notifications).AddRange(ExceptionHelper.GetAjaxNotifications(ex));
                return PartialView(new Models.Core.Node());
            }
        }

        #endregion

        [HttpPost]
        public PartialViewResult CheckPermission(Models.SpecialOperations.CheckPermission cp)
        {
            try
            {
                cp.ResolveNavigationProperties(this.CoreRepository);
                if (!ModelState.IsValid)
                {
                    return PartialView(cp);
                }
                else
                {
                    cp.Granted = this.CoreRepository.InvokeEntityActionWithSingleResult<Boolean>("Nodes", cp.NodeId, "CheckPermission",
                        new { PermissionId = cp.PermissionId, TrusteeId = cp.TrusteeId, TrusteeType = cp.TrusteeType }
                        );
                    return PartialView(cp);
                }
            }
            catch (Exception ex)
            {
                ((List<AjaxNotificationViewModel>)ViewBag.Notifications).AddRange(ExceptionHelper.GetAjaxNotifications(ex));
                return PartialView(cp);
            }
        }

        private void AddCheckNodePermissionObject(long nodeId)
        {
            var cp = new Models.SpecialOperations.CheckPermission()
            {
                NodeId = nodeId,
                TrusteeType = Models.Core.TrusteeTypeEnum.User.GetHashCode()
            };
            cp.ResolveNavigationProperties(this.CoreRepository);
            ViewBag.CheckNodePermission = cp;
        }

    }
}
