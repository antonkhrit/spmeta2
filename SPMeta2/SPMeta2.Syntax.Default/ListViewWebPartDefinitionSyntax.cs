﻿using SPMeta2.Definitions;
using SPMeta2.Definitions.Fields;
using SPMeta2.Definitions.Webparts;
using SPMeta2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPMeta2.Models;
using SPMeta2.Syntax.Default.Extensions;

namespace SPMeta2.Syntax.Default
{
    public static class ListViewWebPartDefinitionSyntax
    {
        #region methods

        public static ModelNode AddListViewWebPart(this ModelNode model, ListViewWebPartDefinition definition)
        {
            return AddListViewWebPart(model, definition, null);
        }

        public static ModelNode AddListViewWebPart(this ModelNode model, ListViewWebPartDefinition definition, Action<ModelNode> action)
        {
            return model.AddDefinitionNode(definition, action);
        }

        #endregion

      
    }
}
