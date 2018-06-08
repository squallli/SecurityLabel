using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aitag.Models.ViewModels
{
    public class PriViewModel
    {
        public string id { get; set; }
        public IEnumerable<SelectListItem> DataManagement { get; set; }
        public IEnumerable<SelectListItem> PolicyManagement { get; set; }
        public IEnumerable<SelectListItem> memberManagement { get; set; }
        public IEnumerable<SelectListItem> applyManagement { get; set; }
        public IEnumerable<SelectListItem> eventManagement { get; set; }
        public IEnumerable<SelectListItem> copyManagement { get; set; }
        public IEnumerable<SelectListItem> ticketManagement { get; set; }
        public IEnumerable<SelectListItem> systemManagement { get; set; }
        public IEnumerable<SelectListItem> docfileManagement { get; set; }
        public IEnumerable<SelectListItem> inventoryManagement { get; set; }

    }
}