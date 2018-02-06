# Introduction
A chat bot prototype to explore a conversational interface for time and expenses. This demo is built-on the [Microsoft Bot Framework](https://dev.botframework.com/) with Language Understanding Intelligent Service [LUIS](https://luis.ai/) and it runs on [Azure Bot Service](https://azure.microsoft.com/en-us/services/bot-service/). Use this as an accelerator to create a chat bot to compliment any boring old time and expense capture entry, or just to learn more about bots!  
What are you waiting for, check out the [Timesheet Ninja Bot Live Demo](https://timesheetninjabot.azurewebsites.net)
I hope this inspires you to explore conversation as a platform. If something should be as simple as sending a text (or responding to voice for that matter), then, maybe it should be, and is especially cool if you want a more human-like interaction.  

For developers, what else can you get from this prototype?
- A sample technical architecture for building out bot experiences designed to cater for new users and power users alike with a hybrid IVR-esque-menu and natural language approach. The IVR-esque-menu approach is great for getting started but once users are acquainted with what your bot can do chances are they'll want to get straight to the point and ask for what they need, their way.  
- Help you gauge how much LUIS does for you versus where you have to take over. For example, think about how you train LUIS to recognize synonyms for a time reporting period but you still have to convert-to or calculate an actual period start/end dates. See LuisResultExtensions.cs  
- Leverage the base classes & framework bits to more rapidly build forms that help you take care of all the bits around the edges. For example, bootstrapping forms with data extracted from LUIS intents so you only need to prompt for missing fields, handling users' cancelling out of a form flow (or going back) versus the form aborting in the case where a backend API is unavailable, see FormDialogBase<T>  
- See some basic conversational patterns in action, for example, help the user without excessively reconfirming the user's intent or decision, but, providing an option to _Undo_ and thus helping you to avoid common pitfalls of [bot navigation design](https://docs.microsoft.com/en-us/bot-framework/bot-service-design-navigation) _(an entertaining read by the way)_
- Deploy Bot resources in Azure and setup CI/CD to deploy your bot and reach the users on their channel (like Facebook Messenger, Slack, and so on). May the Agile and DevOps be with you...  

## Potential Enhancementschallenges
- Going beyond simple rich text / markdown (MD) with Adapative Cards
- Add speech to the web chat channel
- Localization (e.g. currently accepts input in your locale, but is hard-wired to present back in en-AU format)
 
## Challenges
- Training LUIS to match projects, which are referred to by arbitrary descriptions of variable length. Consider a few ways you might log time to a project:
>Log 8h on &lt;vandelay industries&gt; <-- By client name
>Log 8h on &lt;vandelay&gt; <-- By client name shortened
>8 hours on project &lt;import/export digitalisation&gt; <-- By project name (maybe with or without client name)
And, all the variations of that:
>Assign 8 hours to &lt;vandelay&gt; <-- By client name, in an utterance with different structure & verbs
>Yesterday I worked on the project at &lt;vandelay&gt; <-- By client name, prefixed with the project
>8h on VX80099 <-- By project code
>8 hours on VI <-- By acronym for Vandelay Industries
Now, without preloading every an exhaustive list of project code and project descriptions, is this training enough for LUIS to effectively recognise other abitrary project descriptions and codes of variable length? Like:
>8 hours on Novotel
>8 hours Wayne enterprises
>9.5 hours on AZ789AB

I had a hard time training LUIS to recognise arbitrary project descriptions (or codes) within utterances. LUIS recommends providing about 5 examples of what a user might say (utterances) for an intent. 
To match all of the projects in the demo dataset I found the number of utterances kept creeping up, and up, and up. I wasn't fully comfortable with that approach because the more utterances you provide, I think, the harder it is to tell how effective each individual utterance is in helping LUIS recognise an intent.
And, with each entry there were some inherent assumptions about the form of what project descriptions and codes look like, and it was really a guessing game, e.g. maybe if I add a description that contains a noun and a verb it will recognise that, and so on. In a larger real-world data sample I expect some projects may not be recognised and slip through. 
What I found enhanced LUIS extraction/recognition of projects was to really emphasize it, ironically, using the markdown emphasize syntax \__the project I want you to recognize LUIS\__
Of course it was the same effect if I used another convention, like @mention or #hashtag, but those are both taken and in messenger (or any chat context) they do have their own meaning. So I've gone with the emphasis: \__project description or code inside here\__
>Assign 8h to \__project 404\__
>8h on \__RAD\__ yesterday

Now, there's also the notion of standing projects such as sick leave, annual leave, etc. These ones are captured with the help of a List entity so can be more easily recognised within an intent and don't require to be escaped in the \__project\__ emphasis convention. It doesn't hurt if you do, I was __sick__ yesterday; the bot is wired-up to give precedence to the standing project - but that's just an implementation.
The reason I like this approach is because it takes the raw project term/text the user provides in an utterance, and uses as a basis to perform a multi-facet fuzzy-search on projects to narrow down the matches, hopefully, to 1, or a few. Because there's too many to present all at once :) This helps account for typos, among other things, and I think really enhances the likelyness of matching a project.  

Another approach is to load all project codes and descriptions and variations (such as ngrams), to build a List Entity, which could act somewhat like a search index. But list-Entity-item matches in LUIS are exact (not-fuzzy), so it's not a silverbullet. Maybe The MS Bot team will add a fuzzy option for List Entity items in the future? If you're interested in this approach then check out this _BneDev.TimesheetNinjaBot.Controllers.DefaultController.GetProjectSearchListEntity_. You can adapt this export to build up the List Entity/Search Index and transform to the simple format required for loading a list entity in LUIS, see <https://docs.microsoft.com/pt-br/azure/cognitive-services/luis/add-entities>. _admittedly I almost gave up training LUIS to recognise projects, and started down this path, but my preferred approach got me the outcome I wanted in the end :)_  

I also tried utterances that used the term 'project' excessively, it still didn't seem to be as effective as a syntactical convention like @mention, #hastag or _emphasis_, and it certainly isn't as natural or concise to type out all the time:
>Assign 8 hours to project \__RAD\__ yesterday
>I worked on 8 hours on the project at \__Carman's\__

This means assigning time via a voice command is a little cumbersome, requiring two-steps, aligning more to an IVR-esque experience.

# Getting Started
`git clone https://github.com/kierenh/TimesheetNinjaBot.git 

# Build and Test
At a super high-level:
1. Clone the repo
1. Create a Microsoft Bot App
1. Create a luis.ai model - upload the model included with this solution and publish
1. Provison Azure resources: Bot Service, Cognitive Services, Azure Table storage or Cosmos DB for bot state
1. Link Bot Service to LUIS
1. Update web.config with Microsoft App ID and LUIS configuration settings
1. Build and run the Bot locally using the Microsoft Bot Framework emulator
1. Import the VSTS Build and Release templates
1. Commit, push to trigger a Build and Release which deploys the Azure Bot Service

See _Management_ section below for links to help you through these steps.

## Management
Here's a collection of links you'll find useful through out the DEV-TEST and operational cycle:
Manage Bot/Registration Here: <https://dev.botframework.com/>  
Manage App of the Bot: <https://apps.dev.microsoft.com/> (provides access to things like making the App available as a web API, a requirement for registering with Skype for Business Online)  
[Creating a Skype for Business Bot](https://msdn.microsoft.com/en-us/skype/skype-for-business-bot-framework/docs/overview#create-bot) - A good end-to-end guide - from registering with the MS Bot Framework to Adding to Skype for Business (O365 tenant). _It might also be possible to this via [Register on Skype for Business](https://skypeappregistration.azurewebsites.net/) but it didn't seem to work, it is a PREVIEW feature)_  
Manage LUIS model: <https://www.luis.ai/applications/{GUID}/> where GUID is your LUIS model identifier  
Configure direct Line Channel (for voice via web chat): <https://docs.microsoft.com/en-us/bot-framework/bot-service-channel-connect-directline>
Azure Resource Group: <https://portal.azure.com/#resource/subscriptions/{GUID}/resourceGroups/{RESOURCE_GROUP}/overview>  where GUID is your Azure subscription ID & RESOURCE_GROUP is the resource group where your bot is contained
Should you consider deploying to O365 (a DEV tenant, or your PRODUCTION tenant), that's here: <https://aka.ms/admincenter> && end user access: <https://portal.office.com>  
App Service: <https://TODO.azurewebsites.net>

## Links and Resources
MS Bot Framework Class Library Reference: <https://docs.microsoft.com/en-us/dotnet/api/?view=botbuilder-3.8>  
[Principles of Bot Design](https://docs.microsoft.com/en-us/bot-framework/bot-service-design-principles)  
LUIS Guides: <https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/plan-your-app>  
LUIS Dialog Reference: <https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-luis-dialogs>  
Bot channel inspector: <https://docs.botframework.com/en-us/channel-inspector/channels/SkypeForBusiness/#navtitle> (see what messages look like in a particular channel)  
Bot channel inspector doc: <https://docs.microsoft.com/en-us/bot-framework/portal-channel-inspector>  
[Register a bot](https://docs.microsoft.com/en-us/bot-framework/portal-register-bot)  
[Skype for Business PowerShell Module](https://www.microsoft.com/en-au/download/details.aspx?id=39366) (Skype for Business Online, Windows PowerShell Module allows management of a Skype for Business Online deployment and Skype for Business Online user accounts through the use of Windows PowerShell.)  
[Autofac doc](http://autofac.readthedocs.io/en/latest/getting-started/index.html)  
[Setup an O365 Tenant via Visual Studio Subscription](https://support.microsoft.com/en-au/help/4019175/visual-studio-subscriptions-the-office-365-developer-subscription-bene)  
[Working with Files in Azure App Service](https://www.michaelcrump.net/azure-tips-and-tricks20/) (for investigating/troubleshooting files deployed to App Service)  
[Deployment Slots](https://stackify.com/azure-deployment-slots/)  
[Bot Troubleshooting](https://docs.microsoft.com/en-us/bot-framework/troubleshoot-general-problems)  
[Microsoft.Bot.Builder.Azure](https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure)  
[Custom State Sample](https://github.com/Microsoft/BotBuilder-Samples/tree/master/CSharp/core-CustomState)  
[Bootstrap 4](https://getbootstrap.com/docs/4.0/examples/starter-template/)  
