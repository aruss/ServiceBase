// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification.Email
{
    using System;
    using System.Xml.Serialization;

    [Serializable()]
    public class EmailTemplate
    {
        [XmlElement("Subject")]
        public string Subject { get; set; }

        [XmlElement("Html")]
        public string Html { get; set; }

        [XmlElement("Text")]
        public string Text { get; set; }
    }
}
