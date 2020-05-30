// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Mvc.TagHelpers
{
    using Microsoft.AspNetCore.Razor.TagHelpers;

    [HtmlTargetElement(Attributes = "hide-if")]
    public class HideIfTagHelper : TagHelper
    {
        [HtmlAttributeName("hide-if")]
        public bool HideIf { get; set; }

        public override void Process(
            TagHelperContext context, TagHelperOutput output)
        {
            if (this.HideIf)
            {
                output.SuppressOutput();
            }
        }
    }

    [HtmlTargetElement(Attributes = "show-if")]
    public class ShowIfTagHelper : TagHelper
    {
        [HtmlAttributeName("show-if")]
        public bool ShowIf { get; set; }

        public override void Process(
            TagHelperContext context, TagHelperOutput output)
        {
            if (this.ShowIf == false)
            {
                output.SuppressOutput();
            }
        }
    }

    [HtmlTargetElement(Attributes = "hide-if-null")]
    public class HideIfNullTagHelper : TagHelper
    {
        [HtmlAttributeName("hide-if-null")]
        public object HideIfNull { get; set; }

        public override void Process(
            TagHelperContext context, TagHelperOutput output)
        {
            if (this.HideIfNull == null)
            {
                output.SuppressOutput();
            }
        }
    }

    [HtmlTargetElement(Attributes = "show-if-null")]
    public class ShowifNullTagHelper : TagHelper
    {
        [HtmlAttributeName("show-if-null")]
        public object ShowIfNull { get; set; }

        public override void Process(
            TagHelperContext context, TagHelperOutput output)
        {
            if (this.ShowIfNull != null)
            {
                output.SuppressOutput();
            }
        }
    }
}