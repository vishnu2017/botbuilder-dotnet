﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AlarmBot.Models;
using AlarmBot.Topics;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using System.Threading.Tasks;

namespace AlarmBot
{
    public class AlarmBot : IBot
    {
        public async Task OnReceiveActivity(IBotContext botContext)
        {
            // Get the current ActiveTopic from my persisted conversation state
            var context = new AlarmBotContext(botContext);

            var handled = false;

            // if we don't have an active topic yet
            if (context.ConversationState.ActiveTopic == null)
            {
                // use the default topic
                context.ConversationState.ActiveTopic = new DefaultTopic();
                handled = await context.ConversationState.ActiveTopic.StartTopic(context);
            }
            else
            {
                // we do have an active topic, so call it 
                handled = await context.ConversationState.ActiveTopic.ContinueTopic(context);
            }

            // if activeTopic's result is false and the activeTopic is NOT already the default topic
            if (handled == false && !(context.ConversationState.ActiveTopic is DefaultTopic))
            {
                // Use DefaultTopic as the active topic
                context.ConversationState.ActiveTopic = new DefaultTopic();
                await context.ConversationState.ActiveTopic.ResumeTopic(context);
            }
        }
    }
}