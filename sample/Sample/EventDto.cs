// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Sample
{
    public class EventDto
    {
        [JsonPropertyName("formatting")]
        public FormattingDto Formatting { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

        public string FormattedCreated => Created.ToString("dd MMM, HH:mm");
        public string Subject => Formatting?.Subject?.Values.FirstOrDefault() ?? string.Empty;
        public string Body => Formatting?.Body?.Values.FirstOrDefault() ?? string.Empty;
    }

    public class ListResponseDtoOfEventDto
    {
        [JsonPropertyName("items")]
        public ICollection<EventDto> Items { get; set; }
    }

    public class FormattingDto
    {
        [JsonPropertyName("subject")]
        public Dictionary<string, string> Subject { get; set; }

        [JsonPropertyName("body")]
        public Dictionary<string, string> Body { get; set; }
    }
}
