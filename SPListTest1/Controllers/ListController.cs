﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.SharePoint.Client;
using SPClient = Microsoft.SharePoint.Client;


namespace SPListTest1.Controllers
{
    public class ListController : Controller
    {
        string SiteUrl = "http://qtcserver/raymond/";
        string ListName = "Request_Test";

        public ActionResult Items()
        {
            ClientContext clientContext = new ClientContext(SiteUrl);

            // The object model does not automatically load data so we need to
            // use queries to retrieve data

            List spList = clientContext.Web.Lists.GetByTitle(ListName);
            clientContext.Load(spList);
            clientContext.ExecuteQuery();

            //Internal Names        ->  List Names 
            //NewColumn1            ->  Request ID
            //Request_x0020_Details ->  Request Details
            //Author                ->  Request By
            //Request_x0020_Status  ->  Request Status

            CamlQuery camlQuery = new CamlQuery();
            ListItemCollection spListItems = spList.GetItems(camlQuery);
            clientContext.Load(spListItems, items => items.IncludeWithDefaultProperties(
                item => item["NewColumn1"],
                item => item["Request_x0020_Details"],
                item => item["Author"],
                item => item["Request_x0020_Status"]));
            clientContext.ExecuteQuery();

            ViewBag.List = spList;
            ViewBag.ListItems = spListItems;
            return View();
        }

        [HttpGet]
        public ActionResult NewItem()
        {
            return View();
        }

        [HttpPost]
        public RedirectResult NewItem(string details)
        {
            ClientContext clientContext = new ClientContext(SiteUrl);

            // For testing purpose, we'll use a local account jdoe for the new item;
            // otherwise the Requested By (i.e. Author) field will be populated with
            // the user who runs the ASP.NET web applicationn.

            var user = clientContext.Web.EnsureUser("rwu8");
            clientContext.Load(user);
            clientContext.ExecuteQuery();
            FieldUserValue userValue = new FieldUserValue();
            userValue.LookupId = user.Id;

            List spList = clientContext.Web.Lists.GetByTitle(ListName);
            var info = new ListItemCreationInformation();
            var item = spList.AddItem(info);
            item["Request_x0020_Details"] = details;
            item["Author"] = userValue;
            item.Update();
            clientContext.ExecuteQuery();

            return Redirect("Items");
        }

        [HttpGet]
        public ActionResult DeleteItem(int ID)
        {
            ClientContext clientContext = new ClientContext(SiteUrl);
            List spList = clientContext.Web.Lists.GetByTitle(ListName);
            ListItem spListItem = spList.GetItemById(ID);

            spListItem.DeleteObject();
            clientContext.ExecuteQuery();

            return Redirect("Items");
        }
    }
}