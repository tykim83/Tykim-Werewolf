﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Werewolf.Utility
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> items, int selectedValue = 0)
        {
            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropertyValue("Name"),
                       Value = item.GetPropertyValue("Id"),
                       Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())
                   };
        }
    }
}