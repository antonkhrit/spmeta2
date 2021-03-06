﻿using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPMeta2.Containers.Assertion;
using SPMeta2.Definitions;
using SPMeta2.Definitions.Base;

using SPMeta2.SSOM.DefaultSyntax;
using SPMeta2.SSOM.ModelHandlers;
using SPMeta2.SSOM.ModelHosts;
using SPMeta2.Utils;


namespace SPMeta2.Regression.SSOM.Validation
{
    public class ListDefinitionValidator : ListModelHandler
    {
        public override void DeployModel(object modelHost, DefinitionBase model)
        {
            var webModelHost = modelHost.WithAssertAndCast<WebModelHost>("modelHost", value => value.RequireNotNull());
            var web = webModelHost.HostWeb;

            var definition = model.WithAssertAndCast<ListDefinition>("model", value => value.RequireNotNull());
            var spObject = web.GetList(SPUrlUtility.CombineUrl(web.ServerRelativeUrl, definition.GetListUrl()));

            var assert = ServiceFactory.AssertService.NewAssert(model, definition, spObject);

            assert
                .ShouldBeEqual(m => m.Title, o => o.Title)
                .ShouldBeEqual(m => m.Description, o => o.Description)
                .ShouldBeEndOf(m => m.GetListUrl(), m => m.Url, o => o.GetServerRelativeUrl(), o => o.GetServerRelativeUrl())
                .ShouldBeEqual(m => m.ContentTypesEnabled, o => o.ContentTypesEnabled);

            if (definition.TemplateType > 0)
            {
                assert
                    .ShouldBeEqual(m => m.TemplateType, o => (int)o.BaseTemplate)
                    .SkipProperty(m => m.TemplateName, "Skipping from validation. TemplateType should be == 0");
            }
            else
            {
                assert
                    .SkipProperty(m => m.TemplateType, "Skipping from validation. TemplateName should be empty");

                assert.ShouldBeEqual((p, s, d) =>
                {
                    var srcProp = s.GetExpressionValue(m => m.TemplateName);
                    var listTemplate = web.ListTemplates
                                          .OfType<SPListTemplate>()
                                          .FirstOrDefault(t => t.InternalName == definition.TemplateName);

                    return new PropertyValidationResult
                    {
                        Tag = p.Tag,
                        Src = srcProp,
                        Dst = null,
                        IsValid =
                            (spObject.TemplateFeatureId == listTemplate.FeatureId) &&
                            ((int)spObject.BaseTemplate == (int)listTemplate.Type)
                    };
                });
            }
        }
    }

    public static class ListExtensions
    {
        public static string GetServerRelativeUrl(this SPList list)
        {
            return list.RootFolder.ServerRelativeUrl;
        }
    }
}
