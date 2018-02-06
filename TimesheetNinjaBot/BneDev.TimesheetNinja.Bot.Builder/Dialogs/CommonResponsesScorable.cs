using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;
using BneDev.TimesheetNinja.Bot.Builder.Properties;

namespace BneDev.TimesheetNinja.Bot.Builder.Dialogs
{
    // http://www.garypretty.co.uk/2017/04/13/using-scorables-for-global-message-handling-and-interrupt-dialogs-in-bot-framework/
    // https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-scorable-dialogs
    // Breaking it out in to a command: https://blog.botframework.com/2017/07/06/scorables/
    public class CommonResponsesScorable : ScorableBase<IActivity, string, double>
    {
        private class CommonResponsesDialog : IDialog<object>
        {
            private readonly string _messageToSend;

            public CommonResponsesDialog(string message)
            {
                _messageToSend = message;
            }

            public async Task StartAsync(IDialogContext context)
            {
                await context.PostAsync(_messageToSend);
                context.Done<object>(null);
            }
        }

        private readonly IDialogTask task;

        private static readonly HashSet<String> Phrases = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase)
        {
            Resources.Description_Bye,
            Resources.Description_SeeYa,
            Resources.Description_Thanks,
            Resources.Description_ThankYou
        };

        public CommonResponsesScorable(IDialogTask task)
        {
            SetField.NotNull(out this.task, nameof(task), task);
        }

        protected override async Task<string> PrepareAsync(IActivity activity, CancellationToken token)
        {
            var message = activity as IMessageActivity;

            if (message != null && !string.IsNullOrWhiteSpace(message.Text))
            {
                var msg = message.Text.Trim();

                if (Phrases.Contains(msg))
                {
                    return message.Text;
                }
            }

            return await Task.FromResult<string>(null);
        }

        protected override bool HasScore(IActivity item, string state)
        {
            return state != null;
        }

        protected override double GetScore(IActivity item, string state)
        {
            return 1.0;
        }

        protected override async Task PostAsync(IActivity item, string state, CancellationToken token)
        {
            var message = item as IMessageActivity;

            if (message != null)
            {
                var incomingMessage = message.Text.Trim();
                var messageToSend = string.Empty;

                var thankYouPhrases = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase)
                {
                    Resources.Description_Thanks,
                    Resources.Description_ThankYou
                };
                if (thankYouPhrases.Contains(incomingMessage))
                    messageToSend = "You are welcome!";

                var byePhrases = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase)
                {
                    Resources.Description_Bye,
                    Resources.Description_SeeYa
                };
                if (byePhrases.Contains(incomingMessage))
                {
                    messageToSend = "See you later";

                }

                var commonResponsesDialog = new CommonResponsesDialog(messageToSend);
                var interruption = commonResponsesDialog.Void<object, IMessageActivity>();
                this.task.Call(interruption, null);
                await this.task.PollAsync(token);
            }
        }

        protected override Task DoneAsync(IActivity item, string state, CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }


}