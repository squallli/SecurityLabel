using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using UniteErp.Models;

namespace UniteErp.Helper
{
    public static class UIC
    {
        public static IEnumerable<SelectListItem> ToCheckBoxListSource<T,T2>(this IEnumerable<T> checkedCollection,
                                          IEnumerable<T2> allCollection)
                                          where T : SelectListItemBaseEntity
                                          where T2 :SelectListItemBaseEntity
        {
            var result = new List<SelectListItem>();

            foreach (var allItem in allCollection)
            {
                var selectItem = new SelectListItem();
                selectItem.Text = allItem.Text.ToString();
                selectItem.Value = allItem.ID.ToString();
                selectItem.Selected = (checkedCollection!=null && checkedCollection.Count(c => c.ID == allItem.ID) > 0);

                result.Add(selectItem);
            }

            return result;
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper helper, string name,
                                         IEnumerable<SelectListItem> items)
        {
            var output = new StringBuilder();
            output.Append(@"<div class=""checkboxList"">");

            foreach (var item in items)
            {
                output.Append(@"<input type=""checkbox"" name=""");
                output.Append(name);
                output.Append("\" value=\"");
                output.Append(item.Value);
                output.Append("\"");

                if (item.Selected)
                    output.Append(@" checked=""checked""");

                output.Append(" />");
                output.Append(item.Text);
                output.Append("<br />");
            }

            output.Append("</div>");

            return new MvcHtmlString(output.ToString());
        }
    }
}