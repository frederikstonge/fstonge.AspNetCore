using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Randstad.Solutions.AspNetCoreRouting.Attributes;

namespace Randstad.Solutions.AspNetCoreRouting.Helpers
{
    internal class TranslationAttributeHelper
    {
        

        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public TranslationAttributeHelper(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }
        
        
    }
}