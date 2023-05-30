// <copyright file="TicketCard.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.RemoteSupport.Cards
{
    using System;
    using System.Collections.Generic;
    using AdaptiveCards;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Localization;
    using Microsoft.Teams.Apps.RemoteSupport.Common;
    using Microsoft.Teams.Apps.RemoteSupport.Common.Models;
    using Microsoft.Teams.Apps.RemoteSupport.Helpers;
    using Microsoft.Teams.Apps.RemoteSupport.Models;
    using Newtonsoft.Json;

    /// <summary>
    /// Provides adaptive cards for creating and editing new ticket information.
    /// </summary>
    public static class TicketCard
    {
        /// <summary>
        /// Get the create new ticket card.
        /// </summary>
        /// <param name="cardConfiguration">Card configuration.</param>
        /// <param name="localizer">The current cultures' string localizer.</param>
        /// <param name="showValidationMessage">Represents whether to show validation message or not.</param>
        /// <param name="ticketDetail"> Information of the ticket which is being created.</param>
        /// <returns>Returns an attachment of new ticket.</returns>
        public static Attachment GetNewTicketCard(CardConfigurationEntity cardConfiguration, IStringLocalizer<Strings> localizer, bool showValidationMessage = false, TicketDetail ticketDetail = null)
        {
            cardConfiguration = cardConfiguration ?? throw new ArgumentNullException(nameof(cardConfiguration));

            string issueDescription = string.Empty;
            string issueCategory = string.Empty;

            var dynamicElements = new List<AdaptiveElement>();
            var ticketAdditionalFields = new List<AdaptiveElement>();
            bool showDescriptionValidation = false;
            bool showCategoryValidation = false;
            bool showDateValidation = false;

            ticketAdditionalFields = CardHelper.ConvertToAdaptiveCard(localizer, cardConfiguration.CardTemplate, showDateValidation);

            dynamicElements.AddRange(new List<AdaptiveElement>
            {
                new AdaptiveTextBlock
                {
                    Text = localizer.GetString("NewRequestTitle"),
                    Weight = AdaptiveTextWeight.Bolder,
                    Size = AdaptiveTextSize.Large,
                },
            });
        }

        /// <summary>
        /// Card to show ticket details in 1:1 chat with bot after submitting request details.
        /// </summary>
        /// <param name="cardElementMapping">Represents Adaptive card item element {Id, display name} mapping.</param>
        /// <param name="ticketDetail">New ticket values entered by user.</param>
        /// <param name="localizer">The current cultures' string localizer.</param>
        /// <param name="isEdited">flag that sets when card is edited.</param>
        /// <returns>An attachment with ticket details.</returns>
        public static Attachment GetTicketDetailsForPersonalChatCard(Dictionary<string, string> cardElementMapping, TicketDetail ticketDetail, IStringLocalizer<Strings> localizer, bool isEdited = false)
        {
            ticketDetail = ticketDetail ?? throw new ArgumentNullException(nameof(ticketDetail));
            cardElementMapping = cardElementMapping ?? throw new ArgumentNullException(nameof(cardElementMapping));

            Dictionary<string, string> ticketAdditionalDetail = JsonConvert.DeserializeObject<Dictionary<string, string>>(ticketDetail.AdditionalProperties);
            var dynamicElements = new List<AdaptiveElement>();
            var ticketAdditionalFields = new List<AdaptiveElement>();

            foreach (KeyValuePair<string, string> item in ticketAdditionalDetail)
            {
                string key = item.Key;
                if (item.Key.Equals(CardConstants.IssueOccurredOnId, StringComparison.OrdinalIgnoreCase))
                {
                    key = localizer.GetString("FirstObservedText");
                }

                ticketAdditionalFields.Add(CardHelper.GetAdaptiveCardColumnSet(cardElementMapping.ContainsKey(key) ? cardElementMapping[key] : key, item.Value, localizer));
            }

            dynamicElements.AddRange(new List<AdaptiveElement>
            {
                new AdaptiveTextBlock
                {
                    Text = isEdited == true ? localizer.GetString("RequestUpdatedText") : localizer.GetString("RequestSubmittedText"),
                    Weight = AdaptiveTextWeight.Bolder,
                    Size = AdaptiveTextSize.Large,
                },
                new AdaptiveTextBlock()
                {
                    Text = localizer.GetString("RequestSubmittedContent"),
                    Wrap = true,
                    Spacing = AdaptiveSpacing.None,
                },
                CardHelper.GetAdaptiveCardColumnSet(localizer.GetString("RequestNumberText"), $"#{ticketDetail.RowKey}", localizer),
                CardHelper.GetAdaptiveCardColumnSet(localizer.GetString("CategoryTypeText"), ticketDetail.CategoryType, localizer),
                CardHelper.GetAdaptiveCardColumnSet(localizer.GetString("RequestTypeText"), ticketDetail.RequestType, localizer),
            });
            dynamicElements.AddRange(ticketAdditionalFields);
            dynamicElements.Add(CardHelper.GetAdaptiveCardColumnSet(localizer.GetString("DescriptionText"), ticketDetail.Description, localizer));
        }
    }
}
