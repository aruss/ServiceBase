namespace ServiceBase.PayPal.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CreateOrderResult
    {
        public CreateOrderResponseBody Order { get; set; }
        public ErrorResponseBody Error { get; set; }
    }

    public static class CreateOrderResultExtensions
    {
        public static LinkDescription GetApproveLink(
            this CreateOrderResult result)
        {
            return result?.Order?.Links?.GetLink("approve");
        }

        public static bool IsCreated(this CreateOrderResult result)
        {
            if (result?.Order == null)
            {
                return false;
            }

            return "CREATED".Equals(
                result.Order.Status,
                StringComparison.InvariantCultureIgnoreCase
            );
        }

        public static LinkDescription GetLink(
            this IEnumerable<LinkDescription> links,
            string rel)
        {
            if (string.IsNullOrWhiteSpace(rel))
            {
                return null;
            }

            return links?.FirstOrDefault(
                c => rel.Equals(
                    c.Rel,
                    StringComparison.InvariantCultureIgnoreCase
                )
            );
        }

    }
}
