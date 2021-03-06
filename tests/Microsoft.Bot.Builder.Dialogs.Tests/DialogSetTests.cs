﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Bot.Builder.Dialogs.Tests
{
    [TestClass]
    public class DialogSetTests
    {

        [TestMethod]
        public void DialogSet_ConstructorValid()
        {
            var convoState = new ConversationState(new MemoryStorage());
            var dialogStateProperty = convoState.CreateProperty<DialogState>("dialogstate");
            var ds = new DialogSet(dialogStateProperty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DialogSet_ConstructorNullProperty()
        {
            var ds = new DialogSet(null);
        }

        [TestMethod]
        public async Task DialogSet_CreateContextAsync()
        {
            var convoState = new ConversationState(new MemoryStorage());
            var dialogStateProperty = convoState.CreateProperty<DialogState>("dialogstate");
            var ds = new DialogSet(dialogStateProperty);
            var context = TestUtilities.CreateEmptyContext();
            var dc = await ds.CreateContextAsync(context);
        }

        [TestMethod]
        public async Task DialogSet_NullCreateContextAsync()
        {
            var convoState = new ConversationState(new MemoryStorage());
            var dialogStateProperty = convoState.CreateProperty<DialogState>("dialogstate");
            var ds = new DialogSet(dialogStateProperty);
            var context = TestUtilities.CreateEmptyContext();
            var dc = await ds.CreateContextAsync(context);
        }

        [TestMethod]
        public async Task DialogSet_AddWorks()
        {
            var convoState = new ConversationState(new MemoryStorage());
            var dialogStateProperty = convoState.CreateProperty<DialogState>("dialogstate");
            var ds = new DialogSet(dialogStateProperty)
                .Add(new WaterfallDialog("A"))
                .Add(new WaterfallDialog("B"));
            Assert.IsNotNull(ds.Find("A"), "A is missing");
            Assert.IsNotNull(ds.Find("B"), "B is missing");
            Assert.IsNull(ds.Find("C"), "C should not be found");
            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task DialogSet_TelemetrySet()
        {
            var convoState = new ConversationState(new MemoryStorage());
            var dialogStateProperty = convoState.CreateProperty<DialogState>("dialogstate");
            var ds = new DialogSet(dialogStateProperty)
                .Add(new WaterfallDialog("A"))
                .Add(new WaterfallDialog("B"));
            Assert.IsTrue(ds.Find("A").TelemetryClient is NullBotTelemetryClient, "A not NullBotTelemetryClient");
            Assert.IsTrue(ds.Find("B").TelemetryClient is NullBotTelemetryClient, "A not NullBotTelemetryClient");

            var myTelemetryClient = new MyBotTelemetryClient();
            ds.TelemetryClient = myTelemetryClient;

            Assert.IsTrue(ds.Find("A").TelemetryClient is MyBotTelemetryClient, "A not MyBotTelemetryClient");
            Assert.IsTrue(ds.Find("B").TelemetryClient is MyBotTelemetryClient, "A not MyBotTelemetryClient");
            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task DialogSet_NullTelemetrySet()
        {

            var convoState = new ConversationState(new MemoryStorage());
            var dialogStateProperty = convoState.CreateProperty<DialogState>("dialogstate");
            var ds = new DialogSet(dialogStateProperty)
                .Add(new WaterfallDialog("A"))
                .Add(new WaterfallDialog("B"));

            ds.TelemetryClient = new MyBotTelemetryClient();
            ds.TelemetryClient = null;
            Assert.IsTrue(ds.Find("A").TelemetryClient is NullBotTelemetryClient, "A not NullBotTelemetryClient");
            Assert.IsTrue(ds.Find("B").TelemetryClient is NullBotTelemetryClient, "A not NullBotTelemetryClient");
            await Task.CompletedTask;

        }

        [TestMethod]
        public async Task DialogSet_AddTelemetrySet()
        {

            var convoState = new ConversationState(new MemoryStorage());
            var dialogStateProperty = convoState.CreateProperty<DialogState>("dialogstate");
            var ds = new DialogSet(dialogStateProperty)
                .Add(new WaterfallDialog("A"))
                .Add(new WaterfallDialog("B"));

            ds.TelemetryClient = new MyBotTelemetryClient();
            ds.Add(new WaterfallDialog("C"));

            Assert.IsTrue(ds.Find("C").TelemetryClient is MyBotTelemetryClient, "C (added dialog) not MyBotTelemetryClient");
            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task DialogSet_HeterogeneousLoggers()
        {

            var convoState = new ConversationState(new MemoryStorage());
            var dialogStateProperty = convoState.CreateProperty<DialogState>("dialogstate");
            var ds = new DialogSet(dialogStateProperty)
                .Add(new WaterfallDialog("A"))
                .Add(new WaterfallDialog("B"));
            ds.Add(new WaterfallDialog("C"));

            // Make sure we can override (after Adding) the TelemetryClient and "sticks"
            ds.Find("C").TelemetryClient = new MyBotTelemetryClient();

            Assert.IsTrue(ds.Find("A").TelemetryClient is NullBotTelemetryClient, "A not NullBotTelemetryClient");
            Assert.IsTrue(ds.Find("B").TelemetryClient is NullBotTelemetryClient, "B not NullBotTelemetryClient");
            Assert.IsTrue(ds.Find("C").TelemetryClient is MyBotTelemetryClient, "C (added dialog) not MyBotTelemetryClient");
            await Task.CompletedTask;
        }


        private class MyBotTelemetryClient : IBotTelemetryClient
        {
            public MyBotTelemetryClient()
            {

            }

            public void Flush()
            {
                throw new NotImplementedException();
            }

            public void TrackAvailability(string name, DateTimeOffset timeStamp, TimeSpan duration, string runLocation, bool success, string message = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
            {
                throw new NotImplementedException();
            }

            public void TrackDependency(string dependencyTypeName, string target, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, string resultCode, bool success)
            {
                throw new NotImplementedException();
            }

            public void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
            {
                throw new NotImplementedException();
            }

            public void TrackException(Exception exception, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
            {
                throw new NotImplementedException();
            }

            public void TrackTrace(string message, Severity severityLevel, IDictionary<string, string> properties)
            {
                throw new NotImplementedException();
            }
        }

    }
}
